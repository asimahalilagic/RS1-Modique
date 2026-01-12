using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Modique.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;


        public int CategoryId { get; set; }
        public Category? Category { get; set; }


        public int BrandId { get; set; }
        public Brand? Brand { get; set; }


        public ICollection<ProductOption> Options { get; set; } = new List<ProductOption>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<RelatedProduct> RelatedProducts { get; set; } = new List<RelatedProduct>();
        public ICollection<BestsellerRanking> BestsellerRankings { get; set; } = new List<BestsellerRanking>();
        public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
    }







}
