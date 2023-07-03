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
        public string Sales { get; set; }
        public int PenjualanId { get; set; }
        public double Discount { get; set; }
        public double TagihanAfterDiscount => Tagihan - (Tagihan * Discount / 100);
        public double Sisa => TagihanAfterDiscount - Panjar;
    }
}
