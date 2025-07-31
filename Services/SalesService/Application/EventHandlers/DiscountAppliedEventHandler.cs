using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using SalesService.Domain.Events;

namespace SalesService.Application.EventHandlers;

public partial class DiscountAppliedEventHandler : IEventHandler<DomainEventWrapper<DiscountAppliedEvent>>
{
    private readonly ILogger<DiscountAppliedEventHandler> _logger;

    public DiscountAppliedEventHandler(ILogger<DiscountAppliedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<DiscountAppliedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogDiscountApplied(_logger, domainEvent.SaleId.Value, domainEvent.CampaignId, domainEvent.RuleId, domainEvent.DiscountAmount);

        // Additional business logic could be added here, such as:
        // - Discount usage tracking
        // - Employee discount authorization audit
        // - Promotional effectiveness analysis
        // - Fraud detection for excessive discounts

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2004,
        Level = LogLevel.Information,
        Message = "Discount applied to sale {saleId}: Campaign {campaignId} - Rule {ruleId} - Amount: {discountAmount:C}")]
    private static partial void LogDiscountApplied(ILogger logger, Guid saleId, Guid campaignId, Guid ruleId, decimal discountAmount);
}