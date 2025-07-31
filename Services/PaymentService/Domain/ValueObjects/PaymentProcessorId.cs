using BuildingBlocks.Domain.StronglyTypedIds;

namespace PaymentService.Domain.ValueObjects;

public class PaymentProcessorId : StronglyTypedId<Guid>
{
    public PaymentProcessorId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PaymentProcessorId cannot be empty", nameof(value));
    }
    
    public static PaymentProcessorId New() => new(Guid.NewGuid());
    
    public static PaymentProcessorId From(Guid value) => new(value);
} 