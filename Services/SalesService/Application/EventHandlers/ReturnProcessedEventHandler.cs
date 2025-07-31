using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using SalesService.Domain.Events;

namespace SalesService.Application.EventHandlers;

public partial class ReturnProcessedEventHandler : IEventHandler<DomainEventWrapper<ReturnProcessedEvent>>
{
    private readonly ILogger<ReturnProcessedEventHandler> _logger;

    public ReturnProcessedEventHandler(ILogger<ReturnProcessedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ReturnProcessedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogReturnProcessed(_logger, domainEvent.ReturnId.Value, domainEvent.SaleId.Value, domainEvent.TotalRefund);

        // Additional business logic could be added here, such as:
        // - Trigger inventory restocking
        // - Update customer refund balance
        // - Generate refund receipt
        // - Update return metrics
        // - Integration events for payment service

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2006,
        Level = LogLevel.Information,
        Message = "Return processed: {returnId} for sale {saleId} - Refund: {totalRefund:C}")]
    private static partial void LogReturnProcessed(ILogger logger, Guid returnId, Guid saleId, decimal totalRefund);
}