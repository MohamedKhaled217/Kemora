using Kemora.Domain.Entities;
using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext ctx) : base(ctx) { }

        public async Task<IEnumerable<Post>> GetPagedAsync(int page, int size)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Media)
                .Include(p => p.Reactions)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync() => await _dbSet.CountAsync();

        public async Task<Post?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Media)
                .Include(x => x.Reactions)
                .Include(x => x.Comments).ThenInclude(c => c.User)
                .Include(x => x.Comments).ThenInclude(c => c.Media)
                .Include(x => x.Comments).ThenInclude(c => c.Reactions)
                .FirstOrDefaultAsync(x => x.PostID == id);
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId, int page, int size)
        {
            return await _dbSet
                .Include(p => p.User).Include(p => p.Media)
                .Include(p => p.Reactions).Include(p => p.Comments)
                .Where(p => p.UserID == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserIdAsync(string userId) => await _dbSet.CountAsync(p => p.UserID == userId);
    }
}
