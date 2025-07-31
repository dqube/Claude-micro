using BuildingBlocks.Domain.Repository;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace ReportingService.Domain.Repositories;

public interface ISalesSnapshotRepository : IRepository<SalesSnapshot, SalesSnapshotId>, IReadOnlyRepository<SalesSnapshot, SalesSnapshotId>
{
    /// <summary>
    /// Gets all sales snapshots for a specific sale
    /// </summary>
    /// <param name="saleId">The sale ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sales snapshots for the sale</returns>
    Task<IEnumerable<SalesSnapshot>> GetBySaleIdAsync(SaleId saleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all sales snapshots for a specific store
    /// </summary>
    /// <param name="storeId">The store ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sales snapshots for the store</returns>
    Task<IEnumerable<SalesSnapshot>> GetByStoreIdAsync(StoreId storeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sales snapshots within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sales snapshots within the date range</returns>
    Task<IEnumerable<SalesSnapshot>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sales snapshots for a specific store within a date range
    /// </summary>
    /// <param name="storeId">The store ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sales snapshots for the store within the date range</returns>
    Task<IEnumerable<SalesSnapshot>> GetByStoreAndDateRangeAsync(StoreId storeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total sales amount for a store within a date range
    /// </summary>
    /// <param name="storeId">The store ID</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total sales amount</returns>
    Task<decimal> GetTotalSalesAmountAsync(StoreId storeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sales snapshots for a specific customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sales snapshots for the customer</returns>
    Task<IEnumerable<SalesSnapshot>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
} 