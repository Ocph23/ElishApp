using System;
using System.Collections.Generic;
using System.Text;

namespace ShareModels.Reports
{
    public class Penjualan
    {
        public int PenjualanId { get; set; }
        public int OrderId { get; set; }
        public string NomorSO { get; set; }
        public string Invoice { get; set; }
        public string Customer { get; set; }
        public string Sales { get; set; }
        public DateTime Created { get; set; }
        public double DeadLine { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public double Discount { get; set; }
        public double FeeSales { get; set; }
        public double Total { get; set; }
        public double TotalAfterDiscount => Total - (Total * Discount / 100);
        public double TotalFeeSales => TotalAfterDiscount * (FeeSales / 100);
    }
}
