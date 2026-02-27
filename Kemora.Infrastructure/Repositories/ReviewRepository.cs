using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<Review>> GetByPlaceIdAsync(int placeId, int page, int size)
        {
            return await _dbSet
                .Where(r => r.PlaceID == placeId)
                .OrderByDescending(r => r.ReviewID)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync();
        }

        public async Task<int> GetCountByPlaceIdAsync(int placeId) => await _dbSet.CountAsync(r => r.PlaceID == placeId);
    }
}
