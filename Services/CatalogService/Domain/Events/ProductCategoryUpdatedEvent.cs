using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductCategoryUpdatedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public CategoryId OldCategoryId { get; }
    public CategoryId NewCategoryId { get; }

    public ProductCategoryUpdatedEvent(ProductId productId, CategoryId oldCategoryId, CategoryId newCategoryId)
    {
        ProductId = productId;
        OldCategoryId = oldCategoryId;
        NewCategoryId = newCategoryId;
    }
}