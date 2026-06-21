using AutoMapper;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Notification;
using PharmaBridge.Shared.Dto_s.Notificaiton;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.EnumHelper.NotificationEnums;
using System;
using System.Collections.Generic;
using System.Text;

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
        public Task<PaginationResponse<NotificationDto>> GetUserNotificationsAsync(string userId, NotificationQueryParams queryParams)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkAllAsReadAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkAsReadAsync(int notificationId, string userId)
        {
            throw new NotImplementedException();
        }

    }
}
