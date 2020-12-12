using AspNetCore.Reporting;
using AspNetCore.Reporting.ReportExecutionService;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Controllers
{
    public class ReportController : Controller
    {
        private IWebHostEnvironment _iwebhost;
        private IProductService _productService;
        private IPenjualanService _penjualanService;
        private IPembelianService _pembelianService;

        // GET: ReportController

        public ReportController(IWebHostEnvironment iwebhost,
            IProductService productService,IPembelianService pembelianService           ,
            IPenjualanService penjualanService)
        {
            _iwebhost = iwebhost;
            _productService = productService;
            _penjualanService = penjualanService;
            _pembelianService = pembelianService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> PrintOrder(int id)
        {
            var data = await _penjualanService.GetOrder(id);
            var path = $"{_iwebhost.WebRootPath}\\Reports\\NotaOrder.rdlc";
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
                    No=nomor,
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

            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("HeaderNota", new List<ShareModels.Reports.NotaPenjualan>() {nota});
            localReport.AddDataSource("DataNota", datas);
            var result = localReport.Execute(RenderType.ExcelOpenXml, 1, null, "");
            return File(result.MainStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }





        public async Task<ActionResult> PrintPenjualan(int id)
        {
            var data = await _penjualanService.GetPenjualan(id);
            var path = $"{_iwebhost.WebRootPath}\\Reports\\NotaPenjualan.rdlc";

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

            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("HeaderNota", new List<ShareModels.Reports.NotaPenjualan>() { nota });
            localReport.AddDataSource("DataNota", datas);
            var result = localReport.Execute(RenderType.Pdf, 1, null, "");
            return File(result.MainStream, "application/pdf");
        }


        public async Task<ActionResult> OrderPembelianExcel(int id)
        {

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
           
            try
            {
                var pembelian = await _pembelianService.GetOrder(id);
                string fileName = $"Order Pembelian No:{pembelian.Nomor}.xlsx";

                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =   workbook.Worksheets.Add("Order");
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
                    for (int index = start; index <= items.Count; index++)
                    {
                        var total = items[index - 1].Total;
                        worksheet.Cell(index + 1, 1).Value = nomor;
                        worksheet.Cell(index + 1, 2).Value = items[index - 1].Product.CodeArticle;
                        worksheet.Cell(index + 1, 3).Value = items[index - 1].Product.Name+" | "+ items[index - 1].Product.Size;
                        worksheet.Cell(index + 1, 4).Value = items[index - 1].Amount;
                        worksheet.Cell(index + 1, 5).Value = items[index - 1].Price;
                       

                        worksheet.Cell(index + 1, 6).Value =total * pembelian.Discount/100;
                        worksheet.Cell(index + 1, 7).Value = total - (total * pembelian.Discount / 100);

                        worksheet.Cell(index + 1, 5).Style.NumberFormat.Format = "0,000.00";
                        worksheet.Cell(index + 1, 6).Style.NumberFormat.Format = "0,000.00";
                        worksheet.Cell(index + 1, 7).Style.NumberFormat.Format = "0,000.00";
                        nomor++;
                    }

                    worksheet.Range($"A{start}:G{items.Count}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range($"A{start}:G{items.Count+1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    foreach (var item in worksheet.ColumnsUsed())
                    {
                        item.AdjustToContents();
                    }


                    worksheet.Cell(items.Count + 4, 1).Value = "Hormat Kami";
                    worksheet.Cell(items.Count + 8, 1).Value = "Elish";
                    var rangeAsign = worksheet.Range($"A{items.Count+4}:B{items.Count+4}");
                    rangeAsign.Merge().Style.Font.SetBold().Font.FontSize = 12;

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
