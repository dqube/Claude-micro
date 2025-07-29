using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductCategoryDeletedEvent : DomainEventBase
{
    public CategoryId CategoryId { get; }
    public string Name { get; }
    public CategoryId? ParentCategoryId { get; }
    public string? ParentCategoryName { get; }
    public DateTime DeletedAt { get; }
    public int ProductsMovedCount { get; }
    public CategoryId? ProductsMovedToCategoryId { get; }
    public int ChildCategoriesMovedCount { get; }

    public ProductCategoryDeletedEvent(
        CategoryId categoryId, 
        string name,
        CategoryId? parentCategoryId,
        string? parentCategoryName,
        DateTime deletedAt,
        int productsMovedCount = 0,
        CategoryId? productsMovedToCategoryId = null,
        int childCategoriesMovedCount = 0)
    {
        CategoryId = categoryId;
        Name = name;
        ParentCategoryId = parentCategoryId;
        ParentCategoryName = parentCategoryName;
        DeletedAt = deletedAt;
        ProductsMovedCount = productsMovedCount;
        ProductsMovedToCategoryId = productsMovedToCategoryId;
        ChildCategoriesMovedCount = childCategoriesMovedCount;
    }
}