using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class PurchaseOrderSubmittedEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public SupplierId SupplierId { get; }
    public decimal TotalAmount { get; }

    public PurchaseOrderSubmittedEvent(OrderId orderId, SupplierId supplierId, decimal totalAmount)
    {
        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        TotalAmount = totalAmount;
    }
} 