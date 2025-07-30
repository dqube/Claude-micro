using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class PurchaseOrderCreatedEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public SupplierId SupplierId { get; }
    public int StoreId { get; }
    public DateTime OrderDate { get; }

    public PurchaseOrderCreatedEvent(OrderId orderId, SupplierId supplierId, int storeId, DateTime orderDate)
    {
        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        StoreId = storeId;
        OrderDate = orderDate;
    }
} 