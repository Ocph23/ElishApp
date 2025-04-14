using System;
using System.Collections.Generic;
using System.Text;

namespace ShareModels
{
    public class Stock
    {
        public int Id { get; set; }

        public Gudang Gudang { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// Quantity of product in stock
        /// </summary>
        public double Quantity { get; set; }
        public int ProductId { get; set; }
        public int GudangId { get; set; }
    }
}
