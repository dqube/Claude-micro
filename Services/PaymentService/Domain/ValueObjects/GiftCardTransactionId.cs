using BuildingBlocks.Domain.StronglyTypedIds;

namespace PaymentService.Domain.ValueObjects;

public class GiftCardTransactionId : StronglyTypedId<Guid>
{
    public GiftCardTransactionId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("GiftCardTransactionId cannot be empty", nameof(value));
    }
    
    public static GiftCardTransactionId New() => new(Guid.NewGuid());
    
    public static GiftCardTransactionId From(Guid value) => new(value);
} 