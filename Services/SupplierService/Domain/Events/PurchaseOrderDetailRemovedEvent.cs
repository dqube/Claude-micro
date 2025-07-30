using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class PurchaseOrderDetailRemovedEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public OrderDetailId OrderDetailId { get; }
    public Guid ProductId { get; }

    public PurchaseOrderDetailRemovedEvent(OrderId orderId, OrderDetailId orderDetailId, Guid productId)
    {
        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        OrderDetailId = orderDetailId ?? throw new ArgumentNullException(nameof(orderDetailId));
        ProductId = productId;
    }
} 