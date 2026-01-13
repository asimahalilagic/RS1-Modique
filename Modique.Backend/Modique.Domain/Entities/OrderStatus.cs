using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class OrderStatus
    {
        public int OrderStatusId { get; set; }
        public string Name { get; set; } = string.Empty; 


        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
