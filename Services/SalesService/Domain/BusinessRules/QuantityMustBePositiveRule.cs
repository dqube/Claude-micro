using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class QuantityMustBePositiveRule : BusinessRuleBase
{
    private readonly int _quantity;

    public QuantityMustBePositiveRule(int quantity)
    {
        _quantity = quantity;
    }

    public override bool IsBroken() => _quantity <= 0;

    public override string Message => "Quantity must be greater than zero";
}