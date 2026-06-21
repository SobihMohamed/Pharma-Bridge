using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Notification;
using PharmaBridge.Shared.Dto_s.Notificaiton;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.EnumHelper.NotificationEnums;

namespace PharmaBridge.Abstraction.IServices.Notification
{
    // This interface defines the contract for the notification service
    // which will be responsible for sending notifications to users based on different strategies 
    // (e.g., email, SMS, push notifications).
    public interface INotificationService
    {
        Task SendNotificationAsync(NotificationContentDto message, params NotificationType[] types);

        // --- User/Client Operations (The Bell Icon) ---
        Task<PaginationResponse<NotificationDto>> GetUserNotificationsAsync(string userId, NotificationQueryParams queryParams);
        Task<bool> MarkAsReadAsync(int notificationId, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);

    }
}
