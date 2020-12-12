﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShareModels.Reports
{
   public class NotaPenjualan
    {
        public string NomorInvoice { get; set; }
        public string Customer { get; set; }
        public string PoNumber { get; set; }
        public double Discount { get; set; }
        public string PaymentType { get; set; }
        public DateTime InvoiceDeadLine { get; set; }
        public DateTime CreateDate { get; set; }
        public string PaymentStatus { get; set; }
        public string Sales { get; set; }
        public string OrderStatus { get; set; }

    }


    public class NotaData
    {
        public int No { get; set; }
        public string CodeArticle { get; set; }
        public string CodeProduct { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public double Amount { get; set; }
        public string DiscountValue { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
    }



}
