using BuildingBlocks.Domain.StronglyTypedIds;

namespace SalesService.Domain.ValueObjects;

public class SaleId : StronglyTypedId<Guid>
{
    public SaleId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("SaleId cannot be empty", nameof(value));
    }
    
    public static SaleId New() => new(Guid.NewGuid());
    
    public static SaleId From(Guid value) => new(value);
}