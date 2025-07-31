using BuildingBlocks.Domain.ValueObjects;

namespace SalesService.Domain.ValueObjects;

public class ReceiptNumber : SingleValueObject<string>
{
    public ReceiptNumber(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Receipt number cannot be empty", nameof(value));
        
        if (value.Length > 20)
            throw new ArgumentException("Receipt number cannot exceed 20 characters", nameof(value));
    }
    
    public static ReceiptNumber From(string value) => new(value);
}