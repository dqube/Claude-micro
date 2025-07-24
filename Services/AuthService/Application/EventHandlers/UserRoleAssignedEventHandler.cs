using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using AuthService.Domain.Events;

namespace AuthService.Application.EventHandlers;

public partial class UserRoleAssignedEventHandler : IEventHandler<DomainEventWrapper<UserRoleAssignedEvent>>
{
    private readonly ILogger<UserRoleAssignedEventHandler> _logger;

    public UserRoleAssignedEventHandler(ILogger<UserRoleAssignedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<UserRoleAssignedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogRoleAssigned(_logger, domainEvent.UserId.Value, domainEvent.RoleId.Value, domainEvent.AssignedBy?.Value);

        // Additional business logic could be added here, such as:
        // - Sending role assignment notification
        // - Updating user permissions cache
        // - Audit logging
        // - Integration events

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Information,
        Message = "Role assigned to user: {userId} - Role: {roleId} - Assigned by: {assignedBy}")]
    private static partial void LogRoleAssigned(ILogger logger, Guid userId, int roleId, Guid? assignedBy);
}