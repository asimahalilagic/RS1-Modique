using System;

namespace Modique.Domain.Entities
{
    public class CustomerProfile
    {
        public int CustomerProfileId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool NewsletterSubscribed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
