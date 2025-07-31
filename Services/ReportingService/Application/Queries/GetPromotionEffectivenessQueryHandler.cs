using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using ReportingService.Application.DTOs;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;

namespace ReportingService.Application.Queries;

public class GetPromotionEffectivenessQueryHandler : IQueryHandler<GetPromotionEffectivenessQuery, IEnumerable<PromotionEffectivenessDto>>
{
    private readonly IPromotionEffectivenessRepository _promotionEffectivenessRepository;

    public GetPromotionEffectivenessQueryHandler(IPromotionEffectivenessRepository promotionEffectivenessRepository)
    {
        _promotionEffectivenessRepository = promotionEffectivenessRepository;
    }

    public async Task<IEnumerable<PromotionEffectivenessDto>> HandleAsync(GetPromotionEffectivenessQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IEnumerable<PromotionEffectiveness> promotionEffectiveness;

        if (request.PromotionId.HasValue)
        {
            var promotionId = PromotionId.From(request.PromotionId.Value);
            var single = await _promotionEffectivenessRepository.GetByPromotionIdAsync(promotionId, cancellationToken);
            promotionEffectiveness = single != null ? [single] : [];
        }
        else if (request.TopCount.HasValue)
        {
            promotionEffectiveness = request.OrderBy?.ToLower() == "revenue"
                ? await _promotionEffectivenessRepository.GetTopByRevenueImpactAsync(request.TopCount.Value, cancellationToken)
                : await _promotionEffectivenessRepository.GetTopByRedemptionCountAsync(request.TopCount.Value, cancellationToken);
        }
        else if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            promotionEffectiveness = await _promotionEffectivenessRepository.GetByDateRangeAsync(
                request.StartDate.Value, request.EndDate.Value, cancellationToken);
        }
        else
        {
            promotionEffectiveness = await _promotionEffectivenessRepository.GetAllAsync(cancellationToken);
        }

        return promotionEffectiveness.Select(MapToDto);
    }

    private static PromotionEffectivenessDto MapToDto(PromotionEffectiveness promotionEffectiveness)
    {
        return new PromotionEffectivenessDto
        {
            Id = promotionEffectiveness.Id.Value,
            PromotionId = promotionEffectiveness.PromotionId.Value,
            RedemptionCount = promotionEffectiveness.RedemptionCount,
            RevenueImpact = promotionEffectiveness.RevenueImpact,
            AnalysisDate = promotionEffectiveness.AnalysisDate,
            CreatedAt = promotionEffectiveness.CreatedAt,
            CreatedBy = promotionEffectiveness.CreatedBy,
            ROI = promotionEffectiveness.CalculateROI(0) // ROI calculation would need promotion cost
        };
    }
} 