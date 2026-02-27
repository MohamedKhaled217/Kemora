using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class BadgeRepository : Repository<Badge>, IBadgeRepository
    {
        public BadgeRepository(ApplicationDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<UserBadge>> GetUserBadgesAsync(string userId)
        {
            return await _ctx.UserBadges
                .Include(ub => ub.Badge)
                .Where(ub => ub.UserID == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetLeaderboardAsync(int top)
        {
            return await _ctx.Users.OfType<ApplicationUser>()
                .OrderByDescending(u => u.TotalPoints)
                .Take(top)
                .ToListAsync();
        }

        public async Task AddUserBadgeAsync(UserBadge userBadge)
        {
            await _ctx.UserBadges.AddAsync(userBadge);
        }

        public async Task<bool> HasBadgeAsync(string userId, int badgeId)
        {
            return await _ctx.UserBadges.AnyAsync(ub => ub.UserID == userId && ub.BadgeID == badgeId);
        }
    }
}
