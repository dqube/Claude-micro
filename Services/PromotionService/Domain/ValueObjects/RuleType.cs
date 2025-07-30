using BuildingBlocks.Domain.ValueObjects;

namespace PromotionService.Domain.ValueObjects;

public class RuleType : SingleValueObject<string>
{
    public static readonly RuleType Category = new("Category");
    public static readonly RuleType Product = new("Product");
    public static readonly RuleType TotalAmount = new("TotalAmount");
    public static readonly RuleType BuyXGetY = new("BuyXGetY");

    public RuleType(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Rule type cannot be null or empty", nameof(value));
        
        if (!IsValidRuleType(value))
            throw new ArgumentException($"Invalid rule type: {value}", nameof(value));
    }
    
    public static RuleType From(string value) => new(value);
    
    private static bool IsValidRuleType(string value)
    {
        return value is "Category" or "Product" or "TotalAmount" or "BuyXGetY";
    }
} 