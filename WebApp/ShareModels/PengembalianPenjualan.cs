using System;
using System.Collections.Generic;

namespace ShareModels
{
    public class PengembalianPenjualan : IEntity
   {
        public int Id { get; set; }
        public virtual string Nomor
        {
            get
            {
                return $"{Id}/RETURN-APS/{Created.Month}/{Created.Year}";
            }
        }
        public DateTime Created { get; set; }
        public Customer Customer { get; set; }
        public Gudang Gudang { get; set; }
        public virtual ICollection<PengembalianPenjualanItem> Items { get; set; }
        public string Description { get; set; }
    }
}


