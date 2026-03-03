using System.Linq.Expressions;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate); // للبحث بـ Criteria
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
