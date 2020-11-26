using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class Orderpembelian
    {
        public int Id { get; set; }

        public virtual string Nomor
        {
            get
            {
                return $"{Id}/PO-ELISH/{OrderDate.Month}/{OrderDate.Year}";
            }
        }

        public int SupplierId { get; set; }

        public DateTime OrderDate { get; set; }

        public double Discount { get; set; }

        public virtual ICollection<OrderpembelianItem> Items { get; set; }

        private double _total;
        public virtual double Total
        {
            get
            {
                if (Items == null)
                    return 0;
                return Items.Sum(x => x.Total);
            }

            set
            {
                _total = value;
            }
        }

    }
}


