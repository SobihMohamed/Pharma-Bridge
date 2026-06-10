using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.EnumHelper.NotificationEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.ServicesImplementation.Notification.StrategyPattern
{
    // This class implements the INotificationStrategy interface for push notifications.
    // It uses the IWebNotificationPusher to send notifications to users in real-time.
    public class PushedNotificationStrategy(IWebNotificationPusher webNotificationPusher) : INotificationStrategy
    {
        public NotificationType Type => NotificationType.Push;
        // The DeliverAsync method passes the notification content to the IWebNotificationPusher for delivery.
        // because signalR is used in web Project so ,
        // the webNotificationPusher will handle the logic of sending the notification
        // to the appropriate user(s) based on the content of the NotificationContentDto.
        public Task DeliverAsync(NotificationContentDto notificationContentDto)
        {
           return webNotificationPusher.PushToUserAsync(notificationContentDto);
        }
    }
}
