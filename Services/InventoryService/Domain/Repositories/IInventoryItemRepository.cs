using BuildingBlocks.Domain.Repository;
using InventoryService.Domain.Entities;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Repositories;

public interface IInventoryItemRepository : IRepository<InventoryItem, InventoryItemId>
{
    Task<InventoryItem?> GetByStoreAndProductAsync(StoreId storeId, ProductId productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryItem>> GetByStoreAsync(StoreId storeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(StoreId storeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryItem>> GetOutOfStockItemsAsync(StoreId storeId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(StoreId storeId, ProductId productId, CancellationToken cancellationToken = default);
}