using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class SearchFilter
    {
        public int SearchFilterId { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Colors { get; set; } 
        public string? Sizes { get; set; }
        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
        public int ResultsCount { get; set; }
    }
}
