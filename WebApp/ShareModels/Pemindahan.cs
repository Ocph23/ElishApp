using System;
using System.Collections.Generic;

namespace ShareModels
{
    public class  Pemindahan :IEntity
   {
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public Gudang Dari { get; set; }
        public Gudang Tujuan { get; set; }
        public DateTime WaktuPemindahan { get; set; }
        public virtual ICollection<PemindahanItem> Items { get; set; }
        public string Description { get; set; }
    }
}


