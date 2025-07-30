using BuildingBlocks.Application.CQRS.Events;
using Microsoft.Extensions.Logging;
using PromotionService.Domain.Events;

namespace PromotionService.Application.EventHandlers;

public partial class DiscountCampaignCreatedEventHandler : IEventHandler<DomainEventWrapper<DiscountCampaignCreatedEvent>>
{
    private readonly ILogger<DiscountCampaignCreatedEventHandler> _logger;

    public DiscountCampaignCreatedEventHandler(ILogger<DiscountCampaignCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DomainEventWrapper<DiscountCampaignCreatedEvent> eventWrapper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventWrapper);

        var domainEvent = eventWrapper.DomainEvent;
        
        LogDiscountCampaignCreated(_logger, domainEvent.CampaignId.Value, domainEvent.Name, domainEvent.StartDate, domainEvent.EndDate);

        // Additional business logic could be added here, such as:
        // - Sending campaign notification emails
        // - Creating campaign analytics tracking
        // - Audit logging
        // - Integration events

        await Task.CompletedTask;
    }

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Discount campaign created: {campaignId} - {name} ({startDate} to {endDate})")]
    private static partial void LogDiscountCampaignCreated(ILogger logger, Guid campaignId, string name, DateTime startDate, DateTime endDate);
} 