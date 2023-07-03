using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShareModels
{
    public class Pembelian
    {
        public Pembelian()
        {
            Incomingitem = new HashSet<IncomingItem>();
            PembayaranPembelian = new HashSet<PembayaranPembelian>();
            Items = new HashSet<PembelianItem>();
        }

        public int Id { get; set; }
        public virtual string Nomor
        {
            get
            {
                return $"{Id}/PEMB-APS/{CreatedDate.Month}/{CreatedDate.Year}";
            }
        }

        public int OrderPembelianId { get; set; }

        public int DeadLine { get; set; }
        public DateTime CreatedDate { get; set; }
        public string InvoiceNumber { get; set; }
        public PaymentStatus Status { get; set; }
        [NotMapped]
        public DateTime PayDeadLine
        {
            get
            {
                return CreatedDate.AddDays(DeadLine);
            }
        }

        [NotMapped]
        public virtual double Total
        {
            get
            {
                if (Items == null)
                    return 0;
                return Items.Sum(x => x.Total);
            }
        }

        [NotMapped]
        public virtual double TotalDiscount
        {
            get
            {
                if (Items == null)
                    return 0;
                return Items.Sum(x => x.DiscountView);
            }
        }

        public Gudang Gudang { get; set; }
        public virtual OrderPembelian OrderPembelian { get; set; }
        public virtual ICollection<PembelianItem> Items { get; set; }
        public virtual ICollection<IncomingItem> Incomingitem { get; set; }
        public virtual ICollection<PembayaranPembelian> PembayaranPembelian { get; set; } = new List<PembayaranPembelian>();
    }
}


