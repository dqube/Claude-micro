using BuildingBlocks.Domain.Common;

namespace ReportingService.Application.DTOs;

public record PromotionEffectivenessDto
{
    public Guid Id { get; init; }
    public Guid PromotionId { get; init; }
    public int RedemptionCount { get; init; }
    public decimal RevenueImpact { get; init; }
    public DateOnly AnalysisDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
    public decimal ROI { get; init; }
} 