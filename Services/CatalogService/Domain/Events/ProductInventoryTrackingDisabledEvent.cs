using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductInventoryTrackingDisabledEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public SKU SKU { get; }
    public string Name { get; }
    public DateTime DisabledAt { get; }
    public int? FinalQuantity { get; }

    public ProductInventoryTrackingDisabledEvent(
        ProductId productId, 
        SKU sku, 
        string name,
        DateTime disabledAt,
        int? finalQuantity = null)
    {
        ProductId = productId;
        SKU = sku;
        Name = name;
        DisabledAt = disabledAt;
        FinalQuantity = finalQuantity;
    }
}