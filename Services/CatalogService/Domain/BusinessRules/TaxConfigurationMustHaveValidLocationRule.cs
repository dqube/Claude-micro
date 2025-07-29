using BuildingBlocks.Domain.BusinessRules;

namespace CatalogService.Domain.BusinessRules;

public class TaxConfigurationMustHaveValidLocationRule : BusinessRuleBase
{
    private readonly int _locationId;

    public TaxConfigurationMustHaveValidLocationRule(int locationId)
    {
        _locationId = locationId;
    }

    public override bool IsBroken()
    {
        return _locationId <= 0;
    }

    public override string Message => "Tax configuration must have a valid location ID";
}