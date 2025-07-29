using BuildingBlocks.Domain.ValueObjects;

namespace CatalogService.Domain.ValueObjects;

public class BarcodeValue : SingleValueObject<string>
{
    public BarcodeValue(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Barcode value cannot be null or empty", nameof(value));
        
        if (value.Length > 50)
            throw new ArgumentException("Barcode value cannot exceed 50 characters", nameof(value));
    }
    
    public static BarcodeValue From(string value) => new(value.Trim());
}