using CAMPUS.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DOMAIN.Entities
{
    public class NotificationLog
    {
        public int Id { get; set; }
        public int? ServiceRequestId { get; set; }
        public NotificationType Type { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool Succeeded { get; set; }
        public string? Error { get; set; }
    }
}
