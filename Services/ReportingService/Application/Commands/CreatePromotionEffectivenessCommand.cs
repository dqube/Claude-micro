using BuildingBlocks.Application.CQRS.Commands;
using ReportingService.Application.DTOs;

namespace ReportingService.Application.Commands;

public class CreatePromotionEffectivenessCommand : CommandBase<PromotionEffectivenessDto>
{
    public Guid PromotionId { get; init; }
    public int RedemptionCount { get; init; }
    public decimal RevenueImpact { get; init; }
    public DateOnly? AnalysisDate { get; init; }

    public CreatePromotionEffectivenessCommand(
        Guid promotionId,
        int redemptionCount = 0,
        decimal revenueImpact = 0,
        DateOnly? analysisDate = null)
    {
        PromotionId = promotionId;
        RedemptionCount = redemptionCount;
        RevenueImpact = revenueImpact;
        AnalysisDate = analysisDate;
    }
} 