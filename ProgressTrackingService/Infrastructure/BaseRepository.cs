using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using ProgressTrackingService.Domain.Entity;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Infrastructure.Data;

namespace ProgressTrackingService.Infrastructure
{
    public class BaseRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly FitnessAppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(FitnessAppDbContext Context)
        {
            _context = Context;
            _dbSet = _context.Set<T>();

        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        //public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? criteria = null)
        //{
        //    throw new NotImplementedException();
        //}

        //soft delete
        public virtual void Delete(T entity)
        {
            _context.Attach(entity);
            _context.Entry(entity).Property("IsDeleted").CurrentValue = true;
            _context.Entry(entity).Property("DeletedAt").CurrentValue = DateTime.Now;
        }
        public virtual void HardDelete(T entity)
        {
            var isDeleted=(bool) _context.Entry(entity).Property("IsDeleted").CurrentValue;
            if(isDeleted)
                _dbSet.Remove(entity);
            else
                Delete(entity);



        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).Property("IsDeleted").CurrentValue = true;
            }
        }

        public async Task<T> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> criteria, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            IQueryable <T>query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(criteria);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public async Task<T> GetByIdAsync(int id, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

            }
            return  await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
        public IQueryable<T> GetByUserId(int userId)
        {
            IQueryable<T> query = _dbSet;
            
            return  query.Where(e => EF.Property<int>(e, "UserId") == userId);

        }



        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }


       
        public void SaveInclude(T entity, params string[] includedProperties)
        {
            // Get or create the entry for this entity
            var entry = _context.Entry(entity);
            
            // If entity is not tracked (Detached state), attach it
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
                // Re-get the entry after attaching to ensure we have the correct state
                entry = _context.Entry(entity);
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
    }
}
