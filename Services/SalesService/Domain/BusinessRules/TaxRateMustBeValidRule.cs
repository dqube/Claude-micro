using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class TaxRateMustBeValidRule : BusinessRuleBase
{
    private readonly decimal _taxAmount;

    public TaxRateMustBeValidRule(decimal taxAmount)
    {
        _taxAmount = taxAmount;
    }

    public override bool IsBroken() => _taxAmount < 0;

    public override string Message => "Tax amount cannot be negative";
}