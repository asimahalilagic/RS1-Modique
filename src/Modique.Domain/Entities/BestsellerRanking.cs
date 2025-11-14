using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class BestsellerRanking
    {
        public int BestsellerRankingId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Position { get; set; }
        public int SoldCount { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }
}
