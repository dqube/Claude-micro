using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using PromotionService.Domain.Events;

namespace PromotionService.Application.EventHandlers;

public partial class PromotionCreatedEventHandler : IEventHandler<DomainEventWrapper<PromotionCreatedEvent>>
{
    private readonly ILogger<PromotionCreatedEventHandler> _logger;

    public PromotionCreatedEventHandler(ILogger<PromotionCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<PromotionCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogPromotionCreated(_logger, domainEvent.PromotionId.Value, domainEvent.Name, domainEvent.StartDate, domainEvent.EndDate);

        // Additional business logic could be added here, such as:
        // - Sending promotion notification emails
        // - Creating promotion analytics tracking
        // - Audit logging
        // - Integration events

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Promotion created: {promotionId} - {name} ({startDate} to {endDate})")]
    private static partial void LogPromotionCreated(ILogger logger, Guid promotionId, string name, DateTime startDate, DateTime endDate);
} 