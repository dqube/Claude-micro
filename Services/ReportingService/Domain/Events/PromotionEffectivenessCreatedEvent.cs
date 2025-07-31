using BuildingBlocks.Domain.DomainEvents;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Events;

public class PromotionEffectivenessCreatedEvent : DomainEventBase
{
    public PromotionEffectivenessId PromotionEffectivenessId { get; }
    public PromotionId PromotionId { get; }
    public int RedemptionCount { get; }
    public decimal RevenueImpact { get; }
    public DateOnly AnalysisDate { get; }

    public PromotionEffectivenessCreatedEvent(
        PromotionEffectivenessId promotionEffectivenessId,
        PromotionId promotionId,
        int redemptionCount,
        decimal revenueImpact,
        DateOnly analysisDate)
    {
        PromotionEffectivenessId = promotionEffectivenessId;
        PromotionId = promotionId;
        RedemptionCount = redemptionCount;
        RevenueImpact = revenueImpact;
        AnalysisDate = analysisDate;
    }
} 