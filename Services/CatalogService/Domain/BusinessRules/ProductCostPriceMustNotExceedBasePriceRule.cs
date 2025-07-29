using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class ProductCostPriceMustNotExceedBasePriceRule : BusinessRuleBase
{
    private readonly Price _costPrice;
    private readonly Price _basePrice;

    public ProductCostPriceMustNotExceedBasePriceRule(Price costPrice, Price basePrice)
    {
        _costPrice = costPrice;
        _basePrice = basePrice;
    }

    public override bool IsBroken()
    {
        return _costPrice != null && _basePrice != null && _costPrice.Value > _basePrice.Value;
    }

    public override string Message => "Product cost price cannot exceed base price";
}