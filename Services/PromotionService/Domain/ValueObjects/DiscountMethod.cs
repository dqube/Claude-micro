using BuildingBlocks.Domain.ValueObjects;

namespace PromotionService.Domain.ValueObjects;

public class DiscountMethod : SingleValueObject<string>
{
    public static readonly DiscountMethod Percent = new("Percent");
    public static readonly DiscountMethod Fixed = new("Fixed");
    public static readonly DiscountMethod FreeItem = new("FreeItem");

    public DiscountMethod(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Discount method cannot be null or empty", nameof(value));
        
        if (!IsValidDiscountMethod(value))
            throw new ArgumentException($"Invalid discount method: {value}", nameof(value));
    }
    
    public static DiscountMethod From(string value) => new(value);
    
    private static bool IsValidDiscountMethod(string value)
    {
        return value is "Percent" or "Fixed" or "FreeItem";
    }
} 