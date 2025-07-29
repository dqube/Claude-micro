using BuildingBlocks.Application.CQRS.Queries;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Queries;

public class GetProductCategoriesQuery : QueryBase<List<ProductCategoryDto>>
{
    public bool IncludeHierarchy { get; init; }
    public int? ParentCategoryId { get; init; }

    public GetProductCategoriesQuery(bool includeHierarchy = false, int? parentCategoryId = null)
    {
        IncludeHierarchy = includeHierarchy;
        ParentCategoryId = parentCategoryId;
    }
}