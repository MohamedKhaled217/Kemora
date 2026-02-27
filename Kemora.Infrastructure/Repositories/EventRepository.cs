using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<Event>> GetUpcomingAsync(int count)
        {
            return await _dbSet
                .Where(e => e.EndDate >= DateTime.UtcNow)
                .OrderBy(e => e.StartDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetByPlaceIdAsync(int placeId)
        {
            return await _dbSet
                .Where(e => e.PlaceID == placeId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }
    }
}
