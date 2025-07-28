using BuildingBlocks.Domain.DomainEvents;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class RegisterCreatedEvent : DomainEventBase
{
    public RegisterId RegisterId { get; }
    public StoreId StoreId { get; }
    public string Name { get; }

    public RegisterCreatedEvent(RegisterId registerId, StoreId storeId, string name)
    {
        RegisterId = registerId;
        StoreId = storeId;
        Name = name;
    }
} 