using BuildingBlocks.Domain.DomainEvents;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class CashRemovedEvent : DomainEventBase
{
    public RegisterId RegisterId { get; }
    public decimal Amount { get; }
    public string Note { get; }

    public CashRemovedEvent(RegisterId registerId, decimal amount, string note)
    {
        RegisterId = registerId;
        Amount = amount;
        Note = note;
    }
} 