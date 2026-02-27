using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IEnumerable<UserPoint>> GetPointHistoryAsync(string userId);
        Task AddPointsAsync(UserPoint point);

    }
}
