using System;

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
