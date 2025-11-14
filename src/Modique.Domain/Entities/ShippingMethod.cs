using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class ShippingMethod
    {
        public int ShippingMethodId { get; set; }
        public string Name { get; set; } = string.Empty; // Standard, Express
        public decimal Price { get; set; }
        public int DeliveryDays { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
