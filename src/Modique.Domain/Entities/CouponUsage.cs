using System;

namespace Modique.Domain.Entities
{
    public class CouponUsage
    {
        public int CouponUsageId { get; set; }
        public int CouponId { get; set; }
        public Coupon? Coupon { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}
