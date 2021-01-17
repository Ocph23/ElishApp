using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class Pembelian
    {
        public Pembelian()
        {
            Incomingitem = new HashSet<IncomingItem>();
            Pembayaranpembelian = new HashSet<Pembayaranpembelian>();
            Items = new HashSet<PembelianItem>();
        }

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


        public virtual Orderpembelian OrderPembelian { get; set; }
        public virtual ICollection<PembelianItem> Items { get; set; }
        public virtual ICollection<IncomingItem> Incomingitem { get; set; }
        public virtual ICollection<Pembayaranpembelian> Pembayaranpembelian { get; set; }




      
      

       


    }
}


