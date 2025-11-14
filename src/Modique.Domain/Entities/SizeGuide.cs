using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class SizeGuide
    {
        public int SizeGuideId { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? TableUrl { get; set; }
        public string? Description { get; set; }
    }
}
