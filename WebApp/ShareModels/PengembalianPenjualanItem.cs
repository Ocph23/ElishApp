using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class PengembalianPenjualanItem
    {
        public int Id { get; set; }
        public double Quantity { get; set; }

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

        public double Discount { get; set; }

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

        public virtual Unit Unit { get; set; }
        public virtual Product Product { get; set; }
        public virtual Penjualan Penjualan{ get; set; }
       // public virtual PengembalianPenjualan PengembalianPenjualan  { get; set; }


        [NotMapped]
        public double Stock { get; set; }

    }
}


