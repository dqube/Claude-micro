using BuildingBlocks.Domain.DomainEvents;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Events;

public class ReturnDetailAddedEvent : DomainEventBase
{
    public ReturnId ReturnId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }
    public ReturnReason Reason { get; }
    public bool Restock { get; }

    public ReturnDetailAddedEvent(ReturnId returnId, Guid productId, int quantity, ReturnReason reason, bool restock)
    {
        ReturnId = returnId;
        ProductId = productId;
        Quantity = quantity;
        Reason = reason;
        Restock = restock;
    }
}