using BuildingBlocks.Domain.StronglyTypedIds;

namespace SupplierService.Domain.ValueObjects;

public class OrderId : StronglyTypedId<Guid>
{
    public OrderId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(value));
    }

    public static OrderId New() => new(Guid.NewGuid());
    public static OrderId From(Guid value) => new(value);
} 