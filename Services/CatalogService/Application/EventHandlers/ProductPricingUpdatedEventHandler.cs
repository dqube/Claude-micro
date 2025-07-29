using BuildingBlocks.Application.CQRS.Events;
using CatalogService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.EventHandlers;

public class ProductPricingUpdatedEventHandler : IEventHandler<DomainEventWrapper<ProductPricingUpdatedEvent>>
{
    private readonly ILogger<ProductPricingUpdatedEventHandler> _logger;

    public ProductPricingUpdatedEventHandler(ILogger<ProductPricingUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ProductPricingUpdatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        var basePriceChanged = !domainEvent.OldBasePrice.Equals(domainEvent.NewBasePrice);
        var costPriceChanged = !domainEvent.OldCostPrice.Equals(domainEvent.NewCostPrice);

        _logger.LogInformation(
            "Product pricing updated for {ProductId}: " +
            "Base Price: {OldBasePrice} → {NewBasePrice} (Changed: {BasePriceChanged}), " +
            "Cost Price: {OldCostPrice} → {NewCostPrice} (Changed: {CostPriceChanged})",
            domainEvent.ProductId.Value,
            domainEvent.OldBasePrice.Value,
            domainEvent.NewBasePrice.Value,
            basePriceChanged,
            domainEvent.OldCostPrice.Value,
            domainEvent.NewCostPrice.Value,
            costPriceChanged);

        // Business logic for pricing updates:
        // - Recalculate profit margins and notify if below thresholds
        // - Update pricing in all sales channels and systems
        // - Trigger repricing workflows for related products
        // - Send notifications to pricing managers
        // - Update inventory valuations
        // - Log pricing history for audit purposes
        // - Trigger competitive pricing analysis
        // - Update financial projections and reports

        if (basePriceChanged)
        {
            var priceChangePercentage = CalculatePriceChangePercentage(
                domainEvent.OldBasePrice.Value, 
                domainEvent.NewBasePrice.Value);

            if (Math.Abs(priceChangePercentage) > 10) // Significant price change
            {
                _logger.LogWarning(
                    "Significant price change detected for Product {ProductId}: {PriceChangePercentage:F2}%",
                    domainEvent.ProductId.Value,
                    priceChangePercentage);

                // Could trigger additional workflows for significant price changes
                // - Management approval workflows
                // - Customer notifications
                // - Competitor price monitoring
            }
        }

        await Task.CompletedTask;
    }

    private static decimal CalculatePriceChangePercentage(decimal oldPrice, decimal newPrice)
    {
        if (oldPrice == 0) return 0;
        return ((newPrice - oldPrice) / oldPrice) * 100;
    }
}