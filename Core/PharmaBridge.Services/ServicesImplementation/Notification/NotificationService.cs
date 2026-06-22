using AutoMapper;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Services.Specifications.Notification;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Notification;
using PharmaBridge.Shared.Dto_s.Notificaiton;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.EnumHelper.NotificationEnums;


namespace PharmaBridge.Services.ServicesImplementation.Notification
{
    public class NotificationService(IEnumerable<INotificationStrategy> _notificationStrategies, IUnitOfWork unitOfWork, IMapper mapper) 
        : INotificationService
    {
        public async Task SendNotificationAsync(NotificationContentDto message, params NotificationType[] types)
        {
            // create the entity and save to database
            var notificationEntity = new Domain.Models.UserAccess.Notification
            {
                ApplicationUserId = message.UserId,
                Title = message.Subject,
                Description = message.Body,
                ReferenceId = message.ReferenceId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            // get the repo and save the notification

            var repository = unitOfWork.GetRepository<Domain.Models.UserAccess.Notification, int>();
            await repository.AddAsync(notificationEntity);
            await unitOfWork.SaveChangesAsync();

            message.Id = notificationEntity.Id;

            // Loop through the requested types and deliver
            foreach (var type in types)
            {
                var strategy = _notificationStrategies.FirstOrDefault(s => s.Type == type);
                if (strategy != null)
                {
                    await strategy.DeliverAsync(message);
                }
            }
        }
        
        public async Task<PaginationResponse<NotificationDto>> GetUserNotificationsAsync(string userId, NotificationQueryParams queryParams)
        {
            var repo = unitOfWork.GetRepository<Domain.Models.UserAccess.Notification, int>();

            var dataSpec = new UserNotificationsSpec(userId, queryParams);
            var countSpec = new UserNotificationsCountSpec(userId);

            var notifications = await repo.GetAllWithSpecAsync(dataSpec);
            var totalCount = await repo.GetCountAsync(countSpec);

            var mappedNotifications = mapper.Map<IReadOnlyList<NotificationDto>>(notifications);

            return new PaginationResponse<NotificationDto>(
                index: queryParams.PageIndex,
                size: queryParams.PageSize,
                total: totalCount,
                data: mappedNotifications
            );
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, string userId)
        {
            var repo = unitOfWork.GetRepository<Domain.Models.UserAccess.Notification, int>();

            var spec = new NotificationByIdAndUserSpec(notificationId, userId);
            var notification = await repo.GetByIdWithSpecAsync(spec);

            if (notification == null || notification.IsRead)
                return false;

            notification.IsRead = true;
            repo.UpdateAsync(notification);

            return await unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var repo = unitOfWork.GetRepository<Domain.Models.UserAccess.Notification, int>();

            var spec = new UnreadUserNotificationsSpec(userId);
            var unreadNotifications = await repo.GetAllWithSpecAsync(spec);

            if (unreadNotifications == null || !unreadNotifications.Any())
                return true; 

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                repo.UpdateAsync(notification);
            }

            return await unitOfWork.SaveChangesAsync() > 0;
        }

    }
}
