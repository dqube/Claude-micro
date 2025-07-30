using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class PurchaseOrderPartiallyReceivedEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public OrderDetailId OrderDetailId { get; }
    public Guid ProductId { get; }
    public int ReceivedQuantity { get; }

    public PurchaseOrderPartiallyReceivedEvent(OrderId orderId, OrderDetailId orderDetailId, Guid productId, int receivedQuantity)
    {
        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        OrderDetailId = orderDetailId ?? throw new ArgumentNullException(nameof(orderDetailId));
        ProductId = productId;
        ReceivedQuantity = receivedQuantity;
    }
} 