using BuildingBlocks.Domain.Repository;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace ReportingService.Domain.Repositories;

public interface IInventorySnapshotRepository : IRepository<InventorySnapshot, InventorySnapshotId>, IReadOnlyRepository<InventorySnapshot, InventorySnapshotId>
{
    /// <summary>
    /// Gets all inventory snapshots for a specific product
    /// </summary>
    /// <param name="productId">The product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of inventory snapshots for the product</returns>
    Task<IEnumerable<InventorySnapshot>> GetByProductIdAsync(ProductId productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all inventory snapshots for a specific store
    /// </summary>
    /// <param name="storeId">The store ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of inventory snapshots for the store</returns>
    Task<IEnumerable<InventorySnapshot>> GetByStoreIdAsync(StoreId storeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inventory snapshots within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of inventory snapshots within the date range</returns>
    Task<IEnumerable<InventorySnapshot>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest inventory snapshot for a product at a specific store
    /// </summary>
    /// <param name="productId">The product ID</param>
    /// <param name="storeId">The store ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Latest inventory snapshot for the product at the store</returns>
    Task<InventorySnapshot?> GetLatestByProductAndStoreAsync(ProductId productId, StoreId storeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inventory snapshots for a specific product and store within a date range
    /// </summary>
    /// <param name="productId">The product ID</param>
    /// <param name="storeId">The store ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of inventory snapshots for the product and store within the date range</returns>
    Task<IEnumerable<InventorySnapshot>> GetByProductStoreAndDateRangeAsync(ProductId productId, StoreId storeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products with low inventory (below threshold) for a specific store
    /// </summary>
    /// <param name="storeId">The store ID</param>
    /// <param name="threshold">Minimum quantity threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of inventory snapshots below the threshold</returns>
    Task<IEnumerable<InventorySnapshot>> GetLowInventoryAsync(StoreId storeId, int threshold, CancellationToken cancellationToken = default);
} 