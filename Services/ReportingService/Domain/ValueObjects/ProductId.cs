using BuildingBlocks.Domain.StronglyTypedIds;

namespace ReportingService.Domain.ValueObjects;

public class ProductId : StronglyTypedId<Guid>
{
    public ProductId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(value));
    }
    
    public static ProductId New() => new(Guid.NewGuid());
    
    public static ProductId From(Guid value) => new(value);
} 