using BuildingBlocks.Domain.ValueObjects;

namespace CatalogService.Domain.ValueObjects;

public class SKU : SingleValueObject<string>
{
    public SKU(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU cannot be null or empty", nameof(value));
        
        if (value.Length > 50)
            throw new ArgumentException("SKU cannot exceed 50 characters", nameof(value));
    }
    
    public static SKU From(string value) => new(value.Trim());
}