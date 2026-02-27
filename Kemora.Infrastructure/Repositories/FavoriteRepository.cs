using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _ctx;
        public FavoriteRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<bool> IsFavoritedAsync(string userId, int placeId)
        {
            return await _ctx.UserFavorites.AnyAsync(f => f.UserID == userId && f.PlaceID == placeId);
        }

        public async Task<IEnumerable<UserFavorite>> GetByUserIdAsync(string userId)
        {
            return await _ctx.UserFavorites
                .Include(f => f.Place)
                .Where(f => f.UserID == userId)
                .ToListAsync();
        }

        public async Task AddAsync(UserFavorite favorite) => await _ctx.UserFavorites.AddAsync(favorite);

        public void Remove(UserFavorite favorite) => _ctx.UserFavorites.Remove(favorite);

        public async Task<UserFavorite?> GetAsync(string userId, int placeId)
        {
            return await _ctx.UserFavorites
                .FirstOrDefaultAsync(f => f.UserID == userId && f.PlaceID == placeId);
        }


    }
}
