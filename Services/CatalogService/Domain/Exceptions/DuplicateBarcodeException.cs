using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Exceptions;

public class DuplicateBarcodeException : Exception
{
    public DuplicateBarcodeException(BarcodeValue barcodeValue) 
        : base($"A product with barcode '{barcodeValue.Value}' already exists")
    {
    }
}