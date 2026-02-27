using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class PhotoRepository : Repository<Photo>, IPhotoRepository
    {
        public PhotoRepository(ApplicationDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<Photo>> GetByPlaceIdAsync(int placeId)
        {
            return await _dbSet.Where(p => p.PlaceID == placeId).ToListAsync();
        }

        public async Task<Photo?> GetMainPhotoAsync(int placeId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PlaceID == placeId && p.IsMain);
        }
    }
}
