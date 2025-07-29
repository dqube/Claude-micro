using BuildingBlocks.Domain.BusinessRules;

namespace CatalogService.Domain.BusinessRules;

public class ProductMustHaveValidNameRule : BusinessRuleBase
{
    private readonly string _name;

    public ProductMustHaveValidNameRule(string name)
    {
        _name = name;
    }

    public override bool IsBroken()
    {
        return string.IsNullOrWhiteSpace(_name) || _name.Length < 2 || _name.Length > 200;
    }

    public override string Message => "Product name must be between 2 and 200 characters";
}