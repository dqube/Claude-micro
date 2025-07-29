using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class ProductMustHaveValidCategoryRule : BusinessRuleBase
{
    private readonly CategoryId _categoryId;

    public ProductMustHaveValidCategoryRule(CategoryId categoryId)
    {
        _categoryId = categoryId;
    }

    public override bool IsBroken()
    {
        return _categoryId == null;
    }

    public override string Message => "Product must be assigned to a valid category";
}