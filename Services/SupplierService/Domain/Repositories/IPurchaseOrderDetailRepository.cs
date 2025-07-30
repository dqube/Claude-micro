using BuildingBlocks.Domain.Repository;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Repositories;

public interface IPurchaseOrderDetailRepository : IRepository<PurchaseOrderDetail, OrderDetailId>, IReadOnlyRepository<PurchaseOrderDetail, OrderDetailId>
{
    Task<IEnumerable<PurchaseOrderDetail>> GetByOrderIdAsync(OrderId orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrderDetail>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrderDetail>> GetPendingReceiptsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrderDetail>> GetPartiallyReceivedAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalQuantityByProductAsync(Guid productId, DateTime? fromDate = null, CancellationToken cancellationToken = default);
} 