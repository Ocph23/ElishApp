using ClosedXML.Excel;
using ShareModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApsWebApp
{
    public static class Helper
    {
        public static string ApplicationName => "Alpha Papua Sejahtera";
        public static string DirectorName => "Herman Hamid";
        public static string OfficeTelp => "0967 - 5186704";
        public static string OfficeAddress => "Perum Permata Indah No 216";
        public static string Bank => "Bank Mandiri";
        public static string AccountBank => "Ajenkris Yanto Kungkung";
        public static string Rekening => "154-00-1395694-5";

        public static double DefaultFeeSalesman => 3;

        public static JsonSerializerOptions JsonOption { get; set; } = new() { PropertyNameCaseInsensitive = true };

        public static StringContent GenerateHttpContent(object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }


        public static DataTable ToDataTable<T>(this List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static string ImagePath => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products/");
        public static string VideoPath => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/videos/");
        public static string ThumbPath => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/thumbs/");
        public static string ProfilePath => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles/");
        public static string LogoPath => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/logo/");

        public static byte[] CreateThumb(byte[] byteArray)
        {
            try
            {
                System.Drawing.Image imThumbnailImage;
                System.Drawing.Image OriginalImage;
                MemoryStream ms = new MemoryStream();

                // Stream / Write Image to Memory Stream from the Byte Array.
                ms.Write(byteArray, 0, byteArray.Length);

                OriginalImage = System.Drawing.Image.FromStream(ms);

                // Shrink the Original Image to a thumbnail size.
                imThumbnailImage = OriginalImage.GetThumbnailImage(300, 300, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallBack), IntPtr.Zero);

                // Save Thumbnail to Memory Stream for Conversion to Byte Array.
                MemoryStream myMS = new MemoryStream();
                imThumbnailImage.Save(myMS, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] test_imge = myMS.ToArray();
                return test_imge;
            }
            catch (System.Exception)
            {
                return byteArray;
            }
        }
        public static byte[] CreateThumbVideo(byte[] byteArray)
        {
            try
            {

                System.Drawing.Image imThumbnailImage;
                System.Drawing.Image OriginalImage;
                MemoryStream ms = new MemoryStream();

                // Stream / Write Image to Memory Stream from the Byte Array.
                ms.Write(byteArray, 0, byteArray.Length);

                OriginalImage = System.Drawing.Image.FromStream(ms);

                // Shrink the Original Image to a thumbnail size.
                imThumbnailImage = OriginalImage.GetThumbnailImage(75, 75, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallBack), IntPtr.Zero);

                // Save Thumbnail to Memory Stream for Conversion to Byte Array.
                MemoryStream myMS = new MemoryStream();
                imThumbnailImage.Save(myMS, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] test_imge = myMS.ToArray();
                return test_imge;
            }
            catch (System.Exception)
            {
                return byteArray;
            }
        }

        private static bool ThumbnailCallBack()
        {
            return false;
        }

        internal static string GetPath(string fileType)
        {
            if (fileType.Contains("video"))
                return VideoPath;
            return ImagePath;

        }

        internal static string CreateFileName(string fileType)
        {
            Guid guid = Guid.NewGuid();
            if (fileType.Contains("image"))
                return guid.ToString() + ".png";

            return guid.ToString() + ".mp4";


        }

        internal static byte[] CreateFileOrderPembelian(string contentType, OrderPembelian pembelian, string imagePath)
        {
            LoadOptions.DefaultGraphicEngine = new ClosedXML.Graphics.DefaultGraphicEngine("Carlito");
            string fileName = $"Order Pembelian No:{pembelian.Nomor}.xlsx";

            using var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Order");

            if (!string.IsNullOrEmpty(imagePath))
            {
                var image = worksheet.AddPicture(imagePath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(.5); // optional: resize picture
                image.Width = 60;
                image.Height= 60;
            }

            worksheet.Cell(1, 2).Value = "ALPHA PAPUA SEJAHTERA";
            worksheet.Cell(2, 2).Value = "ORDER PEMBELIAN";
            worksheet.Cell(4, 1).Value = $"Tanggal";
            worksheet.Cell(5, 1).Value = $"Nomor";
            worksheet.Cell(6, 1).Value = $"Kepada ";

            worksheet.Cell(4, 2).Value = $": {pembelian.OrderDate.ToString("dd/MM/yyyy")}";
            worksheet.Cell(5, 2).Value = $": {pembelian.Nomor}";
            worksheet.Cell(6, 2).Value = $": {pembelian.Supplier.Nama}";


            var range = worksheet.Range("A1:E2");
            range.Style.Font.SetBold().Font.FontSize = 14;

            //var range2 = worksheet.Range("A2:G2");
            //range2.Merge().Style.Font.SetBold().Font.FontSize = 14;

            var start = 8;
            worksheet.Cell(start, 1).Value = "Nomor";
            worksheet.Cell(start, 2).Value = "Article/Kode";
            worksheet.Cell(start, 3).Value = "Nama";
            worksheet.Cell(start, 4).Value = "Jumlah";
            worksheet.Cell(start, 5).Value = "Keterangan";
            //worksheet.Cell(start, 5).Value = "Harga";
            //worksheet.Cell(start, 6).Value = $"Discount";
            //worksheet.Cell(start, 7).Value = "Total";

            var items = pembelian.Items.ToList();
            int nomor = 1;
            for (int index = 1; index <= items.Count; index++)
            {
                var total = items[index - 1].Total;
                worksheet.Cell(index + start, 1).Value = nomor;
                worksheet.Cell(index + start, 2).Value = items[index - 1].Product.CodeName;
                worksheet.Cell(index + start, 3).Value = items[index - 1].Product.Name + " | " + items[index - 1].Product.Size;
                worksheet.Cell(index + start, 4).Value = $"{items[index - 1].Quntity/12} Lusin";
                worksheet.Cell(index + start, 5).Value = items[index - 1].Keterangan;
                //worksheet.Cell(index + start, 6).Value = total * 10 / 100;
                //worksheet.Cell(index + start, 7).Value = total - (total * 10 / 100);
                //worksheet.Cell(index + start, 5).Style.NumberFormat.Format = "0,000.00";
                //worksheet.Cell(index + start, 6).Style.NumberFormat.Format = "0,000.00";
                //worksheet.Cell(index + start, 7).Style.NumberFormat.Format = "0,000.00";
                nomor++;
            }

            worksheet.Range($"A{start}:E{items.Count + start+1}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"A{start}:E{items.Count + start+1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //foreach (var item in worksheet.ColumnsUsed())
            //{
            //    item.AdjustToContents();
            //}

            var rowTotal = items.Count + start + 1;

            worksheet.Cell(rowTotal, 1).Value = "Total";
            worksheet.Range($"A{rowTotal}:C{rowTotal}").Merge().Style.Font.SetBold().Font.SetFontSize(11);
            worksheet.Cell(rowTotal, 4)
                .SetValue($"{items.Sum(x => x.Quntity / 12)} Lusin")
                .Style.Font.SetBold().Font.SetFontSize(11);
            
            worksheet.Cell(rowTotal + 2, 2).Value = "Hormat Kami";
            worksheet.Cell(rowTotal + 3, 2).Value = "Elish";
            worksheet.Range($"A{rowTotal + 2}:E{rowTotal + 3}").Style.Font.SetBold().Font.FontSize = 12;


            worksheet.Column("A").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Column("A").Width = 10;
            worksheet.Column("B").AdjustToContents();
            worksheet.Column("C").AdjustToContents();
            worksheet.Column("D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Column("D").Width = 12;
            worksheet.Column("E").Width = 15;
            worksheet.Row(1).AdjustToContents();
            worksheet.Row(2).AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return content;
        }
    }
}
