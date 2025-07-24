using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using AuthService.Domain.Events;

namespace AuthService.Application.EventHandlers;

public partial class UserCreatedEventHandler : IEventHandler<DomainEventWrapper<UserCreatedEvent>>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<UserCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogUserCreated(_logger, domainEvent.UserId.Value, domainEvent.Username.Value, domainEvent.Email.Value);

        // Additional business logic could be added here, such as:
        // - Sending welcome email
        // - Creating user profile
        // - Audit logging
        // - Integration events

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "User created: {userId} - {username} ({email})")]
    private static partial void LogUserCreated(ILogger logger, Guid userId, string username, string email);
}