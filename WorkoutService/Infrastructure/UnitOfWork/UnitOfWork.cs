 using Microsoft.EntityFrameworkCore.Storage;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

        }

        // Transaction methods
        public Task<IDbContextTransaction> BeginTransactionAsync()
            => _context.Database.BeginTransactionAsync();

        public int Complete() => _context.SaveChanges();

        public Task<int> CompleteAsync() => _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
