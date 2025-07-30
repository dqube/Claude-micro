using BuildingBlocks.Domain.DomainEvents;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Events;

public class CountryCreatedEvent : DomainEventBase
{
    public CountryCode CountryCode { get; }
    public CountryName Name { get; }
    public CurrencyCode CurrencyCode { get; }

    public CountryCreatedEvent(CountryCode countryCode, CountryName name, CurrencyCode currencyCode)
    {
        CountryCode = countryCode;
        Name = name;
        CurrencyCode = currencyCode;
    }
} 