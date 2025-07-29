using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductCreatedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public SKU SKU { get; }
    public string Name { get; }
    public string? Description { get; }
    public CategoryId CategoryId { get; }
    public Price BasePrice { get; }
    public Price CostPrice { get; }
    public bool IsTaxable { get; }
    public DateTime CreatedAt { get; }
    public int InitialBarcodeCount { get; }
    public int InitialCountryPricingCount { get; }

    public ProductCreatedEvent(
        ProductId productId, 
        SKU sku, 
        string name, 
        string? description,
        CategoryId categoryId, 
        Price basePrice,
        Price costPrice,
        bool isTaxable,
        DateTime createdAt,
        int initialBarcodeCount = 0,
        int initialCountryPricingCount = 0)
    {
        ProductId = productId;
        SKU = sku;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        BasePrice = basePrice;
        CostPrice = costPrice;
        IsTaxable = isTaxable;
        CreatedAt = createdAt;
        InitialBarcodeCount = initialBarcodeCount;
        InitialCountryPricingCount = initialCountryPricingCount;
    }
}