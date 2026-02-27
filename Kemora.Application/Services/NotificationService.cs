using Kemora.Application.DTOs;
using Kemora.Application.Interfaces;
using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationPusher _pusher;

        public NotificationService(INotificationRepository notificationRepo, IUnitOfWork unitOfWork, INotificationPusher pusher)
        {
            _notificationRepo = notificationRepo;
            _unitOfWork = unitOfWork;
            _pusher = pusher;
        }

        public async Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(string userId, int page, int pageSize)
        {
            var notifs = await _notificationRepo.GetByUserIdAsync(userId, page, pageSize);
            var count = await _notificationRepo.GetCountByUserIdAsync(userId);

            var items = notifs.Select(n => new NotificationDto
            {
                NotificationID = n.NotificationID,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();

            return new PagedResult<NotificationDto>
            {
                Items = items,
                TotalCount = count,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _notificationRepo.GetUnreadCountAsync(userId);
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var n = await _notificationRepo.GetByIdAsync(notificationId);
            if (n != null && n.UserID == userId)
            {
                n.IsRead = true;
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            await _notificationRepo.MarkAllAsReadAsync(userId);
            await _unitOfWork.CommitAsync();
        }

        public async Task CreateNotificationAsync(string userId, string title, string message)
        {
            var notification = new Notification
            {
                UserID = userId,
                Title = title,
                Message = message,
                CreatedAt = System.DateTime.UtcNow,
                IsRead = false
            };

            await _notificationRepo.AddAsync(notification);
            await _unitOfWork.CommitAsync();

            // Push real-time notification via SignalR
            await _pusher.PushToUserAsync(userId, title, message);
        }
    }
}
