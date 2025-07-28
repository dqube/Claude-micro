using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class StoreInformationUpdatedEvent : DomainEventBase
{
    public StoreId StoreId { get; }
    public string OldName { get; }
    public string NewName { get; }
    public Address OldAddress { get; }
    public Address NewAddress { get; }

    public StoreInformationUpdatedEvent(StoreId storeId, string oldName, string newName, Address oldAddress, Address newAddress)
    {
        StoreId = storeId;
        OldName = oldName;
        NewName = newName;
        OldAddress = oldAddress;
        NewAddress = newAddress;
    }
} 