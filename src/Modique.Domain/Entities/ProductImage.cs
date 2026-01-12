using System;

namespace Modique.Domain.Entities
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }


        public string ImageUrl { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsMain { get; set; }
    }
}
