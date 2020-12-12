using System;


namespace ShareModels
{
    public class Pembayaranpembelian 
    {
        public int Id { get; set; }

        public int PembelianId { get; set; }

        public DateTime PayDate { get; set; }

        public string PayTo { get; set; }

        public PayType PayType { get; set; }

        public string BankName { get; set; }

        public string RekNumber { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        public double PayValue { get; set; }
    }


}
