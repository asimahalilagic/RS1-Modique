using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class ProductView
    {
        public int ProductViewId { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
        public int ViewCount { get; set; }
        public int? DwellTimeSeconds { get; set; }
    }
}
