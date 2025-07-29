using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductCategoryParentUpdatedEvent : DomainEventBase
{
    public CategoryId CategoryId { get; }
    public CategoryId? OldParentId { get; }
    public CategoryId? NewParentId { get; }

    public ProductCategoryParentUpdatedEvent(CategoryId categoryId, CategoryId? oldParentId, CategoryId? newParentId)
    {
        CategoryId = categoryId;
        OldParentId = oldParentId;
        NewParentId = newParentId;
    }
}