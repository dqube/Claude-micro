using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductCountryPricingUpdatedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public CountryCode CountryCode { get; }
    public Price Price { get; }
    public DateTime EffectiveDate { get; }

    public ProductCountryPricingUpdatedEvent(ProductId productId, CountryCode countryCode, Price price, DateTime effectiveDate)
    {
        ProductId = productId;
        CountryCode = countryCode;
        Price = price;
        EffectiveDate = effectiveDate;
    }
}