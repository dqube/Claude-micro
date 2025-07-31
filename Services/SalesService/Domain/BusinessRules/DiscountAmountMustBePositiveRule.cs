using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class DiscountAmountMustBePositiveRule : BusinessRuleBase
{
    private readonly decimal _discountAmount;

    public DiscountAmountMustBePositiveRule(decimal discountAmount)
    {
        _discountAmount = discountAmount;
    }

    public override bool IsBroken() => _discountAmount < 0;

    public override string Message => "Discount amount cannot be negative";
}