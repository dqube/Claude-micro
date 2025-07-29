using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(ProductId productId) 
        : base($"Product with ID '{productId.Value}' was not found")
    {
    }

    public ProductNotFoundException(SKU sku) 
        : base($"Product with SKU '{sku.Value}' was not found")
    {
    }

    public ProductNotFoundException(BarcodeValue barcodeValue) 
        : base($"Product with barcode '{barcodeValue.Value}' was not found")
    {
    }
}