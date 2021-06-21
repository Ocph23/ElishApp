using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class PemindahanItem
    {
        public int Id { get; set; }

        public double Quantity { get; set; }

        public virtual Unit Unit { get; set; }

        public virtual Product Product { get; set; }

    }
}


