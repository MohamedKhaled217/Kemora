using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<IEnumerable<Event>> GetUpcomingAsync(int count);
        Task<IEnumerable<Event>> GetByPlaceIdAsync(int placeId);
    }
}
