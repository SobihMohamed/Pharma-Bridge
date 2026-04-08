using PharmaBridge.Shared.Dto_s.Notificaiton;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.Notification
{
    // This interface defines the contract for the notification service
    // which will be responsible for sending notifications to users based on different strategies 
    // (e.g., email, SMS, push notifications).
    public interface INotificationService
    {
        Task SendNotificationAsync(MessageDto message, NotificationType type);

        // --- User/Client Operations (The Bell Icon) ---
        //Task<Pagination<NotificationDto>> GetUserNotificationsAsync(Guid userId);
        //Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
        //Task<bool> MarkAllAsReadAsync(Guid userId);

    }
}
