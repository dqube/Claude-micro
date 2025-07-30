using BuildingBlocks.Domain.DomainEvents;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Events;

public class CurrencyCreatedEvent : DomainEventBase
{
    public CurrencyCode CurrencyCode { get; }
    public CurrencyName Name { get; }
    public CurrencySymbol Symbol { get; }

    public CurrencyCreatedEvent(CurrencyCode currencyCode, CurrencyName name, CurrencySymbol symbol)
    {
        CurrencyCode = currencyCode;
        Name = name;
        Symbol = symbol;
    }
} 