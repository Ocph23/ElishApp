using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShareModels
{
    public class OrderPembelianItem
    {
        [Key]
        public int Id { get; set; }

        public double Quntity { get; set; }
        

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

        public string Keterangan { get; set; }

        public virtual double Total
        {
            get
            {
                return Price * Quntity;
            }
        }

        public double Discount { get; set; }
        public virtual double DiscountView
        {
            get
            {
                return Total * Discount/100;
            }
        }
      
        public virtual Unit Unit { get; set; }

        public virtual Product Product { get; set; }

        public virtual OrderPembelian OrderPembelian { get; set; }

    }
}


