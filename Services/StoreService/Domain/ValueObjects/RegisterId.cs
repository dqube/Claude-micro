using BuildingBlocks.Domain.StronglyTypedIds;

namespace StoreService.Domain.ValueObjects;

public class RegisterId : StronglyTypedId<int>
{
    public RegisterId(int value) : base(value)
    {
        if (value <= 0)
            throw new ArgumentException("RegisterId must be a positive integer", nameof(value));
    }
    
    public static RegisterId From(int value) => new(value);
} 