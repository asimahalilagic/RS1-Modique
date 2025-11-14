using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class ProductQuestion
    {
        public int ProductQuestionId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Question { get; set; } = string.Empty;
        public string? Answer { get; set; }
        public DateTime AskedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AnsweredAt { get; set; }
        public int? AdminUserId { get; set; }
        public User? AdminUser { get; set; }
        public bool IsPublished { get; set; }
    }
}
