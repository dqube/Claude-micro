using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class TaxRateMustBeValidRule : BusinessRuleBase
{
    private readonly TaxRate _taxRate;

    public TaxRateMustBeValidRule(TaxRate taxRate)
    {
        _taxRate = taxRate;
    }

    public override bool IsBroken()
    {
        return _taxRate == null || _taxRate.Value < 0 || _taxRate.Value > 100;
    }

    public override string Message => "Tax rate must be between 0 and 100 percent";
}