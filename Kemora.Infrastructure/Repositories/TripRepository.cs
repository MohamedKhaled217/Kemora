using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(ApplicationDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<Trip>> GetByUserIdAsync(string userId, int page, int size)
        {
            return await _dbSet
                .Where(t => t.UserID == userId)
                .OrderByDescending(t => t.StartDate)
                .Include(t=>t.TripPlaces)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserIdAsync(string userId) => await _dbSet.CountAsync(t => t.UserID == userId);

        public async Task<Trip?> GetWithPlacesAsync(int id)
        {
            return await _dbSet
                .Include(x => x.TripPlaces).ThenInclude(tp => tp.Place)
                .FirstOrDefaultAsync(x => x.TripID == id);
        }

        public async Task<TripPlace?> GetTripPlaceAsync(int tripPlaceId)
        {
            return await _ctx.TripPlaces.FindAsync(tripPlaceId);
        }

        public async Task AddTripPlaceAsync(TripPlace tripPlace)
        {
            await _ctx.TripPlaces.AddAsync(tripPlace);
        }

        public void RemoveTripPlace(TripPlace tripPlace)
        {
            _ctx.TripPlaces.Remove(tripPlace);
        }
        
        public async Task<bool> TripPlaceExistsAsync(int tripId, int placeId)
        {
            return await _ctx.TripPlaces.AnyAsync(tp => tp.TripID == tripId && tp.PlaceID == placeId);
        }
    }
}
