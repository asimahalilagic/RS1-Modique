using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; } = string.Empty; 
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
