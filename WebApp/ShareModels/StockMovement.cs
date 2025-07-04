using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareModels
{

    [Index(nameof(StockMovementType), nameof(ReferenceType), nameof(ReferenceId))]
    public class StockMovement
    {
        public int Id { get; set; }
        public Gudang Gudang { get; set; }
        public Product Product { get; set; }
        public double Quantity { get; set; }
        public StockMovementType StockMovementType{ get; set; }
        public int ReferenceId { get; set; }
        public ReferenceType ReferenceType { get; set; }
        public DateTime MovementDate{ get; set; }
        public int GudangId { get; set; }
        public int ProductId { get; set; }
    }
}
