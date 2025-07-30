using BuildingBlocks.Domain.StronglyTypedIds;

namespace SupplierService.Domain.ValueObjects;

public class ContactId : StronglyTypedId<Guid>
{
    public ContactId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ContactId cannot be empty", nameof(value));
    }

    public static ContactId New() => new(Guid.NewGuid());
    public static ContactId From(Guid value) => new(value);
} 