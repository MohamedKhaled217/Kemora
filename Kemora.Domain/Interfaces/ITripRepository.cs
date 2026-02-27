using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task<IEnumerable<Trip>> GetByUserIdAsync(string userId, int page, int size);
        Task<int> GetCountByUserIdAsync(string userId);
        Task<Trip?> GetWithPlacesAsync(int id);
        Task<TripPlace?> GetTripPlaceAsync(int tripPlaceId);
        Task AddTripPlaceAsync(TripPlace tripPlace);
        void RemoveTripPlace(TripPlace tripPlace);
        Task<bool> TripPlaceExistsAsync(int tripId, int placeId);
    }
}
