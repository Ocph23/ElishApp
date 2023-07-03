using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using ShareModels;
using FastReport.Utils;
using FastReport;
using FastReport.Export.Html;
using System.Data;
using ShareModels.ModelViews;
using System.Diagnostics;
using ApsWebApp.Data;
using Microsoft.EntityFrameworkCore;
using ApsWebApp.Services;

namespace ApsWebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _iwebhost;
        private readonly IReportService _reportService;
        private readonly IProductService _productService;
        private readonly IPenjualanService _penjualanService;
        private readonly IPembelianService _pembelianService;
        private readonly ApplicationDbContext _dbContext;
        Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private IEmailService emailService;

        //private readonly IOptions<AppSettings> _appSettings;

        public ReportController(ApplicationDbContext dbContext, IWebHostEnvironment iwebhost, IReportService reportService,
            IProductService productService, IPembelianService pembelianService, IPenjualanService penjualanService,
            IEmailService _emailService, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _iwebhost = iwebhost;
            _reportService = reportService;
            _productService = productService;
            _penjualanService = penjualanService;
            _pembelianService = pembelianService;
            _dbContext = dbContext;
            emailService = _emailService;
            _env = env;
        }


        public async Task<ActionResult> PrintPenjualan(int id)
        {
            var reportItem = "nota.frx";
            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/frxs/{reportItem}";

                try
                {
                    var data = await _penjualanService.GetPenjualan(id);
                    var nota = GetNotaParameters(data, data.GetType());
                    var datas = new List<ShareModels.Reports.NotaData>();
                    int nomor = 1;
                    foreach (var item in data.Items.OrderBy(x => x.Product.Name))
                    {
                        datas.Add(new ShareModels.Reports.NotaData
                        {
                            No = nomor,
                            Amount = item.Quantity,
                            CodeArticle = item.Product.CodeArticle,
                            CodeProduct = item.Product.CodeName,
                            ProductName = $"{item.Product.Name} {item.Product.Size}",
                            Unit = item.Unit.Name,
                            Price = item.Price,
                            Total = item.Total,
                            DiscountValue = item.DiscountView
                        });

                        nomor++;
                    }

                    var datasets = datas.ToDataTable();
                    return Print(datasets, nota, path);

                }
                catch (Exception)
                {
                    return new NoContentResult();
                }
            }
            else
                return NotFound();
        }
        public async Task<ActionResult> PrintSuratJalan(int id)
        {
            var reportItem = "suratjalan.frx";
            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/frxs/{reportItem}";

                try
                {
                    var data = await _penjualanService.GetPenjualan(id);
                    var nota = GetNotaParameters(data, data.GetType());
                    var datas = new List<ShareModels.Reports.NotaData>();
                    int nomor = 1;
                    foreach (var item in data.Items.OrderBy(x => x.Product.Name))
                    {
                        datas.Add(new ShareModels.Reports.NotaData
                        {
                            No = nomor,
                            Amount = item.Quantity,
                            CodeArticle = item.Product.CodeArticle,
                            CodeProduct = item.Product.CodeName,
                            ProductName = $"{item.Product.Name} {item.Product.Size}",
                            Unit = item.Unit.Name,
                            Price = item.Price,
                            Total = item.Total,
                            DiscountValue = item.DiscountView
                        });

                        nomor++;
                    }

                    var datasets = datas.ToDataTable();
                    return Print(datasets, nota, path);

                }
                catch (Exception)
                {
                    return new NoContentResult();
                }
            }
            else
                return NotFound();
        }


        public async Task<ActionResult> PrintLaporanPenjualan([FromQuery] int id, [FromQuery] string ddstart, [FromQuery] string ddend)
        {
            DateTime? dstart = Convert.ToDateTime(ddstart);
            DateTime? dend = Convert.ToDateTime(ddend);
            await Task.Delay(1);
            var reportItem = "laporanpenjualan.frx";
            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/frxs/{reportItem}";

                try
                {
                    var source = _dbContext.Penjualan.Where(x => x.CreateDate >= dstart.Value && x.CreateDate <= dend.Value)
            .Include(x => x.Salesman)
            .Include(x => x.Customer)
            .Include(x => x.Items).ThenInclude(x => x.Unit)
            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Supplier)
            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units).AsEnumerable();
                    var datas = from item in source
                                select new ShareModels.Reports.Penjualan
                                {
                                    Created = item.CreateDate,
                                    Customer = item.Customer.Name,
                                    Sales = item.Salesman.Name,
                                    DeadLine = item.DeadLine,
                                    Invoice = item.Nomor,
                                    NomorSO = item.Nomor,
                                    OrderId = item.Id,
                                    PaymentType = item.Payment.ToString(),
                                    PaymentStatus = item.Status.ToString(),
                                    PenjualanId = item.Id,
                                    Total = item.Total,
                                    FeeSales = item.FeeSalesman 
                                };

                    var datasets = datas.ToList().ToDataTable();
                    return PrintLaporanPenjualanAction(datasets, dstart.Value, dend.Value, path);

                }
                catch (Exception)
                {
                    return new NoContentResult();
                }
            }
            else
                return NotFound();
        }

        private ActionResult PrintLaporanPenjualanAction(DataTable datasets, DateTime dstart, DateTime dend, string path)
        {
            using MemoryStream stream = new MemoryStream();
            try
            {
                var mime = "text/" + "html"; //redefine mime for html
                datasets.TableName = "Table1";
                DataSet ds = new DataSet();
                ds.DataSetName = "Penjualan";
                ds.Tables.Add(datasets);
                ds.WriteXml($"{_iwebhost.WebRootPath}/reports/datas/laporanpenjualan.xml");

                Config.WebMode = true;
                using (Report report = new Report())
                {
                    report.Load(path); //Load the report
                    report.RegisterData(ds.Tables["Table1"], "Table1"); //Register data in the report

                    report.SetParameterValue("DateStart", dstart);
                    report.SetParameterValue("DateEnd", dend);

                    report.Prepare();
                    HTMLExport html = new HTMLExport
                    {
                        SinglePage = true, //report on the one page
                        Navigator = true, //navigation panel on top
                        EmbedPictures = true,
                        Print = true,
                        Preview = true,
                        PageBreaks = true
                    };
                    report.Export(html, stream);

                    report.GetDataSource("Table1").Enabled = true;
                }
                //Get the name of resulting report file with needed extension
                var file = String.Concat(Path.GetFileNameWithoutExtension(path), ".", "html");
                return File(stream.ToArray(), mime);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
            finally
            {
                stream.Dispose();
            }
        }

        public async Task<ActionResult> PrintPiutang()
        {
            var reportItem = "piutang.frx";
            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/{reportItem}";

                try
                {
                    var param1 = DateTime.Now.ToString("dd-MM-yyyy");
                    IEnumerable<ShareModels.Reports.PiutangData> data = (await _reportService.GetPiutang());
                    var datas = new List<ShareModels.Reports.PiutangData>();
                    foreach (var item in data)
                    {
                        datas.Add(item);
                    }

                    var datasets = datas.ToDataTable();
                    return PrintPiutangction(datasets, param1, path);

                }
                catch (Exception)
                {
                    return new NoContentResult();
                }
            }
            else
                return NotFound();
        }

        private ActionResult PrintPiutangction(DataTable datasets, string param, string path)
        {

            using MemoryStream stream = new MemoryStream();
            try
            {
                var mime = "text/" + "html"; //redefine mime for html
                datasets.TableName = "Table1";
                DataSet ds = new DataSet();
                ds.DataSetName = "Piutang";
                ds.Tables.Add(datasets);
                ds.WriteXml($"{_iwebhost.WebRootPath}/reports/Piutang.xml");

                Config.WebMode = true;
                using (Report report = new Report())
                {
                    report.Load(path); //Load the report
                    report.RegisterData(ds.Tables["Table1"], "Table1"); //Register data in the report

                    report.SetParameterValue("Periode", param);

                    report.Prepare();
                    HTMLExport html = new HTMLExport
                    {
                        SinglePage = true, //report on the one page
                        Navigator = true, //navigation panel on top
                        EmbedPictures = true,
                        Print = true,
                        Preview = true,
                        PageBreaks = true,
                    };
                    report.Export(html, stream);

                    report.GetDataSource("Table1").Enabled = true;
                }
                //Get the name of resulting report file with needed extension
                var file = String.Concat(Path.GetFileNameWithoutExtension(path), ".", "html");
                return File(stream.ToArray(), mime);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
            finally
            {
                stream.Dispose();
            }
        }


        public async Task<ActionResult> PrintStockBarang(int id, int merkid)
        {
            var reportItem = "stock.frx";
            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/{reportItem}";

                try
                {
                    var dataParams = new Dictionary<string, string>();
                    var gudangName = "All";
                    var merkName = "-";
                    if (id > 0)
                    {
                        var gudang = _dbContext.Gudang.Where(x => x.Id == id).FirstOrDefault();
                        gudangName = gudang == null ? "" : gudang.Name;
                    }
                    dataParams.Add("Gudang", gudangName);

                    if (merkid > 0)
                    {
                        var merk = _dbContext.Merk.Where(x => x.Id == merkid).FirstOrDefault();
                        merkName = merk == null ? "" : merk.Name;
                    }
                    dataParams.Add("Merk", merkName);

                    IEnumerable<ShareModels.ModelViews.ProductStock> data = new List<ShareModels.ModelViews.ProductStock>();
                    data = (await _productService.GetProductStockByGudangId(merkid, id, false)).OrderBy(x => x.Name);

                    var datas = new List<ShareModels.Reports.NotaData>();
                    int nomor = 1;
                    foreach (var item in data)
                    {
                        datas.Add(new ShareModels.Reports.NotaData
                        {
                            No = nomor,
                            Amount = item.Stock,
                            CodeArticle = item.CodeArticle,
                            CodeProduct = item.CodeName,
                            ProductName = $"{item.Name} {item.Size}",
                            Unit = item.SelectedUnit.Name,
                            Price = item.SelectedUnit.Sell,
                            Size = item.Size,
                            Total = item.SelectedUnit.Sell * item.Stock,

                        });

                        nomor++;
                    }

                    var datasets = datas.ToDataTable();
                    return PrintStockAction(datasets, dataParams, path);

                }
                catch (Exception)
                {
                    return new NoContentResult();
                }
            }
            else
                return NotFound();
        }


        //public async Task<ActionResult> PrintStock(int id)
        //{
        //    var reportItem = "stock.frx";
        //    if (reportItem != null)
        //    {
        //        var path = $"{_iwebhost.WebRootPath}/reports/{reportItem}";

        //        try
        //        {
        //            var supplier = "ALL";
        //            IEnumerable<ShareModels.ModelViews.ProductStock> data = new List<ShareModels.ModelViews.ProductStock>();
        //            if(id<=0)
        //                data = (await _productService.GetProductStock()).OrderBy(x=>x.Name);
        //            else
        //            {
        //                data = (await _productService.GetProductStock()).Where(x => x.Supplier.Id == id).OrderBy(x => x.Name);
        //                supplier = data.FirstOrDefault().Supplier.Nama;
        //            }
        //            var datas = new List<ShareModels.Reports.NotaData>();
        //            int nomor = 1;
        //            foreach (var item in data)
        //            {
        //                datas.Add(new ShareModels.Reports.NotaData
        //                {
        //                    No = nomor,
        //                    Amount = item.StockView,
        //                    CodeArticle = item.CodeArticle,
        //                    CodeProduct = item.CodeName,
        //                    ProductName = $"{item.Name} {item.Size}",
        //                    Unit = item.SelectedUnit.Name,
        //                    Price = item.SelectedUnit.Sell,
        //                    Size = item.Size,
        //                    Total = item.SelectedUnit.Sell * item.StockView,

        //                });

        //                nomor++;
        //            }

        //            var datasets = datas.ToDataTable();
        //            return PrintStockAction(datasets, supplier, path);

        //        }
        //        catch (Exception)
        //        {
        //            return new NoContentResult();
        //        }
        //    }
        //    else
        //        return NotFound();
        //}

        private ActionResult PrintStockAction(DataTable datasets, Dictionary<string, string> dataParams, string path)
        {

            using MemoryStream stream = new MemoryStream();
            try
            {
                var mime = "text/" + "html"; //redefine mime for html
                datasets.TableName = "Table1";
                DataSet ds = new DataSet();
                ds.DataSetName = "Stock";
                ds.Tables.Add(datasets);
                ds.WriteXml($"{_iwebhost.WebRootPath}/reports/Stock.xml");

                Config.WebMode = true;
                using (Report report = new Report())
                {
                    report.Load(path); //Load the report
                    report.RegisterData(ds.Tables["Table1"], "Table1"); //Register data in the report


                    foreach (var item in dataParams)
                    {
                        report.SetParameterValue(item.Key, item.Value);
                    }

                    report.Prepare();
                    HTMLExport html = new HTMLExport
                    {
                        SinglePage = true, //report on the one page
                        Navigator = true, //navigation panel on top
                        EmbedPictures = true,
                        Print = true,
                        Preview = true,
                        PageBreaks = true
                    };
                    report.Export(html, stream);

                    report.GetDataSource("Table1").Enabled = true;
                }
                //Get the name of resulting report file with needed extension
                var file = String.Concat(Path.GetFileNameWithoutExtension(path), ".", "html");
                return File(stream.ToArray(), mime);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
            finally
            {
                stream.Dispose();
            }
        }
        private ActionResult Print(DataTable datasets, ShareModels.Reports.NotaPenjualan nota, string path)
        {
            using MemoryStream stream = new MemoryStream();
            try
            {
                var mime = "text/" + "html"; //redefine mime for html
                datasets.TableName = "Table1";
                DataSet ds = new DataSet();
                ds.DataSetName = "Nota";
                ds.Tables.Add(datasets);
                ds.WriteXml($"{_iwebhost.WebRootPath}/reports/datas/nota.xml");

                Config.WebMode = true;
                using (Report report = new Report())
                {
                    report.Load(path); //Load the report
                    report.RegisterData(ds.Tables["Table1"], "Table1"); //Register data in the report
                    SetParameter(report, nota);


                    report.Prepare();
                    HTMLExport html = new HTMLExport
                    {
                        SinglePage = true, //report on the one page
                        Navigator = true, //navigation panel on top
                        EmbedPictures = true,
                        Print = true,
                        Preview = true,
                        PageBreaks = true,
                        EnableMargins = true
                    };
                    report.Export(html, stream);

                    report.GetDataSource("Table1").Enabled = true;
                }
                //Get the name of resulting report file with needed extension
                var file = String.Concat(Path.GetFileNameWithoutExtension(path), ".", "html");
                return File(stream.ToArray(), mime);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
            finally
            {
                stream.Dispose();
            }
        }

        public async Task<ActionResult> PrintOrder(int id)
        {

            var reportItem = "salesorder.frx";

            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/{reportItem}";
                try
                {
                    var data = await _penjualanService.GetOrder(id);
                    var nota = GetNotaParameters(data, data.GetType());

                    var datas = new List<ShareModels.Reports.NotaData>();
                    int nomor = 1;
                    foreach (var item in data.Items.OrderBy(x => x.Product.Name))
                    {
                        datas.Add(new ShareModels.Reports.NotaData
                        {
                            No = nomor,
                            Amount = item.Quantity,
                            CodeArticle = item.Product.CodeArticle,
                            CodeProduct = item.Product.CodeName,
                            ProductName = $"{item.Product.Name} {item.Product.Size}",
                            DiscountValue = item.DiscountView,
                            Unit = item.Unit.Name,
                            Price = item.Price,
                            Total = item.Total,

                        });

                        nomor++;
                    }

                    return Print(datas.ToDataTable(), nota, path);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return new NoContentResult();
                }
            }
            else
                return NotFound();
        }

        private static void SetParameter(Report report, ShareModels.Reports.NotaPenjualan nota)
        {
            report.SetParameterValue("NomorSO", nota.PoNumber);
            report.SetParameterValue("NomorInvoice", nota.NomorInvoice);
            report.SetParameterValue("NomorSuratJalan", nota.NomorSuratJalan);
            report.SetParameterValue("Customer", nota.Customer);
            report.SetParameterValue("Salesman", nota.Sales);
            report.SetParameterValue("JatuhTempo", nota.InvoiceDeadLine.ToString("dd MMM yyyy"));
            report.SetParameterValue("DeadLine", nota.DeadLine.ToString() + " hr");
            report.SetParameterValue("Tanggal", nota.CreateDate.ToString("dd MMM yyyy"));
            report.SetParameterValue("Discount", nota.Discount);
            report.SetParameterValue("Address", nota.Address);
            report.SetParameterValue("Payment", nota.PaymentType);
            report.SetParameterValue("AppName", Helper.ApplicationName);
            report.SetParameterValue("DirectorName", Helper.DirectorName);
            report.SetParameterValue("OfficeTelp", Helper.OfficeTelp);
            report.SetParameterValue("OfficeAddress", Helper.OfficeAddress);
            report.SetParameterValue("Ekspedisi", nota.Ekspedisi);


        }

        private static ShareModels.Reports.NotaPenjualan GetNotaParameters(object dataParam, Type type)
        {
            try
            {
                if (type == typeof(Penjualan))
                {
                    var data = (Penjualan)dataParam;
                    return new ShareModels.Reports.NotaPenjualan
                    {
                        CreateDate = data.CreateDate,
                        Customer = data.Customer.Name,
                        OrderStatus = data.Status.ToString(),
                        PoNumber = data.OrderPenjualan == null ? "" : data.OrderPenjualan.Nomor,
                        Sales = data.Salesman.Name,
                        NomorInvoice = data.Nomor,
                        NomorSuratJalan = data.NomorSuratJalan,
                        DeadLine = data.DeadLine,
                        InvoiceDeadLine = data.CreateDate.AddDays(data.DeadLine),
                        PaymentType = data.Payment == PaymentType.Kredit ? "Kredit" : "Tunai",
                        Address = data.Customer.Address,
                        Ekspedisi = data.Expedisi,
                        Discount = data.Items.Sum(x => x.DiscountView)
                    };
                }
                else
                {
                    var data = (OrderPenjualan)dataParam;
                    return new ShareModels.Reports.NotaPenjualan
                    {
                        CreateDate = data.OrderDate,
                        Customer = data.Customer.Name,
                        OrderStatus = data.Status.ToString(),
                        PoNumber = data.Nomor,
                        Sales = data.Sales.Name,
                        NomorInvoice = data.Nomor,
                        InvoiceDeadLine = data.OrderDate.AddDays(data.DeadLine),
                        PaymentType = data.DeadLine <= 0 ? "Tunai" : "Kredit",
                        DeadLine = data.DeadLine,
                        Address = data.Customer.Address,
                        Discount = data.Items.Sum(x => x.DiscountView)
                    };
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);

            }
        }


        public async Task<ActionResult> OrderPembelianExcel(int id)
        {

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            try
            {
                var logo = _env.WebRootPath + "/images/apslogo.png";
                var pembelian = await _pembelianService.GetOrder(id);
                string fileName = $"Order Pembelian No:{pembelian.Nomor}.xlsx";
                var content = Helper.CreateFileOrderPembelian(contentType, pembelian, logo);

                var base64 = Convert.ToBase64String(content);
                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
       

        public DataSet ToDataSet<OrderPenjualanItem>(List<OrderPenjualanItem> list)
        {
            Type elementType = typeof(OrderPenjualanItem);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                t.Columns.Add(propInfo.Name, ColType);
            }

            //go through each property on T and add each value to the table
            foreach (OrderPenjualanItem item in list)
            {
                DataRow row = t.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }

                t.Rows.Add(row);
            }

            return ds;
        }





    }




}
