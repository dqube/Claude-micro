using BuildingBlocks.Domain.StronglyTypedIds;

namespace CustomerService.Domain.ValueObjects;

public class ContactNumberId : StronglyTypedId<Guid>
{
    public ContactNumberId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ContactNumberId cannot be empty", nameof(value));
    }
    
    public static ContactNumberId New() => new(Guid.NewGuid());
    
    public static ContactNumberId From(Guid value) => new(value);
} 