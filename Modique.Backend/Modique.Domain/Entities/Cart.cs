using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
