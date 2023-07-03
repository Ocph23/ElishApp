using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class PembelianItem
    {
        public int Id { get; set; }

        public double Amount { get; set; }

        private double price;
        public double Price
        {
            get
            {
                if (price <= 0 && this.Unit != null)
                {
                    price = this.Unit.Buy;
                }
                return price;
            }
            set { price = value; }
        }


        public virtual Unit Unit { get; set; }

        public virtual Product Product { get; set; }
        public double Discount { get; set; }

        public virtual double Total
        {
            get
            {
                return Price * Amount;
            }
        }

        public virtual double DiscountView
        {
            get
            {
                return Total * Discount / 100;
            }
        }


      //  public virtual Pembelian Pembelian { get; set; }

    }
}


