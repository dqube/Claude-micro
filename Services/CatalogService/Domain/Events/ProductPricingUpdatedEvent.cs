using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductPricingUpdatedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public Price OldBasePrice { get; }
    public Price NewBasePrice { get; }
    public Price OldCostPrice { get; }
    public Price NewCostPrice { get; }

    public ProductPricingUpdatedEvent(ProductId productId, Price oldBasePrice, Price newBasePrice, Price oldCostPrice, Price newCostPrice)
    {
        ProductId = productId;
        OldBasePrice = oldBasePrice;
        NewBasePrice = newBasePrice;
        OldCostPrice = oldCostPrice;
        NewCostPrice = newCostPrice;
    }
}