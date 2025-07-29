using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class ProductMustHaveValidSKURule : BusinessRuleBase
{
    private readonly SKU _sku;

    public ProductMustHaveValidSKURule(SKU sku)
    {
        _sku = sku;
    }

    public override bool IsBroken()
    {
        return _sku == null || string.IsNullOrWhiteSpace(_sku.Value);
    }

    public override string Message => "Product must have a valid SKU";
}