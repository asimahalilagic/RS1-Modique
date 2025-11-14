using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class Color
    {
        public int ColorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ColorCode { get; set; } = "#FFFFFF";


        public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();

    }
}
