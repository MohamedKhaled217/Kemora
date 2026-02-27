using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IBadgeRepository : IRepository<Badge>
    {
        Task<IEnumerable<UserBadge>> GetUserBadgesAsync(string userId);
        Task<IEnumerable<ApplicationUser>> GetLeaderboardAsync(int top);
        Task AddUserBadgeAsync(UserBadge userBadge);
        Task<bool> HasBadgeAsync(string userId, int badgeId);
    }
}
