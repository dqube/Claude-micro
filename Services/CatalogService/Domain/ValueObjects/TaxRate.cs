using BuildingBlocks.Domain.ValueObjects;

namespace CatalogService.Domain.ValueObjects;

public class TaxRate : SingleValueObject<decimal>
{
    public TaxRate(decimal value) : base(value)
    {
        if (value < 0)
            throw new ArgumentException("Tax rate cannot be negative", nameof(value));
        
        if (value > 100)
            throw new ArgumentException("Tax rate cannot exceed 100%", nameof(value));
    }
    
    public static TaxRate From(decimal value) => new(value);
    public static TaxRate Zero => new(0);
    
    public decimal AsDecimal => Value / 100;
}