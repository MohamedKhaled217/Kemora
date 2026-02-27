using Kemora.Application.DTOs;
using System.Threading.Tasks;

namespace Kemora.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(string userId, int page, int pageSize);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
        Task CreateNotificationAsync(string userId, string title, string message);
    }

    public class NotificationDto
    {
        public int NotificationID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
