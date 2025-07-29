using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class TaxConfigurationMustBeUniqueRule : BusinessRuleBase
{
    private readonly int _locationId;
    private readonly CategoryId? _categoryId;
    private readonly IEnumerable<(int LocationId, CategoryId? CategoryId)> _existingConfigurations;

    public TaxConfigurationMustBeUniqueRule(
        int locationId, 
        CategoryId? categoryId, 
        IEnumerable<(int LocationId, CategoryId? CategoryId)> existingConfigurations)
    {
        _locationId = locationId;
        _categoryId = categoryId;
        _existingConfigurations = existingConfigurations ?? Enumerable.Empty<(int, CategoryId?)>();
    }

    public override bool IsBroken()
    {
        return _existingConfigurations.Any(config => 
            config.LocationId == _locationId && 
            ((config.CategoryId == null && _categoryId == null) || 
             (config.CategoryId != null && _categoryId != null && config.CategoryId.Equals(_categoryId))));
    }

    public override string Message => $"Tax configuration for location {_locationId} and category {(_categoryId?.Value.ToString() ?? "null")} already exists";
}