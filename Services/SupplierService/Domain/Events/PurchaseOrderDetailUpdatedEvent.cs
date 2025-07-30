using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class PurchaseOrderDetailUpdatedEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public OrderDetailId OrderDetailId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }
    public decimal UnitCost { get; }

    public PurchaseOrderDetailUpdatedEvent(OrderId orderId, OrderDetailId orderDetailId, Guid productId, int quantity, decimal unitCost)
    {
        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        OrderDetailId = orderDetailId ?? throw new ArgumentNullException(nameof(orderDetailId));
        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost;
    }
} 