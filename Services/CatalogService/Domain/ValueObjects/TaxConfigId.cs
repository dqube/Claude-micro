using BuildingBlocks.Domain.StronglyTypedIds;

namespace CatalogService.Domain.ValueObjects;

public class TaxConfigId : StronglyTypedId<Guid>
{
    public TaxConfigId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("TaxConfigId cannot be empty", nameof(value));
    }
    
    public static TaxConfigId New() => new(Guid.NewGuid());
    public static TaxConfigId From(Guid value) => new(value);
}