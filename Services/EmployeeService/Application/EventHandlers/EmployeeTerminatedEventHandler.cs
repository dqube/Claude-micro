using BuildingBlocks.Application.CQRS.Events;
using EmployeeService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EmployeeService.Application.EventHandlers;

public partial class EmployeeTerminatedEventHandler : IEventHandler<DomainEventWrapper<EmployeeTerminatedEvent>>
{
    private readonly ILogger<EmployeeTerminatedEventHandler> _logger;

    public EmployeeTerminatedEventHandler(ILogger<EmployeeTerminatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<EmployeeTerminatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        var domainEvent = eventWrapper.DomainEvent;
        
        _logger.LogInformation("Employee terminated: {EmployeeId} on {TerminationDate}",
            domainEvent.EmployeeId, domainEvent.TerminationDate);

        await Task.CompletedTask;
    }
}