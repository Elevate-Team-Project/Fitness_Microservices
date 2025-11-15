using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Infrastructure
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Implement IBaseRepository methods
        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => Microsoft.EntityFrameworkCore.EF.Property<int>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(criteria);
        }
 
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void SaveInclude(T entity, params string[] includedProperties)
        {
            var LocalEntity = _dbSet.Local.FirstOrDefault(e => e.Id == entity.Id);
            EntityEntry entry;

            if (LocalEntity == null)
            {
                entry = _context.Entry(entity);
            }
            else
            {
                entry = _context.ChangeTracker.Entries<T>().First(e => e.Entity.Id == entity.Id);
            }

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsPrimaryKey())
                    continue;
                else
                {
                    if (includedProperties.Contains(property.Metadata.Name))
                    {
                        property.IsModified = true;
                    }
                    else
                    {
                        property.IsModified = false;
                    }
                }
            }
        }

        // Soft Delete - marks as deleted but keeps in database
        public void Delete(T entity)
        {
            _dbSet.Attach(entity);
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            // Mark the entity as modified so EF will update it
            _context.Entry(entity).State = EntityState.Modified;
        }

        // Hard Delete - physically removes from database
        public void HardDelete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // New method to save changes from repository level
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity); // Use soft delete for range as well
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null)
        {
            if (criteria == null)
            {
                return await _dbSet.CountAsync();
            }

            return await _dbSet.CountAsync(criteria);
        }

        // Keep your existing additional methods as they are useful
        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            Delete(entity); // Use soft delete
            return Task.CompletedTask;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // Extension methods to support LINQ operations
        public async Task<List<T>> ToListAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.MaxAsync(selector);
        }

        public async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return await _dbSet.MinAsync(selector);
        }

        // Fixed SumAsync and AverageAsync methods
        public async Task<int> SumAsync(Expression<Func<T, int>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<long> SumAsync(Expression<Func<T, long>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<double> SumAsync(Expression<Func<T, double>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<float> SumAsync(Expression<Func<T, float>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<int?> SumAsync(Expression<Func<T, int?>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<long?> SumAsync(Expression<Func<T, long?>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<double?> SumAsync(Expression<Func<T, double?>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<float?> SumAsync(Expression<Func<T, float?>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        // AverageAsync methods
        public async Task<double> AverageAsync(Expression<Func<T, int>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<double> AverageAsync(Expression<Func<T, long>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<double> AverageAsync(Expression<Func<T, double>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<float> AverageAsync(Expression<Func<T, float>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<double?> AverageAsync(Expression<Func<T, int?>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<double?> AverageAsync(Expression<Func<T, long?>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<decimal?> AverageAsync(Expression<Func<T, decimal?>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<double?> AverageAsync(Expression<Func<T, double?>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }

        public async Task<float?> AverageAsync(Expression<Func<T, float?>> selector)
        {
            return await _dbSet.AverageAsync(selector);
        }
    }
}
