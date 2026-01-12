using System;

namespace Modique.Domain.Entities
{
    public class WishlistItem
    {
        public int WishlistItemId { get; set; }
        public int WishlistId { get; set; }
        public Wishlist? Wishlist { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
