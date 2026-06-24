using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Shared.Common.Params.Notification;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmaBridge.Presentation.Controllers.Notification
{
    [Route("api/notifications")]
    [Authorize] 
    public class NotificationController : AppBaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private string GetUserIdFromToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid User ID in token.");
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult> GetMyNotifications([FromQuery] NotificationQueryParams queryParams)
        {
            var userId = GetUserIdFromToken();
            var result = await _notificationService.GetUserNotificationsAsync(userId, queryParams);

            return Success(result, "Notifications retrieved successfully");
        }

        [HttpPatch("{id}/read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await _notificationService.MarkAsReadAsync(id, userId);

            if (result)
                return Success("Notification marked as read successfully");

            return BadRequestError("Failed to mark notification as read. It might not exist, belong to another user, or is already read.");
        }

        [HttpPatch("read-all")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var userId = GetUserIdFromToken();
            var result = await _notificationService.MarkAllAsReadAsync(userId);

            if (result)
                return Success("All notifications marked as read successfully");

            return Success("No new notifications to mark as read.");
        }
    }
}