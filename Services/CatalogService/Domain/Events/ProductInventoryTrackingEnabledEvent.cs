using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductInventoryTrackingEnabledEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public SKU SKU { get; }
    public string Name { get; }
    public DateTime EnabledAt { get; }
    public int InitialQuantity { get; }

    public ProductInventoryTrackingEnabledEvent(
        ProductId productId, 
        SKU sku, 
        string name,
        DateTime enabledAt,
        int initialQuantity = 0)
    {
        ProductId = productId;
        SKU = sku;
        Name = name;
        EnabledAt = enabledAt;
        InitialQuantity = initialQuantity;
    }
}