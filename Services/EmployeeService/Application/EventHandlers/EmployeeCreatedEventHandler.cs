using BuildingBlocks.Application.CQRS.Events;
using EmployeeService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EmployeeService.Application.EventHandlers;

public partial class EmployeeCreatedEventHandler : IEventHandler<DomainEventWrapper<EmployeeCreatedEvent>>
{
    private readonly ILogger<EmployeeCreatedEventHandler> _logger;

    public EmployeeCreatedEventHandler(ILogger<EmployeeCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<EmployeeCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        var domainEvent = eventWrapper.DomainEvent;
        
        _logger.LogInformation("Employee created: {EmployeeId} with number {EmployeeNumber} for store {StoreId}",
            domainEvent.EmployeeId, domainEvent.EmployeeNumber, domainEvent.StoreId);

        await Task.CompletedTask;
    }
}