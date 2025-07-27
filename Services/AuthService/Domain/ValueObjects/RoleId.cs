using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class RoleId : StronglyTypedId<int>
{
    public RoleId(int value) : base(value)
    {
        if (value <= 0)
            throw new ArgumentException("RoleId must be positive", nameof(value));
    }
    
    public static RoleId From(int value) => new(value);
}