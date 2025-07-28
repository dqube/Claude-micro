using BuildingBlocks.Domain.Repository;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Repositories;

public interface IStoreRepository : IRepository<Store, StoreId>
{
    Task<Store?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Store>> GetByStatusAsync(StoreStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Store>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Store>> GetActiveStoresAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken = default);
} 