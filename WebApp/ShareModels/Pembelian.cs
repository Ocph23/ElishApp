using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class Pembelian
    {
        public int Id { get; set; }
        public virtual string Nomor
        {
            get
            {
                return $"{Id}/PEMB-ELISH/{CreatedDate.Month}/{CreatedDate.Year}";
            }
        }

        public double Discount { get; set; }
        public int OrderPembelianId { get; set; }
        public DateTime PayDeadLine { get; set; }
        public DateTime CreatedDate { get; set; }
        public string InvoiceNumber { get; set; }
        public PaymentStatus Status { get; set; }
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


        public virtual Supplier Supplier{ get; set; }
        public virtual Orderpembelian OrderPembelian { get; set; }
        public virtual ICollection<PembelianItem> Items { get; set; }





    }
}


