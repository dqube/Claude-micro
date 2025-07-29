using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductReactivatedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public SKU SKU { get; }
    public string Name { get; }
    public CategoryId CategoryId { get; }
    public DateTime ReactivatedAt { get; }
    public string? Reason { get; }

    public ProductReactivatedEvent(
        ProductId productId, 
        SKU sku, 
        string name, 
        CategoryId categoryId,
        DateTime reactivatedAt,
        string? reason = null)
    {
        ProductId = productId;
        SKU = sku;
        Name = name;
        CategoryId = categoryId;
        ReactivatedAt = reactivatedAt;
        Reason = reason;
    }
}