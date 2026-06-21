using PharmaBridge.Shared.DTOs.Notificaiton;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.Notification
{
    // This interface defines the contract for pushing web notifications to users.
    public interface IWebNotificationPusher
    {
        // this method is responsible for pushing a notification to a user asynchronously.
        Task PushToUserAsync(NotificationContentDto notificationContentDto);
    }
}
