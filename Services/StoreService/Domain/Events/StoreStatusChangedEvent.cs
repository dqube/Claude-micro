using BuildingBlocks.Domain.DomainEvents;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class StoreStatusChangedEvent : DomainEventBase
{
    public StoreId StoreId { get; }
    public StoreStatus OldStatus { get; }
    public StoreStatus NewStatus { get; }

    public StoreStatusChangedEvent(StoreId storeId, StoreStatus oldStatus, StoreStatus newStatus)
    {
        StoreId = storeId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
} 