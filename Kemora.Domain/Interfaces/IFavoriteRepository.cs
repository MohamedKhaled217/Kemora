using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<bool> IsFavoritedAsync(string userId, int placeId);
        Task<IEnumerable<UserFavorite>> GetByUserIdAsync(string userId);
        Task AddAsync(UserFavorite favorite);
        void Remove(UserFavorite favorite);
        Task<UserFavorite?> GetAsync(string userId, int placeId);

    }
}
