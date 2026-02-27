using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, int page, int size);
        Task<int> GetCountByUserIdAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAllAsReadAsync(string userId);
    }
}
