using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductCategoryMovedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public SKU SKU { get; }
    public string Name { get; }
    public CategoryId OldCategoryId { get; }
    public CategoryId NewCategoryId { get; }
    public DateTime MovedAt { get; }

    public ProductCategoryMovedEvent(
        ProductId productId, 
        SKU sku, 
        string name,
        CategoryId oldCategoryId,
        CategoryId newCategoryId,
        DateTime movedAt)
    {
        ProductId = productId;
        SKU = sku;
        Name = name;
        OldCategoryId = oldCategoryId;
        NewCategoryId = newCategoryId;
        MovedAt = movedAt;
    }
}