using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using AuthService.Domain.Events;

namespace AuthService.Application.EventHandlers;

public partial class RegistrationTokenCreatedEventHandler : IEventHandler<DomainEventWrapper<RegistrationTokenCreatedEvent>>
{
    private readonly ILogger<RegistrationTokenCreatedEventHandler> _logger;

    public RegistrationTokenCreatedEventHandler(ILogger<RegistrationTokenCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<RegistrationTokenCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogTokenCreated(_logger, domainEvent.TokenId.Value, domainEvent.UserId.Value, domainEvent.TokenType.Value);

        // Additional business logic could be added here, such as:
        // - Sending email with token
        // - Scheduling token cleanup
        // - Audit logging
        // - Integration events

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "Registration token created: {tokenId} - UserId: {userId} - Type: {tokenType}")]
    private static partial void LogTokenCreated(ILogger logger, Guid tokenId, Guid userId, string tokenType);
}