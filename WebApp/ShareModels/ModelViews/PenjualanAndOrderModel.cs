using ShareModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareModels.ModelViews
{
    public class PenjualanAndOrderModel: IEntity
    {
        public int Id { get; set; }
        public int PenjualanId { get; set; }
        public int OrderId { get; set; }
        public string NomorSO { get; set; }
        public string Invoice { get; set; }
        public string Customer { get; set; }
        public string Sales { get; set; }
        public DateTime Created { get; set; }
        public double DeadLine { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus PaymentStatus{ get; set; }
        public PaymentType PaymentType { get; set; }
        public double Discount { get; set; }
        public double FeeSales{ get; set; }
        public double Total { get; set; }
        public double TotalAfterDiscount => Total - (Total * Discount / 100);
        public double TotalFeeSales => TotalAfterDiscount * (FeeSales / 100);

        public Gudang Gudang { get; set; }

       
    }
}
