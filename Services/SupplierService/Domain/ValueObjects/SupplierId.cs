using BuildingBlocks.Domain.StronglyTypedIds;

namespace SupplierService.Domain.ValueObjects;

public class SupplierId : StronglyTypedId<Guid>
{
    public SupplierId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("SupplierId cannot be empty", nameof(value));
    }

    public static SupplierId New() => new(Guid.NewGuid());
    public static SupplierId From(Guid value) => new(value);
} 