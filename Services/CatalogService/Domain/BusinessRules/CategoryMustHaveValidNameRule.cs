using BuildingBlocks.Domain.BusinessRules;

namespace CatalogService.Domain.BusinessRules;

public class CategoryMustHaveValidNameRule : BusinessRuleBase
{
    private readonly string _name;

    public CategoryMustHaveValidNameRule(string name)
    {
        _name = name;
    }

    public override bool IsBroken()
    {
        return string.IsNullOrWhiteSpace(_name) || _name.Length < 2 || _name.Length > 100;
    }

    public override string Message => "Category name must be between 2 and 100 characters";
}