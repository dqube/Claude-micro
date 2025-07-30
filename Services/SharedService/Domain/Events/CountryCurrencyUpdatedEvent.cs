using BuildingBlocks.Domain.DomainEvents;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Events;

public class CountryCurrencyUpdatedEvent : DomainEventBase
{
    public CountryCode CountryCode { get; }
    public CurrencyCode OldCurrencyCode { get; }
    public CurrencyCode NewCurrencyCode { get; }

    public CountryCurrencyUpdatedEvent(CountryCode countryCode, CurrencyCode oldCurrencyCode, CurrencyCode newCurrencyCode)
    {
        CountryCode = countryCode;
        OldCurrencyCode = oldCurrencyCode;
        NewCurrencyCode = newCurrencyCode;
    }
} 