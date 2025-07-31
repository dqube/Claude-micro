using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using SalesService.Domain.Events;

namespace SalesService.Application.EventHandlers;

public partial class ReturnCreatedEventHandler : IEventHandler<DomainEventWrapper<ReturnCreatedEvent>>
{
    private readonly ILogger<ReturnCreatedEventHandler> _logger;

    public ReturnCreatedEventHandler(ILogger<ReturnCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ReturnCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogReturnCreated(_logger, domainEvent.ReturnId.Value, domainEvent.SaleId.Value, domainEvent.EmployeeId);

        // Additional business logic could be added here, such as:
        // - Return policy validation
        // - Customer return history tracking
        // - Return fraud detection
        // - Integration events for other services

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2005,
        Level = LogLevel.Information,
        Message = "Return created: {returnId} for sale {saleId} by employee {employeeId}")]
    private static partial void LogReturnCreated(ILogger logger, Guid returnId, Guid saleId, Guid employeeId);
}