using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using SalesService.Domain.Events;

namespace SalesService.Application.EventHandlers;

public partial class SaleCreatedEventHandler : IEventHandler<DomainEventWrapper<SaleCreatedEvent>>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<SaleCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogSaleCreated(_logger, domainEvent.SaleId.Value, domainEvent.StoreId, domainEvent.EmployeeId);

        // Additional business logic could be added here, such as:
        // - Inventory adjustments notification
        // - Sales analytics updates
        // - Employee performance tracking
        // - Integration events for other services

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Sale created: {saleId} at store {storeId} by employee {employeeId}")]
    private static partial void LogSaleCreated(ILogger logger, Guid saleId, int storeId, Guid employeeId);
}