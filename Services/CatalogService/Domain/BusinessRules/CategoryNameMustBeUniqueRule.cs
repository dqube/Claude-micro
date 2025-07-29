using BuildingBlocks.Domain.BusinessRules;

namespace CatalogService.Domain.BusinessRules;

public class CategoryNameMustBeUniqueRule : BusinessRuleBase
{
    private readonly string _name;
    private readonly IEnumerable<string> _existingNames;

    public CategoryNameMustBeUniqueRule(string name, IEnumerable<string> existingNames)
    {
        _name = name;
        _existingNames = existingNames ?? Enumerable.Empty<string>();
    }

    public override bool IsBroken()
    {
        return _existingNames.Any(n => n.Equals(_name, StringComparison.OrdinalIgnoreCase));
    }

    public override string Message => $"Category name '{_name}' already exists";
}