using PharmaBridge.Domain.Models.User;
using System;

namespace PharmaBridge.Domain.Models
{
    public class Notification : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }

        // 1- ApplicationUser (1) to (Many) Notifications (Get)
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
