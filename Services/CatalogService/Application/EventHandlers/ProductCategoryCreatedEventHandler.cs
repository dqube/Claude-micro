using BuildingBlocks.Application.CQRS.Events;
using CatalogService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.EventHandlers;

public class ProductCategoryCreatedEventHandler : IEventHandler<DomainEventWrapper<ProductCategoryCreatedEvent>>
{
    private readonly ILogger<ProductCategoryCreatedEventHandler> _logger;

    public ProductCategoryCreatedEventHandler(ILogger<ProductCategoryCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ProductCategoryCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        _logger.LogInformation(
            "Product category created: {CategoryId} '{Name}' (Description: '{Description}'), " +
            "Parent: {ParentCategoryId} '{ParentCategoryName}', Root Category: {IsRootCategory}, Created at {CreatedAt}",
            domainEvent.CategoryId.Value,
            domainEvent.Name,
            domainEvent.Description ?? "None",
            domainEvent.ParentCategoryId?.Value ?? "None",
            domainEvent.ParentCategoryName ?? "None",
            domainEvent.IsRootCategory,
            domainEvent.CreatedAt);

        // Business logic for category creation:
        // - Update navigation structures and hierarchies
        // - Create default tax configurations for the category
        // - Initialize category-specific business rules
        // - Update search indexes and filters
        // - Notify merchandising teams
        // - Create default product templates for the category
        // - Set up category-specific workflows
        // - Initialize analytics and reporting structures

        if (domainEvent.IsRootCategory)
        {
            _logger.LogInformation(
                "New root category created: {CategoryId} '{Name}' - consider setting up category-specific configurations",
                domainEvent.CategoryId.Value,
                domainEvent.Name);

            // Additional logic for root categories:
            // - Set up department-level permissions
            // - Initialize category managers
            // - Create category-specific reporting
        }

        // Example: Create integration events for other bounded contexts
        // await _integrationEventPublisher.PublishAsync(new CategoryCreatedIntegrationEvent(
        //     domainEvent.CategoryId.Value,
        //     domainEvent.Name,
        //     domainEvent.ParentCategoryId?.Value,
        //     domainEvent.IsRootCategory));

        await Task.CompletedTask;
    }
}