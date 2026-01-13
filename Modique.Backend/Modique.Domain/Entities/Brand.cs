using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string Name { get; set; } = "";
        public string Country { get; set; } = "";
        public string LogoURL { get; set; } = "";
        public ICollection<Product>? Products { get; set; }
    }
}
