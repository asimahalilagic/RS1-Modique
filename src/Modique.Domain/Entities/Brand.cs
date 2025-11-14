using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
