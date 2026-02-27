using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _ctx;
        public UserRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _ctx.Users.OfType<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<UserPoint>> GetPointHistoryAsync(string userId)
        {
            return await _ctx.UserPoints
                .Include(up => up.SourcePlace)
                .Where(up => up.UserID == userId)
                .OrderByDescending(up => up.GainedAt)
                .ToListAsync();
        }

        public async Task AddPointsAsync(UserPoint point)
        {
            await _ctx.UserPoints.AddAsync(point);
            var user = await GetByIdAsync(point.UserID);
            if (user != null)
            {
                user.TotalPoints += point.PointsGained;
            }
        }


    }
}
