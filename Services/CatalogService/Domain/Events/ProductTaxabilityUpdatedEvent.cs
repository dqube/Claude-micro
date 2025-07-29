using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductTaxabilityUpdatedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public SKU SKU { get; }
    public string Name { get; }
    public bool IsTaxable { get; }

    public ProductTaxabilityUpdatedEvent(ProductId productId, SKU sku, string name, bool isTaxable)
    {
        ProductId = productId;
        SKU = sku;
        Name = name;
        IsTaxable = isTaxable;
    }
}