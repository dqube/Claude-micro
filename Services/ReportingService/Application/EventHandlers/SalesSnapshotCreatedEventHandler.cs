using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using ReportingService.Domain.Events;

namespace ReportingService.Application.EventHandlers;

public partial class SalesSnapshotCreatedEventHandler : IEventHandler<DomainEventWrapper<SalesSnapshotCreatedEvent>>
{
    private readonly ILogger<SalesSnapshotCreatedEventHandler> _logger;

    public SalesSnapshotCreatedEventHandler(ILogger<SalesSnapshotCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<SalesSnapshotCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogSalesSnapshotCreated(_logger, 
            domainEvent.SalesSnapshotId.Value, 
            domainEvent.SaleId.Value, 
            domainEvent.StoreId.Value, 
            domainEvent.TotalAmount);

        // Additional business logic could be added here, such as:
        // - Updating analytics dashboards
        // - Triggering alerts for significant sales
        // - Integration events for other services
        // - Performance metrics calculation

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Sales snapshot created: {salesSnapshotId} for sale {saleId} at store {storeId} with amount {totalAmount:C}")]
    private static partial void LogSalesSnapshotCreated(ILogger logger, Guid salesSnapshotId, Guid saleId, int storeId, decimal totalAmount);
} 