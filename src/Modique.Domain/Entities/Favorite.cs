using System;

namespace Modique.Domain.Entities
{
    public class Favorite
    {
        public int FavoriteId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
