using BuildingBlocks.Domain.StronglyTypedIds;

namespace SupplierService.Domain.ValueObjects;

public class OrderDetailId : StronglyTypedId<Guid>
{
    public OrderDetailId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderDetailId cannot be empty", nameof(value));
    }

    public static OrderDetailId New() => new(Guid.NewGuid());
    public static OrderDetailId From(Guid value) => new(value);
} 