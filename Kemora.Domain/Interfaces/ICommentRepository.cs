using Kemora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByPostIdAsync(int postId, int page, int size);
        Task<int> GetCountByPostIdAsync(int postId);
    }
}
