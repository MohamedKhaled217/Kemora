using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IPhotoRepository : IRepository<Photo>
    {
        Task<IEnumerable<Photo>> GetByPlaceIdAsync(int placeId);
        Task<Photo?> GetMainPhotoAsync(int placeId);
    }
}
