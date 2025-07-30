using BuildingBlocks.Domain.StronglyTypedIds;

namespace EmployeeService.Domain.ValueObjects;

public class AddressId : StronglyTypedId<Guid>
{
    public AddressId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AddressId cannot be empty", nameof(value));
    }
    
    public static AddressId New() => new(Guid.NewGuid());
    public static AddressId From(Guid value) => new(value);
}