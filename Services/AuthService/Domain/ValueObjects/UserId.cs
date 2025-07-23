using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class UserId : StronglyTypedId<Guid>
{
    public UserId(Guid value) : base(value)
    {
    }

    public static UserId New() => new(Guid.NewGuid());
}
