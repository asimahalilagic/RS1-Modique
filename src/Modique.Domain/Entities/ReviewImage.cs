using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class ReviewImage
    {
        public int ReviewImageId { get; set; }
        public int ReviewId { get; set; }
        public Review? Review { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
