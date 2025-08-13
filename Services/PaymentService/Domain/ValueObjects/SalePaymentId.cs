using BuildingBlocks.Domain.StronglyTypedIds;

namespace PaymentService.Domain.ValueObjects;

public class SalePaymentId : StronglyTypedId<Guid>
{
    public SalePaymentId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("SalePaymentId cannot be empty", nameof(value));
    }
    
    public static SalePaymentId New() => new(Guid.NewGuid());
    
    public static SalePaymentId From(Guid value) => new(value);
} 