using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Notifications
{
    public class NotificationDto
    {
        public NotificationType NotifyType { get; }
        public string Title { get; }
        public string Description { get; }
        public bool IsRead { get; }
    }
}
