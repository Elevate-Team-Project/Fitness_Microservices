using Microsoft.EntityFrameworkCore.Storage;

namespace ProgressTrackingService.Domain.Interfaces
{
    public interface IUniteOfWork
    {
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
