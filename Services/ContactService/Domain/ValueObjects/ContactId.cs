using BuildingBlocks.Domain.StronglyTypedIds;

namespace ContactService.Domain.ValueObjects;

public class ContactId : GuidId
{
    public ContactId(Guid value) : base(value)
    {
    }

    public static ContactId New() => new(Guid.NewGuid());
    public static ContactId From(Guid value) => new(value);
}