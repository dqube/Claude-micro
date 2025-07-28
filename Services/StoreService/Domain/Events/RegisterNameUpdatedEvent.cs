using BuildingBlocks.Domain.DomainEvents;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class RegisterNameUpdatedEvent : DomainEventBase
{
    public RegisterId RegisterId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public RegisterNameUpdatedEvent(RegisterId registerId, string oldName, string newName)
    {
        RegisterId = registerId;
        OldName = oldName;
        NewName = newName;
    }
} 