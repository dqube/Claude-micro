using BuildingBlocks.Domain.StronglyTypedIds;

namespace StoreService.Domain.ValueObjects;

public class ShiftId : StronglyTypedId<Guid>
{
    public ShiftId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ShiftId cannot be empty", nameof(value));
    }
    
    public static ShiftId New() => new(Guid.NewGuid());
    public static ShiftId From(Guid value) => new(value);
} 