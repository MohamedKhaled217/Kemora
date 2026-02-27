using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Asp.Versioning;

namespace Kemora.Api.Controllers
{
    /// <summary>
    /// Manage user notifications: list, unread count, and mark as read.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        /// <summary>
        /// Get paginated notifications for the authenticated user.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetMyNotifications(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            return Ok(await _notificationService.GetMyNotificationsAsync(GetUserId(), page, pageSize));
        }

        /// <summary>
        /// Get the count of unread notifications.
        /// </summary>
        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            return Ok(await _notificationService.GetUnreadCountAsync(GetUserId()));
        }

        /// <summary>
        /// Mark a specific notification as read.
        /// </summary>
        [HttpPut("{id}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id, GetUserId());
            return NoContent();
        }

        /// <summary>
        /// Mark all notifications as read.
        /// </summary>
        [HttpPut("read-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(GetUserId());
            return NoContent();
        }
    }
}
