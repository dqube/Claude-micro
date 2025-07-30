using BuildingBlocks.Domain.Repository;
using InventoryService.Domain.Entities;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Domain.Repositories;

public interface IStockMovementRepository : IRepository<StockMovement, StockMovementId>
{
    Task<IReadOnlyList<StockMovement>> GetByProductAsync(ProductId productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovement>> GetByStoreAsync(StoreId storeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovement>> GetByStoreAndProductAsync(StoreId storeId, ProductId productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovement>> GetByMovementTypeAsync(MovementTypeValue movementType, CancellationToken cancellationToken = default);
}