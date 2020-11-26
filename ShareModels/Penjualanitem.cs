using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{ 
     public class Penjualanitem  
   {
        public int Id { get; set; }

        public int PenjualanId { get; set; }

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

        public virtual Unit Unit { get; set; }

        public virtual Product Product { get; set; }
    }
}


