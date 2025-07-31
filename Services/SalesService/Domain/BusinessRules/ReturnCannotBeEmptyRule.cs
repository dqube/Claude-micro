using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class ReturnCannotBeEmptyRule : BusinessRuleBase
{
    private readonly int _returnDetailsCount;

    public ReturnCannotBeEmptyRule(int returnDetailsCount)
    {
        _returnDetailsCount = returnDetailsCount;
    }

    public override bool IsBroken() => _returnDetailsCount == 0;

    public override string Message => "Return must contain at least one item";
}