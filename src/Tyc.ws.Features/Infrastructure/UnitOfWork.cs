using Microsoft.EntityFrameworkCore.Storage;
using Solg.Common.Infrastructure.Fw.Data;
using Tyc.ws.Features.Infrastructure.Data.Abstractions;
using IUnitOfWork = Tyc.ws.Features.Infrastructure.Data.Abstractions.IUnitOfWork;

namespace Tyc.ws.Features.Infrastructure.Data;
public class UnitOfWork : IUnitOfWork, IAsyncDisposable, IDisposable
{
    private readonly IDbContextProvider _dbContextProvider;
    private TycDbContext? _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public UnitOfWork(IDbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public async Task<TycDbContext> GetOrCreateContextAsync()
    {
        _context ??= await _dbContextProvider.CrearDbContextAsync<TycDbContext>();
        return _context;
    }

    public async Task EnsureContextAsync() => await GetOrCreateContextAsync();

    public TycDbContext GetDbContext()
    {
        if (_context == null)
        {
            throw new InvalidOperationException("El contexto no ha sido inicializado. Llame a un método async primero.");
        }
        return _context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await (await GetOrCreateContextAsync()).SaveChangesAsync(cancellationToken);

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        TycDbContext context = await GetOrCreateContextAsync();
        _currentTransaction ??= await context.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
            }
            if (_context != null)
            {
                await _context.DisposeAsync();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _currentTransaction?.Dispose();
            _context?.Dispose();
            _disposed = true;
        }
    }
}
