using BuildingBlocks.Domain.StronglyTypedIds;

namespace PaymentService.Domain.ValueObjects;

public class PaymentDetailId : StronglyTypedId<Guid>
{
    public PaymentDetailId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PaymentDetailId cannot be empty", nameof(value));
    }
    
    public static PaymentDetailId New() => new(Guid.NewGuid());
    
    public static PaymentDetailId From(Guid value) => new(value);
} 