using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetPagedAsync(int page, int size);
        Task<int> GetCountAsync();
        Task<Post?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Post>> GetByUserIdAsync(string userId, int page, int size);
        Task<int> GetCountByUserIdAsync(string userId);
    }
}
