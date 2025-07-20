using Microsoft.EntityFrameworkCore.Storage;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using BuildingBlocks.Infrastructure.Data.Repositories;

namespace BuildingBlocks.Infrastructure.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : class, IStronglyTypedId;

    IReadOnlyRepository<TEntity, TId> ReadOnlyRepository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : class, IStronglyTypedId;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    void ClearChangeTracker();
    Task<bool> HasActiveTransactionAsync();
}