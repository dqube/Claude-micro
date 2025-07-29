using BuildingBlocks.Domain.StronglyTypedIds;

namespace CatalogService.Domain.ValueObjects;

public class BarcodeId : StronglyTypedId<Guid>
{
    public BarcodeId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("BarcodeId cannot be empty", nameof(value));
    }
    
    public static BarcodeId New() => new(Guid.NewGuid());
    public static BarcodeId From(Guid value) => new(value);
}