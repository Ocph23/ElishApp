using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class Unit
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Amount { get; set; }

        public int Level { get; set; }

        public double Buy { get; set; }

        public double Sell { get; set; }

        public int ProductId { get; set; }


        public Unit()
        {
            OrderpembelianItem = new HashSet<OrderpembelianItem>();
            OrderPenjualanItem = new HashSet<OrderPenjualanItem>();
            PembelianItem = new HashSet<PembelianItem>();
            Penjualanitem = new HashSet<Penjualanitem>();
        }

        public virtual Product Product { get; set; }
        public virtual ICollection<OrderpembelianItem> OrderpembelianItem { get; set; }
        public virtual ICollection<OrderPenjualanItem> OrderPenjualanItem { get; set; }
        public virtual ICollection<PembelianItem> PembelianItem { get; set; }
        public virtual ICollection<Penjualanitem> Penjualanitem { get; set; }

    }
}


