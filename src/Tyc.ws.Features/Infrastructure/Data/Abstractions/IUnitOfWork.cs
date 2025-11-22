using Microsoft.EntityFrameworkCore.Storage;

namespace Tyc.ws.Features.Infrastructure.Data.Abstractions;
public interface IUnitOfWork : IDisposable
{
    Task<TycDbContext> GetOrCreateContextAsync();
    Task EnsureContextAsync();
    TycDbContext GetDbContext();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

}
