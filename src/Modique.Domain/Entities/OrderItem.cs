using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductOptionId { get; set; }
        public ProductOption? ProductOption { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
