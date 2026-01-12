using System;

namespace Modique.Domain.Entities
{
    public class RelatedProduct
    {
        public int RelatedProductId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int RelatedToProductId { get; set; }
        public Product? RelatedToProduct { get; set; }
        public string RelationType { get; set; } = "Similar"; 
        public int Priority { get; set; }
    }
}
