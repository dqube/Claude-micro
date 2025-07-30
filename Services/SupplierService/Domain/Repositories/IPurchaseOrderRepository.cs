using BuildingBlocks.Domain.Repository;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Repositories;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder, OrderId>, IReadOnlyRepository<PurchaseOrder, OrderId>
{
    Task<PurchaseOrder?> GetWithDetailsAsync(OrderId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrder>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrder>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrder>> GetOverdueOrdersAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAmountBySupplierAsync(SupplierId supplierId, DateTime? fromDate = null, CancellationToken cancellationToken = default);
} 