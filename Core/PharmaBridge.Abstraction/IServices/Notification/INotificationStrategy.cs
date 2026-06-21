using PharmaBridge.Shared.Dto_s.Notificaiton;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.EnumHelper.NotificationEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.Notification
{
    public interface INotificationStrategy
    {
        NotificationType Type { get; }
        Task DeliverAsync(NotificationContentDto notificationContentDto);
    }
}
