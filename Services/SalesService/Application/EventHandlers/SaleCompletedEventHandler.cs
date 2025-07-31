using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using SalesService.Domain.Events;

namespace SalesService.Application.EventHandlers;

public partial class SaleCompletedEventHandler : IEventHandler<DomainEventWrapper<SaleCompletedEvent>>
{
    private readonly ILogger<SaleCompletedEventHandler> _logger;

    public SaleCompletedEventHandler(ILogger<SaleCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<SaleCompletedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogSaleCompleted(_logger, domainEvent.SaleId.Value, domainEvent.TotalAmount, domainEvent.TransactionTime);

        // Additional business logic could be added here, such as:
        // - Trigger inventory deduction
        // - Update sales metrics
        // - Send receipt to customer
        // - Update loyalty points
        // - Integration events for other services

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Sale completed: {saleId} - Total: {totalAmount:C} - Time: {transactionTime}")]
    private static partial void LogSaleCompleted(ILogger logger, Guid saleId, decimal totalAmount, DateTime transactionTime);
}