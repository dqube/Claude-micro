using BuildingBlocks.Domain.StronglyTypedIds;

namespace SalesService.Domain.ValueObjects;

public class ReturnId : StronglyTypedId<Guid>
{
    public ReturnId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ReturnId cannot be empty", nameof(value));
    }
    
    public static ReturnId New() => new(Guid.NewGuid());
    
    public static ReturnId From(Guid value) => new(value);
}