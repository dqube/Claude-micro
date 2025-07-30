using BuildingBlocks.Domain.DomainEvents;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Events;

public class CurrencyNameUpdatedEvent : DomainEventBase
{
    public CurrencyCode CurrencyCode { get; }
    public CurrencyName OldName { get; }
    public CurrencyName NewName { get; }

    public CurrencyNameUpdatedEvent(CurrencyCode currencyCode, CurrencyName oldName, CurrencyName newName)
    {
        CurrencyCode = currencyCode;
        OldName = oldName;
        NewName = newName;
    }
} 