using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using SalesService.Domain.Events;

namespace SalesService.Application.EventHandlers;

public partial class SaleDetailAddedEventHandler : IEventHandler<DomainEventWrapper<SaleDetailAddedEvent>>
{
    private readonly ILogger<SaleDetailAddedEventHandler> _logger;

    public SaleDetailAddedEventHandler(ILogger<SaleDetailAddedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<SaleDetailAddedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogSaleDetailAdded(_logger, domainEvent.SaleId.Value, domainEvent.ProductId, domainEvent.Quantity, domainEvent.UnitPrice);

        // Additional business logic could be added here, such as:
        // - Product popularity tracking
        // - Inventory reservation
        // - Price validation against current catalog
        // - Cross-selling recommendations

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Information,
        Message = "Sale detail added to sale {saleId}: Product {productId} - Qty: {quantity} @ {unitPrice:C}")]
    private static partial void LogSaleDetailAdded(ILogger logger, Guid saleId, Guid productId, int quantity, decimal unitPrice);
}