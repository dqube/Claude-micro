using BuildingBlocks.Domain.Repository;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Repositories;

public interface IRegisterRepository : IRepository<Register, RegisterId>
{
    Task<IEnumerable<Register>> GetByStoreIdAsync(StoreId storeId, CancellationToken cancellationToken = default);
    Task<Register?> GetByStoreAndNameAsync(StoreId storeId, string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Register>> GetByStatusAsync(RegisterStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Register>> GetOpenRegistersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Register>> GetOpenRegistersByStoreAsync(StoreId storeId, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameInStoreAsync(StoreId storeId, string name, CancellationToken cancellationToken = default);
} 