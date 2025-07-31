using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using ReportingService.Domain.Events;

namespace ReportingService.Application.EventHandlers;

public partial class PromotionEffectivenessCreatedEventHandler : IEventHandler<DomainEventWrapper<PromotionEffectivenessCreatedEvent>>
{
    private readonly ILogger<PromotionEffectivenessCreatedEventHandler> _logger;

    public PromotionEffectivenessCreatedEventHandler(ILogger<PromotionEffectivenessCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<PromotionEffectivenessCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogPromotionEffectivenessCreated(_logger, 
            domainEvent.PromotionEffectivenessId.Value, 
            domainEvent.PromotionId.Value, 
            domainEvent.RedemptionCount, 
            domainEvent.RevenueImpact);

        // Additional business logic could be added here, such as:
        // - Performance analysis triggers
        // - Marketing campaign adjustments
        // - ROI calculations
        // - Promotion optimization recommendations

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Information,
        Message = "Promotion effectiveness created: {promotionEffectivenessId} for promotion {promotionId} with {redemptionCount} redemptions and {revenueImpact:C} impact")]
    private static partial void LogPromotionEffectivenessCreated(ILogger logger, Guid promotionEffectivenessId, Guid promotionId, int redemptionCount, decimal revenueImpact);
} 