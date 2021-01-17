using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels.ModelViews
{
    public class PenjualanViewModel
    {
        public int Id { get; set; }

        public virtual string Nomor
        {
            get
            {
                return $"{Id}/INV-ELISH/{CreateDate.Month}/{CreateDate.Year}";
            }
        }

        public int OrderPenjualanId { get; set; }

        public DateTime CreateDate { get; set; }

        public double PayDeadLine { get; set; }

        public double Discount { get; set; }

        public PaymentType Payment { get; set; }
        public PaymentStatus Status { get; set; }
        public ActivityStatus Activity { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public string SupplierName { get; set; }
        public string CustomerName { get; set; }
        public string SalesName { get; set; }
        public string Merk { get; set; }

        public string Name { get; set; }

        public string CodeName { get; set; }

        public string CodeArticle { get; set; }

        public string Size { get; set; }

        public double Total => Price * Amount;
        public double TotalDiscount => (Total * this.Discount)/100;
        public double TotalView => Total - TotalDiscount;
        public string AmountView { get; set; }
        public string TotalDiscountView { get; set; }
        public string GrandTotal { get; set; }

        public string ProductView => $"{Name} {Size}";

    }
}
