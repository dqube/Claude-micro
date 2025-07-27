using BuildingBlocks.Application.CQRS.Events;
using ContactService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.EventHandlers;

public class ContactCreatedEventHandler : IEventHandler<DomainEventWrapper<ContactCreatedEvent>>
{
    private readonly ILogger<ContactCreatedEventHandler> _logger;

    public ContactCreatedEventHandler(ILogger<ContactCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<ContactCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation("Contact created: {ContactId} with email {Email}", 
            domainEvent.ContactId.Value, 
            domainEvent.Email.Value);

        await Task.CompletedTask;
    }
}