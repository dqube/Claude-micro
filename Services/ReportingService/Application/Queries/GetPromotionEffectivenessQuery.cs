using BuildingBlocks.Application.CQRS.Queries;
using ReportingService.Application.DTOs;

namespace ReportingService.Application.Queries;

public class GetPromotionEffectivenessQuery : QueryBase<IEnumerable<PromotionEffectivenessDto>>
{
    public Guid? PromotionId { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public int? TopCount { get; init; }
    public string? OrderBy { get; init; } // "redemptions" or "revenue"

    public GetPromotionEffectivenessQuery(
        Guid? promotionId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        int? topCount = null,
        string? orderBy = null)
    {
        PromotionId = promotionId;
        StartDate = startDate;
        EndDate = endDate;
        TopCount = topCount;
        OrderBy = orderBy;
    }
} 