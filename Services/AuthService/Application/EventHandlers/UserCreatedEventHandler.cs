using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using AuthService.Domain.Events;

namespace AuthService.Application.EventHandlers;

public partial class UserCreatedEventHandler : IEventHandler<DomainEventWrapper<UserCreatedEvent>>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(DomainEventWrapper<UserCreatedEvent> notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        var userCreatedEvent = notification.DomainEvent;
        
        LogUserCreated(
            _logger,
            userCreatedEvent.UserId,
            userCreatedEvent.Username,
            userCreatedEvent.Email);

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "User created: {userId} - {username} ({email})")]
    private static partial void LogUserCreated(ILogger logger, Guid userId, string username, string email);
}
