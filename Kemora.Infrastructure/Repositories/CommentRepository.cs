using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<Comment>> GetByPostIdAsync(int postId, int page, int size)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Media)
                .Include(c => c.Reactions)
                .Where(c => c.PostID == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync();
        }

        public async Task<int> GetCountByPostIdAsync(int postId) => await _dbSet.CountAsync(c => c.PostID == postId);
    }
}
