using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Notificaiton
{
    public class NotificationContentDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // For DB and Push (SignalR)
        public string? Email { get; set; } // For Email Strategy (Nullable because not all notifications need emails)
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public int? ReferenceId { get; set; }

        public object? Payload { get; set; } // Optional additional data for strategies that need it like the card of the request of patient
    }
}
