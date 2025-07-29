using BuildingBlocks.Application.CQRS.Events;
using CatalogService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.EventHandlers;

public class ProductCategoryDeletedEventHandler : IEventHandler<DomainEventWrapper<ProductCategoryDeletedEvent>>
{
    private readonly ILogger<ProductCategoryDeletedEventHandler> _logger;

    public ProductCategoryDeletedEventHandler(ILogger<ProductCategoryDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ProductCategoryDeletedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        _logger.LogInformation(
            "Product category deleted: {CategoryId} '{Name}', Parent: {ParentCategoryId} '{ParentCategoryName}', " +
            "Deleted: {DeletedAt}, Products moved: {ProductsMovedCount} to category {ProductsMovedToCategoryId}, " +
            "Child categories moved: {ChildCategoriesMovedCount}",
            domainEvent.CategoryId.Value,
            domainEvent.Name,
            domainEvent.ParentCategoryId?.Value.ToString() ?? "None",
            domainEvent.ParentCategoryName ?? "None",
            domainEvent.DeletedAt,
            domainEvent.ProductsMovedCount,
            domainEvent.ProductsMovedToCategoryId?.Value.ToString() ?? "None",
            domainEvent.ChildCategoriesMovedCount);

        // Business logic for category deletion:
        // - Clean up navigation structures
        // - Archive category-specific configurations
        // - Remove category from search indexes
        // - Update analytics and reporting
        // - Clean up permissions and access controls
        // - Archive historical data
        // - Update audit trails
        // - Notify stakeholders of category restructuring

        if (domainEvent.ProductsMovedCount > 0)
        {
            _logger.LogInformation(
                "Category deletion resulted in {ProductsMovedCount} products being moved to category {TargetCategoryId}",
                domainEvent.ProductsMovedCount,
                domainEvent.ProductsMovedToCategoryId?.Value.ToString() ?? "Unknown");

            // Additional processing for moved products:
            // - Verify product configurations are still valid
            // - Update product search metadata
            // - Recalculate category metrics
        }

        if (domainEvent.ChildCategoriesMovedCount > 0)
        {
            _logger.LogInformation(
                "Category deletion resulted in {ChildCategoriesMovedCount} child categories being restructured",
                domainEvent.ChildCategoriesMovedCount);

            // Additional processing for restructured hierarchy:
            // - Update navigation paths
            // - Recalculate category hierarchies
            // - Update breadcrumb trails
        }

        // Example: Publish integration events
        // await _integrationEventPublisher.PublishAsync(new CategoryDeletedIntegrationEvent(
        //     domainEvent.CategoryId.Value,
        //     domainEvent.Name,
        //     domainEvent.ProductsMovedCount,
        //     domainEvent.ChildCategoriesMovedCount));

        await Task.CompletedTask;
    }
}