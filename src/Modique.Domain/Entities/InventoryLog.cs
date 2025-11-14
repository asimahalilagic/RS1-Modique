using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class InventoryLog
    {
        public int InventoryLogId { get; set; }
        public int ProductOptionId { get; set; }
        public ProductOption? ProductOption { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public string Reason { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
