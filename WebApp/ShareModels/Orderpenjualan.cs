using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShareModels
{
    public class Orderpenjualan   :BaseNotify
    {
        public Orderpenjualan()
        {
            Items = new HashSet<OrderPenjualanItem>();
        }
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public double DeadLine { get => _deadLIne; set => SetProperty(ref _deadLIne, value); }
        public int CustomerId { get => _customerId; set => SetProperty(ref _customerId, value); }
        public double Discount { get; set; }
        public int? SalesId { get; set; }
        public OrderStatus Status { get => status; set => SetProperty(ref status, value); }

        #region virtual 
        public virtual string Nomor
        {
            get
            {
                return $"{Id}/SO-ELISH/{OrderDate.Month}/{OrderDate.Year}";
            }
        }
        public virtual ICollection<OrderPenjualanItem> Items { get; set; }

        [NotMapped]
        public PaymentType PaymentType { get => DeadLine<=0? PaymentType.PayOff: PaymentType.Credit; }

        [NotMapped]
        public virtual double Total
        {
            get
            {
                if (Items == null)
                   _total= 0;
                _total= Items.Sum(x => x.Total);
                return _total;
            }

            set
            {
                _total = value;
            }
        }

        public virtual Customer Customer { get; set; }
        public virtual Karyawan Sales { get; set; }

        #endregion


        #region fields 
        private double _total;
        private int _customerId;
        private OrderStatus status;
        private double _deadLIne;
        #endregion

        public virtual Penjualan Penjualan { get; set; }
    }
}


