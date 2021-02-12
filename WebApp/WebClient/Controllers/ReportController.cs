using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebClient.Models;

using FastReport.Utils;
using FastReport;
using FastReport.Export.Html;
using System.Data;
using System.ComponentModel;
using FastReport.Web;

namespace WebClient.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _iwebhost;
        private readonly IReportService _reportService;
        private readonly IProductService _productService;
        private readonly IPenjualanService _penjualanService;
        private readonly IPembelianService _pembelianService;
        //private readonly IOptions<AppSettings> _appSettings;

        public ReportController(IWebHostEnvironment iwebhost, IReportService reportService, IProductService productService,  IPembelianService pembelianService, IPenjualanService penjualanService)
        {
            _iwebhost = iwebhost;
            _reportService = reportService;
            _productService = productService;
            _penjualanService = penjualanService;
            _pembelianService = pembelianService;
          //  _appSettings = appSettings;
        }

        public async Task<ActionResult> PrintPenjualan(int id)
        {
            var reportItem = "nota1.frx";
            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/{reportItem}";
                
                try
                {
                    var data = await _penjualanService.GetPenjualan(id);
                    var nota = GetNotaParameters(data, data.GetType());
                    var datas = new List<ShareModels.Reports.NotaData>();
                    int nomor = 1;
                    foreach (var item in data.Items)
                    {
                        datas.Add(new ShareModels.Reports.NotaData
                        {
                            No = nomor,         
                            Amount = item.Amount,
                            CodeArticle = item.Product.CodeArticle,
                            CodeProduct = item.Product.CodeName,
                            ProductName = $"{item.Product.Name} {item.Product.Size}",
                            Unit = item.Unit.Name,
                            Price = item.Price,
                            Total = item.Total,    

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

        public async Task<ActionResult> PrintPiutang()
        {
            var reportItem = "piutang.frx";
            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/{reportItem}";

                try
                {
                    var param1 = DateTime.Now.ToString("dd-MM-yyyy");
                    IEnumerable<Penjualan> data = (await _reportService.GetPiutang());
                    var datas = new List<ShareModels.Reports.PiutangData>();
                    foreach (var item in data)
                    {
                        datas.Add(new ShareModels.Reports.PiutangData
                        {
                            Nomor = item.Nomor,
                            Customer = item.OrderPenjualan.Customer.Name,
                            JatuhTempo = item.CreateDate.AddDays(item.PayDeadLine),
                            Tagihan = item.Total,
                            Panjar = item.Pembayaranpenjualan.Sum(x=>x.PayValue),
                            Sisa = item.Total - item.Pembayaranpenjualan.Sum(x => x.PayValue),
                        });
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

        public async Task<ActionResult> PrintStock(int id)
        {
            var reportItem = "stock.frx";
            if (reportItem != null)
            {
                var path = $"{_iwebhost.WebRootPath}/reports/{reportItem}";

                try
                {
                    var supplier = "ALL";
                    IEnumerable<ShareModels.ModelViews.ProductStock> data = new List<ShareModels.ModelViews.ProductStock>();
                    if(id<=0)
                        data = (await _productService.GetProductStock()).OrderBy(x=>x.Name);
                    else
                    {
                        data = (await _productService.GetProductStock()).Where(x => x.SupplierId == id).OrderBy(x => x.Name);
                        supplier = data.FirstOrDefault().Supplier.Nama;
                    }
                    var datas = new List<ShareModels.Reports.NotaData>();
                    int nomor = 1;
                    foreach (var item in data)
                    {
                        datas.Add(new ShareModels.Reports.NotaData
                        {
                            No = nomor,
                            Amount = item.StockView,
                            CodeArticle = item.CodeArticle,
                            CodeProduct = item.CodeName,
                            ProductName = $"{item.Name} {item.Size}",
                            Unit = item.SelectedUnit.Name,
                            Price = item.SelectedUnit.Sell,
                            Size = item.Size,
                            Total = item.SelectedUnit.Sell * item.StockView,

                        });

                        nomor++;
                    }

                    var datasets = datas.ToDataTable();
                    return PrintStockAction(datasets, supplier, path);

                }
                catch (Exception)
                {
                    return new NoContentResult();
                }
            }
            else
                return NotFound();
        }

        private ActionResult PrintStockAction(DataTable datasets, string param, string path)
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

                    report.SetParameterValue("Supplier", param);

                    report.Prepare();
                    HTMLExport html = new HTMLExport
                    {
                        SinglePage = true, //report on the one page
                        Navigator = true, //navigation panel on top
                        EmbedPictures = true,
                        Print=true,  Preview=true, 
                        PageBreaks=true
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
                ds.WriteXml($"{_iwebhost.WebRootPath}/reports/nota1.xml");

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
                        Print=true,  Preview=true, 
                        PageBreaks=false
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
                    foreach (var item in data.Items)
                    {
                        datas.Add(new ShareModels.Reports.NotaData
                        {
                            No = nomor,
                            Amount = item.Amount,
                            CodeArticle = item.Product.CodeArticle,
                            CodeProduct = item.Product.CodeName,
                            ProductName = $"{item.Product.Name} {item.Product.Size}",
                            Unit = item.Unit.Name,
                            Price = item.Price,
                            Total = item.Total,

                        });

                        nomor++;
                    }

                    var datasets = datas.OrderBy(x=>x.ProductName).ToList().ToDataTable();
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

        private static void SetParameter(Report report, ShareModels.Reports.NotaPenjualan nota)
        {
            report.SetParameterValue("NomorSO", nota.PoNumber);
            report.SetParameterValue("NomorInvoice", nota.NomorInvoice);
            report.SetParameterValue("Customer", nota.Customer);
            report.SetParameterValue("Salesman", nota.Sales);
            report.SetParameterValue("JatuhTempo", nota.InvoiceDeadLine.ToString("dd MMM yyyy"));
            report.SetParameterValue("Tanggal", nota.CreateDate.ToString("dd MMM yyyy"));
            report.SetParameterValue("Discount", nota.Discount);
            report.SetParameterValue("Address", nota.Address);
            report.SetParameterValue("Payment", nota.PaymentType);
            report.SetParameterValue("AppName", Helper.ApplicationName);
            report.SetParameterValue("DirectorName", Helper.DirectorName);
            report.SetParameterValue("OfficeTelp", Helper.OfficeTelp);
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
                        Customer = data.OrderPenjualan.Customer.Name,
                        Discount = data.Discount,
                        OrderStatus = data.Status.ToString(),
                        PoNumber = data.OrderPenjualan.Nomor,
                        Sales = data.OrderPenjualan.Sales.Name,
                        NomorInvoice = data.Nomor,
                        InvoiceDeadLine =data.CreateDate.AddDays(data.PayDeadLine),
                        PaymentType = data.Payment == PaymentType.Credit ? "Credit" : "Tunai",
                        Address = data.OrderPenjualan.Customer.Address

                    };
                }
                else
                {
                    var data = (Orderpenjualan)dataParam;
                    return new ShareModels.Reports.NotaPenjualan
                    {
                        CreateDate = data.OrderDate,
                        Customer = data.Customer.Name,
                        Discount = data.Discount,
                        OrderStatus = data.Status.ToString(),
                        PoNumber = data.Nomor,
                        Sales = data.Sales.Name,
                        NomorInvoice = data.Nomor,  
                        InvoiceDeadLine = data.OrderDate.AddDays(data.DeadLine),
                        PaymentType = data.DeadLine <=0 ? "Tunai" : "Kredit",
                        Address = data.Customer.Address

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
                var pembelian = await _pembelianService.GetOrder(id);
                string fileName = $"Order Pembelian No:{pembelian.Nomor}.xlsx";

                using var workbook = new XLWorkbook();
                IXLWorksheet worksheet = workbook.Worksheets.Add("Order");
                worksheet.Cell(1, 1).Value = "ORDER PEMBELIAN";
                worksheet.Cell(2, 1).Value = $"Kepada : {pembelian.Supplier.Nama}";

                var range = worksheet.Range("A1:G1");
                range.Merge().Style.Font.SetBold().Font.FontSize = 16;

                var range2 = worksheet.Range("A2:G2");
                range2.Merge().Style.Font.SetBold().Font.FontSize = 14;

                var start = 4;
                worksheet.Cell(start, 1).Value = "Nomor";
                worksheet.Cell(start, 2).Value = "Article";
                worksheet.Cell(start, 3).Value = "Nama";
                worksheet.Cell(start, 4).Value = "Jumlah";
                worksheet.Cell(start, 5).Value = "Harga";
                worksheet.Cell(start, 6).Value = $"Discount ({pembelian.Discount})%";
                worksheet.Cell(start, 7).Value = "Total";

                var items = pembelian.Items.ToList();
                int nomor = 1;
                for (int index = 1; index <= items.Count; index++)
                {
                    var total = items[index - 1].Total;
                    worksheet.Cell(index + start, 1).Value = nomor;
                    worksheet.Cell(index + start, 2).Value = items[index - 1].Product.CodeArticle;
                    worksheet.Cell(index + start, 3).Value = items[index - 1].Product.Name + " | " + items[index - 1].Product.Size;
                    worksheet.Cell(index + start, 4).Value = items[index - 1].Amount;
                    worksheet.Cell(index + start, 5).Value = items[index - 1].Price;


                    worksheet.Cell(index + start, 6).Value = total * pembelian.Discount / 100;
                    worksheet.Cell(index + start, 7).Value = total - (total * pembelian.Discount / 100);

                    worksheet.Cell(index + start, 5).Style.NumberFormat.Format = "0,000.00";
                    worksheet.Cell(index + start, 6).Style.NumberFormat.Format = "0,000.00";
                    worksheet.Cell(index + start, 7).Style.NumberFormat.Format = "0,000.00";
                    nomor++;
                }

                worksheet.Range($"A{start}:G{items.Count + start}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range($"A{start}:G{items.Count + start}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                foreach (var item in worksheet.ColumnsUsed())
                {
                    item.AdjustToContents();
                }


                worksheet.Cell(items.Count + 8, 1).Value = "Hormat Kami";
                worksheet.Cell(items.Count + 12, 1).Value = "Elish";
                var rangeAsign = worksheet.Range($"A{items.Count + 8}:B{items.Count + 8}");
                rangeAsign.Merge().Style.Font.SetBold().Font.FontSize = 12;

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        public  DataSet ToDataSet<OrderPenjualanItem>( List<OrderPenjualanItem> list)
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
