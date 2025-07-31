using BuildingBlocks.Domain.DomainEvents;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Events;

public class PromotionEffectivenessUpdatedEvent : DomainEventBase
{
    public PromotionEffectivenessId PromotionEffectivenessId { get; }
    public int OldRedemptionCount { get; }
    public int NewRedemptionCount { get; }
    public decimal OldRevenueImpact { get; }
    public decimal NewRevenueImpact { get; }

    public PromotionEffectivenessUpdatedEvent(
        PromotionEffectivenessId promotionEffectivenessId,
        int oldRedemptionCount,
        int newRedemptionCount,
        decimal oldRevenueImpact,
        decimal newRevenueImpact)
    {
        PromotionEffectivenessId = promotionEffectivenessId;
        OldRedemptionCount = oldRedemptionCount;
        NewRedemptionCount = newRedemptionCount;
        OldRevenueImpact = oldRevenueImpact;
        NewRevenueImpact = newRevenueImpact;
    }
} 