using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ShareModels.ModelViews
{
    public class PembelianModel
    {
        public Pembelian Model { get; set; }
        public List<IncomingItem> Datas { get; set; }

    }
}
