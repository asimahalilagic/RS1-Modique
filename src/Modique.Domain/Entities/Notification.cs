using System;

namespace Modique.Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}
