using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using AuthService.Domain.Events;

namespace AuthService.Application.EventHandlers;

public partial class UserPasswordUpdatedEventHandler : IEventHandler<DomainEventWrapper<UserPasswordUpdatedEvent>>
{
    private readonly ILogger<UserPasswordUpdatedEventHandler> _logger;

    public UserPasswordUpdatedEventHandler(ILogger<UserPasswordUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<UserPasswordUpdatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogPasswordUpdated(_logger, domainEvent.UserId.Value);

        // Additional business logic could be added here, such as:
        // - Sending password change notification email
        // - Audit logging
        // - Invalidating existing sessions
        // - Security monitoring

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "User password updated: {userId}")]
    private static partial void LogPasswordUpdated(ILogger logger, Guid userId);
}