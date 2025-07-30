using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Domain.Common;
using InventoryService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.EventHandlers;

public class InventoryItemCreatedEventHandler : IEventHandler<DomainEventWrapper<InventoryItemCreatedEvent>>
{
    private readonly ILogger<InventoryItemCreatedEventHandler> _logger;

    public InventoryItemCreatedEventHandler(ILogger<InventoryItemCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<InventoryItemCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        _logger.LogInformation(
            "Inventory item created: {InventoryItemId} for product {ProductId} in store {StoreId} with initial quantity {Quantity}",
            domainEvent.InventoryItemId,
            domainEvent.ProductId,
            domainEvent.StoreId,
            domainEvent.InitialQuantity);

        await Task.CompletedTask;
    }
}