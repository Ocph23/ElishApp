using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShareModels
{
    public class Penjualan
    {
        public int Id { get; set; }

        public Customer Customer { get; set; }
        public Karyawan Salesman { get; set; }

        public virtual string Nomor
        {
            get
            {
                return $"{Id}/INV-APS/{CreateDate.Month}/{CreateDate.Year}";
            }
        }


        public virtual string NomorSuratJalan
        {
            get
            {
                return $"{Id}/SJ-APS/{CreateDate.Month}/{CreateDate.Year}";
            }
        }

        public DateTime CreateDate { get; set; }

        public double DeadLine { get; set; }

        [NotMapped]
        public DateTime PayDeadLine
        {
            get
            {
                return CreateDate.AddDays(DeadLine);
            }
        }

        public PaymentStatus Status { get; set; }

        public string Description { get; set; }

        public ActivityStatus Activity { get; set; }

        public double FeeSalesman { get; set; }
        public double Expedisi { get; set; }


        [NotMapped]
        public PaymentType Payment { get => DeadLine <= 0 ? PaymentType.Tunai : PaymentType.Kredit; }

        public virtual double Total
        {
            get
            {
                if (Items == null)
                    return 0;
                return Items.Sum(x => x.Total);
            }
        }

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

        public virtual OrderPenjualan OrderPenjualan { get; set; }

        public virtual ICollection<Penjualanitem> Items { get; set; }
        public virtual ICollection<PembayaranPenjualan> PembayaranPenjualan { get; set; }


       
    }
}


