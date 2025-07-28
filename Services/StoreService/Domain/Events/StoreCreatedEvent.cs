using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class StoreCreatedEvent : DomainEventBase
{
    public StoreId StoreId { get; }
    public string Name { get; }
    public Address Address { get; }

    public StoreCreatedEvent(StoreId storeId, string name, Address address)
    {
        StoreId = storeId;
        Name = name;
        Address = address;
    }
} 