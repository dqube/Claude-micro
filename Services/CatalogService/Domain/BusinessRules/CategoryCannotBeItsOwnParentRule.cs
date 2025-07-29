using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class CategoryCannotBeItsOwnParentRule : BusinessRuleBase
{
    private readonly CategoryId _categoryId;
    private readonly CategoryId? _parentCategoryId;

    public CategoryCannotBeItsOwnParentRule(CategoryId categoryId, CategoryId? parentCategoryId)
    {
        _categoryId = categoryId;
        _parentCategoryId = parentCategoryId;
    }

    public override bool IsBroken()
    {
        return _parentCategoryId != null && _categoryId.Equals(_parentCategoryId);
    }

    public override string Message => "Category cannot be its own parent";
}