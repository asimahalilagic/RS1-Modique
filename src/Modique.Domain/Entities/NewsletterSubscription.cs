using System;

namespace Modique.Domain.Entities
{
    public class NewsletterSubscription
    {
        public int NewsletterSubscriptionId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
        public string? UnsubscribeToken { get; set; }
    }
}
