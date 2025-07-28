using BuildingBlocks.Domain.DomainEvents;
using StoreService.Domain.ValueObjects;

namespace StoreService.Domain.Events;

public class CashAddedEvent : DomainEventBase
{
    public RegisterId RegisterId { get; }
    public decimal Amount { get; }
    public string Note { get; }

    public CashAddedEvent(RegisterId registerId, decimal amount, string note)
    {
        RegisterId = registerId;
        Amount = amount;
        Note = note;
    }
} 