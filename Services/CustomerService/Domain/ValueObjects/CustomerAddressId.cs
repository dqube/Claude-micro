using BuildingBlocks.Domain.StronglyTypedIds;

namespace CustomerService.Domain.ValueObjects;

public class CustomerAddressId : StronglyTypedId<Guid>
{
    public CustomerAddressId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerAddressId cannot be empty", nameof(value));
    }
    
    public static CustomerAddressId New() => new(Guid.NewGuid());
    
    public static CustomerAddressId From(Guid value) => new(value);
} 