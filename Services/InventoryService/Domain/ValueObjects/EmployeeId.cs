using BuildingBlocks.Domain.StronglyTypedIds;

namespace InventoryService.Domain.ValueObjects;

public class EmployeeId : StronglyTypedId<Guid>
{
    public EmployeeId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("EmployeeId cannot be empty", nameof(value));
    }
    
    public static EmployeeId New() => new(Guid.NewGuid());
    
    public static EmployeeId From(Guid value) => new(value);
}