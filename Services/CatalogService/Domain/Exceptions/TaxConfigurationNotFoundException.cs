using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Exceptions;

public class TaxConfigurationNotFoundException : Exception
{
    public TaxConfigurationNotFoundException(TaxConfigId taxConfigId) 
        : base($"Tax configuration with ID '{taxConfigId.Value}' was not found")
    {
    }

    public TaxConfigurationNotFoundException(int locationId, CategoryId? categoryId) 
        : base($"Tax configuration for location '{locationId}' and category '{categoryId?.Value}' was not found")
    {
    }
}