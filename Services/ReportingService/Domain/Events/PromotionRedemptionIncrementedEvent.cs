using BuildingBlocks.Domain.DomainEvents;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Events;

public class PromotionRedemptionIncrementedEvent : DomainEventBase
{
    public PromotionEffectivenessId PromotionEffectivenessId { get; }
    public int NewRedemptionCount { get; }

    public PromotionRedemptionIncrementedEvent(
        PromotionEffectivenessId promotionEffectivenessId,
        int newRedemptionCount)
    {
        PromotionEffectivenessId = promotionEffectivenessId;
        NewRedemptionCount = newRedemptionCount;
    }
} 