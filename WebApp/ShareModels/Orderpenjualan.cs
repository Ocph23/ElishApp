using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareModels
{
    public class Orderpenjualan   :BaseNotify
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get => _customerId; set => SetProperty(ref _customerId, value); }
        public double Discount { get; set; }
        public int? SalesId { get; set; }
        public OrderStatus Status { get; set; }


        #region virtual 
        public virtual string Nomor
        {
            get
            {
                return $"{Id}/SO-ELISH/{OrderDate.Month}/{OrderDate.Year}";
            }
        }
        public virtual ICollection<OrderPenjualanItem> Items { get; set; }
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

        public virtual Customer Customer { get; set; }
        public virtual Karyawan Sales { get; set; }

        #endregion



        #region fields 
            #pragma warning disable IDE0052 // Remove unread private members
            private double _total;
            #pragma warning restore IDE0052 // Remove unread private members
            private int _customerId;
        #endregion
    }
}


