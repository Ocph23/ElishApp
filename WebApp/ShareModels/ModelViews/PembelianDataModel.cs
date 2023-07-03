using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ShareModels.ModelViews
{
    public class PembelianDataModel    :BaseNotify
    {
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

        private PaymentStatus status;

        public PaymentStatus Status
        {
            get { return status; }
            set { SetProperty(ref status , value); }
        }


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

        public string Gudang { get; set; }
        public string OrderNumber{ get; set; }

        public string SupplierName { get; set; }
        public int SupplierId { get; set; }

        public virtual ICollection<PembelianItem> Items { get; set; } =new List<PembelianItem>();

    }
}
