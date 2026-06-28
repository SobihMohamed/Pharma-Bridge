using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // ضروري عشان StatusCodes
using Microsoft.AspNetCore.Mvc;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Shared.Common.Pagination; 
using PharmaBridge.Shared.Common.Params.Notification;
using PharmaBridge.Shared.Common.Response; 
using PharmaBridge.Shared.DTOs.Notificaiton;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmaBridge.Presentation.Controllers.Notification
{
    [Route("api/notifications")]
    [ApiController] 
    [Authorize]
    public class NotificationController : AppBaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<NotificationDto>>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetMyNotifications([FromQuery] NotificationQueryParams queryParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return UnauthorizedError();

            var result = await _notificationService.GetUserNotificationsAsync(userId, queryParams);

            return Success(result, "Notifications retrieved successfully");
        }

        [HttpPatch("{id:int}/read")] 
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return UnauthorizedError();

            var result = await _notificationService.MarkAsReadAsync(id, userId);

            if (result)
                return Success(true, "Notification marked as read successfully");

            return BadRequestError("Failed to mark notification as read. It might not exist, belong to another user, or is already read.");
        }

        [HttpPatch("read-all")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return UnauthorizedError();

            var result = await _notificationService.MarkAllAsReadAsync(userId);

            if (result)
                return Success(true, "All notifications marked as read successfully");

            return Success(true, "No new notifications to mark as read.");
        }
    }
}