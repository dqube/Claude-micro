using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class RefundAmountMustBePositiveRule : BusinessRuleBase
{
    private readonly decimal _refundAmount;

    public RefundAmountMustBePositiveRule(decimal refundAmount)
    {
        _refundAmount = refundAmount;
    }

    public override bool IsBroken() => _refundAmount < 0;

    public override string Message => "Refund amount cannot be negative";
}