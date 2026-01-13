using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class Wishlist
    {
        public int WishlistId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }
}
