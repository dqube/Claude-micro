using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using SalesService.Domain.Events;

namespace SalesService.Application.EventHandlers;

public partial class ReturnDetailAddedEventHandler : IEventHandler<DomainEventWrapper<ReturnDetailAddedEvent>>
{
    private readonly ILogger<ReturnDetailAddedEventHandler> _logger;

    public ReturnDetailAddedEventHandler(ILogger<ReturnDetailAddedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ReturnDetailAddedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogReturnDetailAdded(_logger, domainEvent.ReturnId.Value, domainEvent.ProductId, domainEvent.Quantity, domainEvent.Reason.Name);

        // Additional business logic could be added here, such as:
        // - Product quality tracking
        // - Return reason analysis
        // - Inventory impact assessment
        // - Product defect notifications

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2007,
        Level = LogLevel.Information,
        Message = "Return detail added to return {returnId}: Product {productId} - Qty: {quantity} - Reason: {reason}")]
    private static partial void LogReturnDetailAdded(ILogger logger, Guid returnId, Guid productId, int quantity, string reason);
}