using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShareModels
{
    public class OrderPenjualan:BaseNotify, IEntity
    {
        public OrderPenjualan()
        {
        }
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public double DeadLine { get => _deadLIne; set => SetProperty(ref _deadLIne, value); }
        public OrderStatus Status { get => status; set => SetProperty(ref status, value); }
        public string Discription { get; set; }
        public Gudang Gudang { get; set; }

        #region virtual 
        public virtual string Nomor
        {
            get
            {
                return $"{Id}/SO-APS/{OrderDate.Month}/{OrderDate.Year}";
            }
        }
        public virtual ICollection<OrderPenjualanItem> Items { get; set; }

        [NotMapped]
        public PaymentType PaymentType { get => DeadLine<=0? PaymentType.Tunai: PaymentType.Kredit; }

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

        private Customer customer;

        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer , value); }
        }


        private Karyawan sales;

        public Karyawan Sales
        {
            get { return sales; }
            set { SetProperty(ref sales , value); }
        }

        #endregion


        #region fields 
        private OrderStatus status;
        private double _deadLIne;
        #endregion

    }
}


