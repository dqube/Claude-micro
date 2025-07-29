using BuildingBlocks.Domain.StronglyTypedIds;

namespace CatalogService.Domain.ValueObjects;

public class PricingId : StronglyTypedId<Guid>
{
    public PricingId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PricingId cannot be empty", nameof(value));
    }
    
    public static PricingId New() => new(Guid.NewGuid());
    public static PricingId From(Guid value) => new(value);
}