using BuildingBlocks.Domain.StronglyTypedIds;

namespace AuthService.Domain.ValueObjects;

public class RegistrationTokenId : StronglyTypedId<Guid>
{
    public RegistrationTokenId(Guid value) : base(value)
    {
    }

    public static RegistrationTokenId New() => new(Guid.NewGuid());
}
