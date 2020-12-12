using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{
    public class OrderPenjualanItem     :BaseNotify
    {
        public int Id { get; set; }
        public int OrderPenjualanId { get; set; }

        public int ProductId { get; set; }


        private double amount;

        public double Amount
        {
            get { return amount; }
            set {SetProperty(ref amount , value); }
        }

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

        public int UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        public virtual Product Product { get; set; }

        public virtual double Total
        {
            get
            {
                return Price * Amount;
            }
        }
    }
}


