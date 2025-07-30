using BuildingBlocks.Domain.DomainEvents;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Events;

public class CurrencySymbolUpdatedEvent : DomainEventBase
{
    public CurrencyCode CurrencyCode { get; }
    public CurrencySymbol OldSymbol { get; }
    public CurrencySymbol NewSymbol { get; }

    public CurrencySymbolUpdatedEvent(CurrencyCode currencyCode, CurrencySymbol oldSymbol, CurrencySymbol newSymbol)
    {
        CurrencyCode = currencyCode;
        OldSymbol = oldSymbol;
        NewSymbol = newSymbol;
    }
} 