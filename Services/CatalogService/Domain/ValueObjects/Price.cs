using BuildingBlocks.Domain.ValueObjects;

namespace CatalogService.Domain.ValueObjects;

public class Price : SingleValueObject<decimal>
{
    public Price(decimal value) : base(value)
    {
        if (value < 0)
            throw new ArgumentException("Price cannot be negative", nameof(value));
    }
    
    public static Price From(decimal value) => new(value);
    public static Price Zero => new(0);
    
    public static Price operator +(Price left, Price right) => new(left.Value + right.Value);
    public static Price operator -(Price left, Price right) => new(left.Value - right.Value);
    public static Price operator *(Price left, decimal multiplier) => new(left.Value * multiplier);
}