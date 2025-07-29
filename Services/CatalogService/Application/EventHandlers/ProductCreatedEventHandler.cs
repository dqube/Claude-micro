using BuildingBlocks.Application.CQRS.Events;
using CatalogService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.EventHandlers;

public class ProductCreatedEventHandler : IEventHandler<DomainEventWrapper<ProductCreatedEvent>>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ProductCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        _logger.LogInformation(
            "Product created: {ProductId} with SKU {SKU}, Name '{Name}', Base Price {BasePrice}, Cost Price {CostPrice}, " +
            "Category {CategoryId}, Taxable: {IsTaxable}, Created at {CreatedAt}",
            domainEvent.ProductId.Value,
            domainEvent.SKU.Value,
            domainEvent.Name,
            domainEvent.BasePrice.Value,
            domainEvent.CostPrice.Value,
            domainEvent.CategoryId.Value,
            domainEvent.IsTaxable,
            domainEvent.CreatedAt);

        // Additional business logic could be added here, such as:
        // - Sending integration events to other bounded contexts
        // - Updating denormalized views or read models
        // - Triggering workflows or business processes
        // - Sending notifications to external systems
        // - Creating audit logs
        // - Initializing inventory records if inventory tracking is enabled
        // - Generating product codes or identifiers
        // - Triggering product validation workflows

        // Example: Could publish an integration event for inventory management
        // if (domainEvent.InitialBarcodeCount > 0)
        // {
        //     await _integrationEventPublisher.PublishAsync(new ProductWithBarcodesCreatedIntegrationEvent(...));
        // }

        await Task.CompletedTask;
    }
}