using BuildingBlocks.Domain.DomainEvents;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class RegisterOpenedEvent : DomainEventBase
{
    public RegisterId RegisterId { get; }
    public StoreId StoreId { get; }
    public decimal StartingCash { get; }

    public RegisterOpenedEvent(RegisterId registerId, StoreId storeId, decimal startingCash)
    {
        RegisterId = registerId;
        StoreId = storeId;
        StartingCash = startingCash;
    }
} 