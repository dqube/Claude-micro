using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.StronglyTypedIds;
using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Repository;

public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : class, IStronglyTypedId
{
    public abstract Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    public abstract Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public abstract Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    public abstract Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    public abstract Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    public abstract Task<IReadOnlyList<TEntity>> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    public abstract Task<TEntity?> FindFirstAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    public abstract Task<int> CountAsync(CancellationToken cancellationToken = default);
    public abstract Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    public abstract Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    public abstract Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    public abstract Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    public abstract Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    public abstract Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    public abstract void Update(TEntity entity);
    public abstract void UpdateRange(IEnumerable<TEntity> entities);
    public abstract void Delete(TEntity entity);
    public abstract void DeleteRange(IEnumerable<TEntity> entities);
    public abstract Task<bool> DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);
}