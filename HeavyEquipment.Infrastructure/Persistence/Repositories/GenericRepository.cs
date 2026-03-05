using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.FindAsync(new object[] { id }, ct);
        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
             => await _dbSet.ToListAsync(ct);

        public async Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
            => await _dbSet.Where(predicate).ToListAsync(ct);
        public async Task<T?> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, ct);

        public async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
            => await _dbSet.AnyAsync(predicate, ct);
        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken ct = default)
            => predicate is null
                ? await _dbSet.CountAsync(ct)
                : await _dbSet.CountAsync(predicate, ct);

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await _dbSet.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
            => await _dbSet.AddRangeAsync(entities, ct);
        public void Update(T entity)
             => _dbSet.Update(entity);

        public void Remove(T entity)
            => _dbSet.Remove(entity);
        public void RemoveRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);
    }
}
