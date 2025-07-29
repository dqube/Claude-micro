using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductCategoryNameUpdatedEvent : DomainEventBase
{
    public CategoryId CategoryId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public ProductCategoryNameUpdatedEvent(CategoryId categoryId, string oldName, string newName)
    {
        CategoryId = categoryId;
        OldName = oldName;
        NewName = newName;
    }
}