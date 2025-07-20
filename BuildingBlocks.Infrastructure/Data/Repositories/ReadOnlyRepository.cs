using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;
using BuildingBlocks.Infrastructure.Data.Context;

namespace BuildingBlocks.Infrastructure.Data.Repositories;

public class ReadOnlyRepository<TEntity, TId> : RepositoryBase<TEntity, TId>, IReadOnlyRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : class, IStronglyTypedId
{
    public ReadOnlyRepository(IDbContext dbContext) : base(dbContext)
    {
    }
}