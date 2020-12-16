using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QRCoder;
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
using AspNetCore.Reporting;
using FastReport.Web;

namespace WebClient.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _iwebhost;
        private readonly IPenjualanService _penjualanService;
        private readonly IPembelianService _pembelianService;
        private readonly IOptions<AppSettings> _appSettings;

        public ReportController(IWebHostEnvironment iwebhost, IOptions<AppSettings> appSettings, IPembelianService pembelianService, IPenjualanService penjualanService)
        {
            _iwebhost = iwebhost;
            _penjualanService = penjualanService;
            _pembelianService = pembelianService;
            _appSettings = appSettings;
        }

        public async Task<ActionResult> Index()
        {
          

            var data = await _penjualanService.GetOrder(10);
            var nota = new ShareModels.Reports.NotaPenjualan
            {
                CreateDate = data.OrderDate,
                Customer = data.Customer.Name,
                Discount = data.Discount,
                OrderStatus = data.Status.ToString(),
                PoNumber = data.Nomor,
                Sales = data.Sales.Name,
            };

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

            var report = new WebReport();
            report.Report.Load($"{_iwebhost.WebRootPath}/reports/nota1.frx");
            var datasets = datas.ToDataTable();
            report.Report.RegisterData(datasets, "Table1");
            report.ToolbarColor = System.Drawing.Color.White;
            report.ShowZoomButton = false;
            report.ShowExports = false;
            report.ShowPrint= false;

            ViewBag.WebReport = report;
            return View();
        }

        public async Task<ActionResult> PrintOrder(int id)
        {
            var data = await _penjualanService.GetOrder(id);
            var path = $"{_iwebhost.WebRootPath}/reports/notaorder.rdlc";

            var nota = new ShareModels.Reports.NotaPenjualan
            {
                CreateDate = data.OrderDate,
                Customer = data.Customer.Name,
                Discount = data.Discount,
                OrderStatus = data.Status.ToString(),
                PoNumber = data.Nomor,
                Sales = data.Sales.Name,
            };

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


            //paramas
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string barcode = GetBarCode(data.Nomor);
            var accounting = _appSettings.Value.Accounting;
            Dictionary<string, string> reportparameter = new Dictionary<string, string>();
            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("HeaderNota", new List<ShareModels.Reports.NotaPenjualan>() { nota });
            localReport.AddDataSource("DataNota", datas);


            var result = localReport.Execute(RenderType.Pdf, 1, reportparameter, "");
            return File(result.MainStream, "application/pdf");
            //var result = localReport.Execute(RenderType.ExcelOpenXml, 1, null, "");
            //return File(result.MainStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        public async Task<ActionResult> TestFastReport(int id)
        {

            var reportItem = "nota1.frx";

            string mime = "application/" + "html"; //MIME-header with default value
                                                   // Find a report
            if (reportItem != null)
            {

                var path = $"{_iwebhost.WebRootPath}/reports/{reportItem}";
                using (MemoryStream stream = new MemoryStream()) //Create the stream for the report
                {
                    try
                    {
                        var data = await _penjualanService.GetOrder(id);
                        var nota = new ShareModels.Reports.NotaPenjualan
                        {
                            CreateDate = data.OrderDate,
                            Customer = data.Customer.Name,
                            Discount = data.Discount,
                            OrderStatus = data.Status.ToString(),
                            PoNumber = data.Nomor,
                            Sales = data.Sales.Name,
                        };

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
                        DataSet ds = new DataSet();
                        ds.Tables.Add(datasets);
                        ds.WriteXml($"{_iwebhost.WebRootPath}/reports/nota1.xml");

                        Config.WebMode = true;
                        using (Report report = new Report())
                        {
                            ds.DataSetName = "Nota";
                            report.Load(path); //Load the report
                         //   report.RegisterData(ds, "Nota"); //Register data in the report
                            report.Prepare();
                            HTMLExport html = new HTMLExport();
                            html.SinglePage = true; //report on the one page
                            html.Navigator = false; //navigation panel on top
                            html.EmbedPictures = true; //build in images to the document
                            report.Export(html, stream);
                            mime = "text/" + "html"; //redefine mime for html
                        }
                        //Get the name of resulting report file with needed extension
                        var file = String.Concat(Path.GetFileNameWithoutExtension(path), ".", "html");
                        return File(stream.ToArray(), mime);
                        
                    }
                    catch(Exception ex)
                    {
                        return new NoContentResult();
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                }
            }
            else
                return NotFound();
        }



        public async Task<ActionResult> PrintPenjualan(int id)
        {

            var data = await _penjualanService.GetPenjualan(id);
            var path = $"{_iwebhost.WebRootPath}/reports/notapenjualan.rdlc";
            var nota = new ShareModels.Reports.NotaPenjualan
            {
                CreateDate = data.CreateDate,
                Customer = data.Customer.Name,
                Discount = data.Discount,
                OrderStatus = data.Status.ToString(),
                PoNumber = data.Nomor,
                Sales = data.Sales.Name,
                PaymentType = data.Payment.ToString()
            };


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

            //paramas
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string barcode = data.Nomor;
            var accounting = _appSettings.Value.Accounting;
            Dictionary<string, string> reportparameter = new Dictionary<string, string>();
            reportparameter.Add("acc", accounting);
            reportparameter.Add("customer", textInfo.ToTitleCase(data.Customer.Name));
            reportparameter.Add("barcode", barcode);

            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("HeaderNota", new List<ShareModels.Reports.NotaPenjualan>() { nota });
            localReport.AddDataSource("DataNota", datas);
            var result = localReport.Execute(RenderType.Pdf, 1, reportparameter, "");
            return File(result.MainStream, "application/pdf");
        }

        private string GetBarCode(string nomor)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(nomor, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);

                    using (Bitmap bitmap = qrCode.GetGraphic(20))
                    {
                        //bitmap.Save(ms, ImageFormat.Png);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new System.SystemException(ex.Message);
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
