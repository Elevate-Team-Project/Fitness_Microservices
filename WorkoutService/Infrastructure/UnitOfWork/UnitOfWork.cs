using Microsoft.EntityFrameworkCore.Storage;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Infrastructure.Data;
using WorkoutService.Infrastructure.Repositories;

namespace WorkoutService.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WorkoutContext _context;
        public IWorkoutRepository Workouts { get; }

        public UnitOfWork(WorkoutContext context, IWorkoutRepository workoutRepository)
        {
            _context = context;
            Workouts = workoutRepository;
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
