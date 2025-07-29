using BuildingBlocks.Domain.StronglyTypedIds;

namespace CustomerService.Domain.ValueObjects;

public class CustomerId : StronglyTypedId<Guid>
{
    public CustomerId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(value));
    }
    
    public static CustomerId New() => new(Guid.NewGuid());
    
    public static CustomerId From(Guid value) => new(value);
} 