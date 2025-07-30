using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Domain.Common;
using InventoryService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.EventHandlers;

public class LowStockDetectedEventHandler : IEventHandler<DomainEventWrapper<LowStockDetectedEvent>>
{
    private readonly ILogger<LowStockDetectedEventHandler> _logger;

    public LowStockDetectedEventHandler(ILogger<LowStockDetectedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<LowStockDetectedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        _logger.LogWarning(
            "Low stock detected for product {ProductId} in store {StoreId}. Current quantity: {CurrentQuantity}, Reorder level: {ReorderLevel}",
            domainEvent.ProductId,
            domainEvent.StoreId,
            domainEvent.CurrentQuantity,
            domainEvent.ReorderLevel);

        await Task.CompletedTask;
    }
}