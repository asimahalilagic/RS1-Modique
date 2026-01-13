using System;

namespace Modique.Domain.Entities
{
    public class PriceHistory
    {
        public int PriceHistoryId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } = "Update"; 
    }
}
