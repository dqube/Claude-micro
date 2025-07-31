using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class SaleCannotBeEmptyRule : BusinessRuleBase
{
    private readonly int _saleDetailsCount;

    public SaleCannotBeEmptyRule(int saleDetailsCount)
    {
        _saleDetailsCount = saleDetailsCount;
    }

    public override bool IsBroken() => _saleDetailsCount == 0;

    public override string Message => "Sale must contain at least one item";
}