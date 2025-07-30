using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using PromotionService.Domain.Events;

namespace PromotionService.Application.EventHandlers;

public partial class DiscountRuleCreatedEventHandler : IEventHandler<DomainEventWrapper<DiscountRuleCreatedEvent>>
{
    private readonly ILogger<DiscountRuleCreatedEventHandler> _logger;

    public DiscountRuleCreatedEventHandler(ILogger<DiscountRuleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<DiscountRuleCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogDiscountRuleCreated(_logger, domainEvent.RuleId.Value, domainEvent.CampaignId.Value, domainEvent.RuleType.Value, domainEvent.DiscountValue);

        // Additional business logic could be added here, such as:
        // - Updating campaign cache
        // - Recalculating discount matrices
        // - Audit logging
        // - Integration events

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Information,
        Message = "Discount rule created: {ruleId} for campaign {campaignId} - {ruleType} with discount {discountValue}")]
    private static partial void LogDiscountRuleCreated(ILogger logger, Guid ruleId, Guid campaignId, string ruleType, decimal discountValue);
} 