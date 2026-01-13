using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = "";
        public string SubCategory { get; set; } = "";
        public string Description { get; set; } = "";
        public ICollection<Product>? Products { get; set; }
    }
}
