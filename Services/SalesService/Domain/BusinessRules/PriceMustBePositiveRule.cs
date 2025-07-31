using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class PriceMustBePositiveRule : BusinessRuleBase
{
    private readonly decimal _price;

    public PriceMustBePositiveRule(decimal price)
    {
        _price = price;
    }

    public override bool IsBroken() => _price <= 0;

    public override string Message => "Price must be greater than zero";
}