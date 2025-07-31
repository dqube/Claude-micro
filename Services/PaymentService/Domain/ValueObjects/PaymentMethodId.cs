using BuildingBlocks.Domain.StronglyTypedIds;

namespace PaymentService.Domain.ValueObjects;

public class PaymentMethodId : StronglyTypedId<Guid>
{
    public PaymentMethodId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PaymentMethodId cannot be empty", nameof(value));
    }
    
    public static PaymentMethodId New() => new(Guid.NewGuid());
    
    public static PaymentMethodId From(Guid value) => new(value);
} 