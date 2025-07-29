using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductDiscontinuedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public SKU SKU { get; }
    public string Name { get; }
    public CategoryId CategoryId { get; }
    public DateTime DiscontinuedAt { get; }
    public string? Reason { get; }

    public ProductDiscontinuedEvent(
        ProductId productId, 
        SKU sku, 
        string name, 
        CategoryId categoryId,
        DateTime discontinuedAt,
        string? reason = null)
    {
        ProductId = productId;
        SKU = sku;
        Name = name;
        CategoryId = categoryId;
        DiscontinuedAt = discontinuedAt;
        Reason = reason;
    }
}