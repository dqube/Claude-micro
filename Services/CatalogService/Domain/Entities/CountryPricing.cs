using BuildingBlocks.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Entities;

public class CountryPricing : Entity<PricingId>
{
    public ProductId ProductId { get; private set; }
    public CountryCode CountryCode { get; private set; }
    public Price Price { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    private CountryPricing() : base(PricingId.New())
    {
        ProductId = ProductId.New();
        CountryCode = CountryCode.From("US");
        Price = Price.Zero;
        EffectiveDate = DateTime.Today;
    }

    public CountryPricing(
        PricingId id,
        ProductId productId,
        CountryCode countryCode,
        Price price,
        DateTime effectiveDate,
        Guid? createdBy = null) : base(id)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        CountryCode = countryCode ?? throw new ArgumentNullException(nameof(countryCode));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        EffectiveDate = effectiveDate.Date;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void UpdatePrice(Price newPrice, DateTime effectiveDate, Guid updatedBy)
    {
        ArgumentNullException.ThrowIfNull(newPrice);

        Price = newPrice;
        EffectiveDate = effectiveDate.Date;
        CreatedBy = updatedBy;
    }
}