using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{
    public class OrderPenjualanItem :BaseNotify
    {
        public int Id { get; set; }

        public double Quantity { get; set; }

        public double Discount { get; set; }


        private double price;

        public double Price
        {
            get
            {
                if (price <= 0 && this.Unit!=null)
                {
                    price = this.Unit.Sell;     
                }
                return price;
            }
            set { price = value; }
        }

     //   public int UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        public virtual Product Product { get; set; }

        public virtual double Total
        {
            get
            {
                return Price * Quantity;
            }
        }

        public virtual double DiscountView
        {
            get
            {
                return Total * Discount / 100;
            }
        }

        public virtual OrderPenjualan OrderPenjualan { get; set; }
    }
}


