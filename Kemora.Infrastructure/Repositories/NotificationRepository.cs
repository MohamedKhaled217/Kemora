using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, int page, int size)
        {
            return await _dbSet
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserIdAsync(string userId)
        {
            return await _dbSet.CountAsync(n => n.UserID == userId);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _dbSet.CountAsync(n => n.UserID == userId && !n.IsRead);
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unread = await _dbSet.Where(n => n.UserID == userId && !n.IsRead).ToListAsync();
            foreach (var notification in unread)
            {
                notification.IsRead = true;
            }
        }
    }
}
