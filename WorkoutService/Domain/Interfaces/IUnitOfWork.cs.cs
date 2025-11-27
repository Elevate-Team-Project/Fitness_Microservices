using Microsoft.EntityFrameworkCore.Storage;

namespace WorkoutService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Transaction Methods
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
