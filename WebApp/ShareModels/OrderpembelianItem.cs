using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShareModels
{
    public class OrderpembelianItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderPembelianId { get; set; }

        public int ProductId { get; set; }

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

        public int UnitId { get; set; }
        public virtual double Total
        {
            get
            {
                return Price * Amount;
            }
        }

        public virtual Unit Unit { get; set; }

        public virtual Product Product { get; set; }

        public virtual Orderpembelian OrderPembelian { get; set; }

    }
}


