//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ShareModels.ModelViews
//{
//   public class PiutangModel
//    {
//        public int Id { get; set; }
//        public string Customer { get; set; }
//        public string Sales { get; set; }
//        public double Discount { get; set; }
//        public DateTime DateDeadLine { get; set; }
//        public double Terbayar { get; set; }
//        public Double Total { get; set; }

//        public double TotalAfterDiscount => Total - Total * Discount / 100;

//        public double Sisa => TotalAfterDiscount - Terbayar;
//    }
//}
