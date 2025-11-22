using System.Linq.Expressions;

namespace ProgressTrackingService.Domain.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        public IQueryable<T> GetByUserId(int userId);

        IQueryable<T> GetAll();

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void HardDelete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        public void SaveInclude(T entity, params string[] includedProperties);

        //Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null);
    }
}
