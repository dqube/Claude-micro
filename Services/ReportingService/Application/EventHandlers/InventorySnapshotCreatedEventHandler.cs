using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using ReportingService.Domain.Events;

namespace ReportingService.Application.EventHandlers;

public partial class InventorySnapshotCreatedEventHandler : IEventHandler<DomainEventWrapper<InventorySnapshotCreatedEvent>>
{
    private readonly ILogger<InventorySnapshotCreatedEventHandler> _logger;

    public InventorySnapshotCreatedEventHandler(ILogger<InventorySnapshotCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<InventorySnapshotCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogInventorySnapshotCreated(_logger, 
            domainEvent.InventorySnapshotId.Value, 
            domainEvent.ProductId.Value, 
            domainEvent.StoreId.Value, 
            domainEvent.Quantity);

        // Additional business logic could be added here, such as:
        // - Low inventory alerts
        // - Reorder point triggers
        // - Inventory trend analysis
        // - Stock level notifications

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Inventory snapshot created: {inventorySnapshotId} for product {productId} at store {storeId} with quantity {quantity}")]
    private static partial void LogInventorySnapshotCreated(ILogger logger, Guid inventorySnapshotId, Guid productId, int storeId, int quantity);
} 