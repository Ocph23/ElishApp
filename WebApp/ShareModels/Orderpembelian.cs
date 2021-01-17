using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class Orderpembelian
    {

        public Orderpembelian()
        {
            Items = new HashSet<OrderpembelianItem>();
        }
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

        public OrderStatus Status { get; set; }

        public virtual Supplier Supplier { get; set; }


        public virtual ICollection<OrderpembelianItem> Items { get; set; }

        private double _total;

        [NotMapped]
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

        public virtual Pembelian Pembelian { get; set; }

    }
}


