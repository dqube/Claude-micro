using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class UserId : StronglyTypedId<Guid>
{
    public UserId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(value));
    }
    
    public static UserId New() => new(Guid.NewGuid());
    
    public static UserId From(Guid value) => new(value);
}