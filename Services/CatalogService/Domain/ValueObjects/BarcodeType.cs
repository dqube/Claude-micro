using BuildingBlocks.Domain.ValueObjects;

namespace CatalogService.Domain.ValueObjects;

public class BarcodeType : Enumeration
{
    public static readonly BarcodeType UpcA = new(1, "UPC-A");
    public static readonly BarcodeType UpcE = new(2, "UPC-E");
    public static readonly BarcodeType Ean13 = new(3, "EAN-13");
    public static readonly BarcodeType Ean8 = new(4, "EAN-8");
    public static readonly BarcodeType Code128 = new(5, "Code128");
    public static readonly BarcodeType Code39 = new(6, "Code39");
    
    public static readonly BarcodeType Default = UpcA;

    private BarcodeType(int id, string name) : base(id, name) { }

    public static BarcodeType FromName(string name)
    {
        var type = GetAll<BarcodeType>().SingleOrDefault(t => 
            string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        
        return type ?? throw new ArgumentException($"Invalid barcode type: {name}", nameof(name));
    }

    public static BarcodeType FromId(int id)
    {
        var type = GetAll<BarcodeType>().SingleOrDefault(t => t.Id == id);
        return type ?? throw new ArgumentException($"Invalid barcode type ID: {id}", nameof(id));
    }
}