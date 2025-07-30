using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class PurchaseOrderCancelledEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public SupplierId SupplierId { get; }
    public string Status { get; }

    public PurchaseOrderCancelledEvent(OrderId orderId, SupplierId supplierId, string status)
    {
        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        Status = status ?? throw new ArgumentNullException(nameof(status));
    }
} 