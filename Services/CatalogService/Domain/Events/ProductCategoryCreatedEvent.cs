using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductCategoryCreatedEvent : DomainEventBase
{
    public CategoryId CategoryId { get; }
    public string Name { get; }
    public string? Description { get; }
    public CategoryId? ParentCategoryId { get; }
    public string? ParentCategoryName { get; }
    public DateTime CreatedAt { get; }
    public bool IsRootCategory { get; }

    public ProductCategoryCreatedEvent(
        CategoryId categoryId, 
        string name, 
        string? description,
        CategoryId? parentCategoryId,
        string? parentCategoryName,
        DateTime createdAt)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
        ParentCategoryName = parentCategoryName;
        CreatedAt = createdAt;
        IsRootCategory = parentCategoryId == null;
    }
}