using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class ProductOption
    {
        public int ProductOptionId { get; set; }


        public int ProductId { get; set; }
        public Product? Product { get; set; }


        public int ColorId { get; set; }
        public Color? Color { get; set; }


        public int SizeId { get; set; }
        public Size? Size { get; set; }


        public int QuantityInStock { get; set; }
        public string SKU { get; set; } = string.Empty;


        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
    }
}
