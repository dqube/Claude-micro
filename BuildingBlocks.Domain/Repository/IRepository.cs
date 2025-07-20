using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Specifications;
using BuildingBlocks.Domain.StronglyTypedIds;
using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Repository;

public interface IRepository<TEntity, TId> : IReadOnlyRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : class, IStronglyTypedId
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
    Task<bool> DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);
}