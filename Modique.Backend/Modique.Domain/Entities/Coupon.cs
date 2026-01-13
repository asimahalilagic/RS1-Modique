using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty; 
        public string DiscountType { get; set; } = "Percent"; 
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int MaxRedemptions { get; set; }
        public int RedemptionCount { get; set; }
        public bool IsActive { get; set; }
        public ICollection<CouponUsage> Usages { get; set; } = new List<CouponUsage>();
    }
}
