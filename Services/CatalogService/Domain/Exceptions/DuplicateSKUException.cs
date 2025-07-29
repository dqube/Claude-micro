using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Exceptions;

public class DuplicateSKUException : Exception
{
    public DuplicateSKUException(SKU sku) 
        : base($"A product with SKU '{sku.Value}' already exists")
    {
    }
}