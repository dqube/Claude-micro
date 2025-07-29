using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class ProductPriceMustBePositiveRule : BusinessRuleBase
{
    private readonly Price _price;
    private readonly string _priceType;

    public ProductPriceMustBePositiveRule(Price price, string priceType = "Price")
    {
        _price = price;
        _priceType = priceType;
    }

    public override bool IsBroken()
    {
        return _price == null || _price.Value < 0;
    }

    public override string Message => $"{_priceType} must be positive";
}