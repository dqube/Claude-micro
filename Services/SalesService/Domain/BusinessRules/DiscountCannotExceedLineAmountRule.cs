using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class DiscountCannotExceedLineAmountRule : BusinessRuleBase
{
    private readonly decimal _discountAmount;
    private readonly decimal _lineAmount;

    public DiscountCannotExceedLineAmountRule(decimal discountAmount, decimal lineAmount)
    {
        _discountAmount = discountAmount;
        _lineAmount = lineAmount;
    }

    public override bool IsBroken() => _discountAmount > _lineAmount;

    public override string Message => "Discount amount cannot exceed line amount";
}