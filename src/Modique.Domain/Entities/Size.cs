using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class Size
    {
        public int SizeId { get; set; }
        public string Code { get; set; } = ""; 
        public string? Description { get; set; }
    }
}
