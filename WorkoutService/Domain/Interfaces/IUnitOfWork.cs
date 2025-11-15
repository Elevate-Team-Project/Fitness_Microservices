using Microsoft.EntityFrameworkCore.Storage;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IWorkoutRepository Workouts { get; }
        Task<int> CompleteAsync();
        int Complete();
        Task<int> SaveAsync();
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
