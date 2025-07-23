using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class RoleId : StronglyTypedId<int>
{
    public RoleId(int value) : base(value)
    {
    }
}
