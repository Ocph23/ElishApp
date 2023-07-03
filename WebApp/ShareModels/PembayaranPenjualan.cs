using System;
using System.ComponentModel.DataAnnotations;

namespace ShareModels
{
    public class PembayaranPenjualan 
    {
        public int Id { get; set; }

        public DateTime PayDate { get; set; }

        public string PayTo { get; set; }

        public PayType PayType { get; set; }

        public string BankName { get; set; }

        public string RekNumber { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public double PayValue { get; set; }

        public virtual Penjualan Penjualan { get; set; }

    }
}
