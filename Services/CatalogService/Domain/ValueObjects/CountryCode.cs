using BuildingBlocks.Domain.ValueObjects;

namespace CatalogService.Domain.ValueObjects;

public class CountryCode : SingleValueObject<string>
{
    public CountryCode(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Country code cannot be null or empty", nameof(value));
        
        if (value.Length != 2)
            throw new ArgumentException("Country code must be exactly 2 characters", nameof(value));
    }
    
    public static CountryCode From(string value) => new(value.ToUpperInvariant());
}