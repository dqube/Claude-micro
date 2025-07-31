using BuildingBlocks.Domain.Entities;
using ReportingService.Domain.Events;
using ReportingService.Domain.ValueObjects;

namespace ReportingService.Domain.Entities;

public class PromotionEffectiveness : AggregateRoot<PromotionEffectivenessId>
{
    public PromotionId PromotionId { get; private set; }
    public int RedemptionCount { get; private set; }
    public decimal RevenueImpact { get; private set; }
    public DateOnly AnalysisDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    // Private constructor for EF Core
    private PromotionEffectiveness() : base(PromotionEffectivenessId.New())
    {
        PromotionId = PromotionId.New();
        RedemptionCount = 0;
        RevenueImpact = 0;
        AnalysisDate = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public PromotionEffectiveness(
        PromotionEffectivenessId id,
        PromotionId promotionId,
        int redemptionCount = 0,
        decimal revenueImpact = 0,
        DateOnly? analysisDate = null) : base(id)
    {
        PromotionId = promotionId ?? throw new ArgumentNullException(nameof(promotionId));
        RedemptionCount = redemptionCount >= 0 ? redemptionCount : throw new ArgumentException("Redemption count cannot be negative", nameof(redemptionCount));
        RevenueImpact = revenueImpact;
        AnalysisDate = analysisDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new PromotionEffectivenessCreatedEvent(Id, PromotionId, RedemptionCount, RevenueImpact, AnalysisDate));
    }

    public void UpdateMetrics(int newRedemptionCount, decimal newRevenueImpact)
    {
        if (newRedemptionCount < 0)
            throw new ArgumentException("Redemption count cannot be negative", nameof(newRedemptionCount));

        var oldRedemptionCount = RedemptionCount;
        var oldRevenueImpact = RevenueImpact;

        RedemptionCount = newRedemptionCount;
        RevenueImpact = newRevenueImpact;

        AddDomainEvent(new PromotionEffectivenessUpdatedEvent(Id, oldRedemptionCount, newRedemptionCount, oldRevenueImpact, newRevenueImpact));
    }

    public void IncrementRedemption()
    {
        RedemptionCount++;
        AddDomainEvent(new PromotionRedemptionIncrementedEvent(Id, RedemptionCount));
    }

    public decimal CalculateROI(decimal promotionCost)
    {
        if (promotionCost <= 0)
            return 0;

        return (RevenueImpact - promotionCost) / promotionCost * 100;
    }
} 