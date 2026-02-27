using Kemora.Domain.Interfaces;
using Kemora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _ctx;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public virtual void Update(T entity) => _dbSet.Update(entity);
        public virtual void Remove(T entity) => _dbSet.Remove(entity);
        
        public virtual async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }


    }
}
