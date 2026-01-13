using System;

namespace Modique.Domain.Entities
{
    public class BannerAd
    {
        public int BannerAdId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public string Position { get; set; } = "Home"; 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int Clicks { get; set; }
    }
}
