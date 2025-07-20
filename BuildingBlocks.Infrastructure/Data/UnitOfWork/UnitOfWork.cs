using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Data.Repositories;

namespace BuildingBlocks.Infrastructure.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _dbContext;
    private readonly ConcurrentDictionary<string, object> _repositories = new();
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public UnitOfWork(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IRepository<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : class, IStronglyTypedId
    {
        var key = $"{typeof(TEntity).Name}_{typeof(TId).Name}_Repository";
        return (IRepository<TEntity, TId>)_repositories.GetOrAdd(key, _ => new Repository<TEntity, TId>(_dbContext));
    }

    public IReadOnlyRepository<TEntity, TId> ReadOnlyRepository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : class, IStronglyTypedId
    {
        var key = $"{typeof(TEntity).Name}_{typeof(TId).Name}_ReadOnlyRepository";
        return (IReadOnlyRepository<TEntity, TId>)_repositories.GetOrAdd(key, _ => new ReadOnlyRepository<TEntity, TId>(_dbContext));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress");
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction in progress");
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction in progress");
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void ClearChangeTracker()
    {
        _dbContext.ChangeTracker.Clear();
    }

    public Task<bool> HasActiveTransactionAsync()
    {
        return Task.FromResult(_currentTransaction != null);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _currentTransaction?.Dispose();
            _dbContext?.Dispose();
            _disposed = true;
        }
    }
}