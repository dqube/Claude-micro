using BuildingBlocks.Application.CQRS.Events;
using ContactService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.EventHandlers;

public class ContactInformationUpdatedEventHandler : IEventHandler<DomainEventWrapper<ContactInformationUpdatedEvent>>
{
    private readonly ILogger<ContactInformationUpdatedEventHandler> _logger;

    public ContactInformationUpdatedEventHandler(ILogger<ContactInformationUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ContactInformationUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation("Contact information updated: {ContactId} with email {Email}", 
            domainEvent.ContactId.Value, 
            domainEvent.Email.Value);

        await Task.CompletedTask;
    }
}