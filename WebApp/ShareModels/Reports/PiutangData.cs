using System;
using System.Collections.Generic;
using System.Text;

namespace ShareModels.Reports
{
    public class PiutangData
    {
        public string Nomor { get; set; }
        public string Customer { get; set; }
        public DateTime JatuhTempo { get; set; }
        public double Tagihan { get; set; }
        public double Panjar { get; set; }
        public double Sisa { get; set; }
    }
}
