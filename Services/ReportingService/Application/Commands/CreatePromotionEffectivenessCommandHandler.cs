using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using ReportingService.Application.DTOs;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;

namespace ReportingService.Application.Commands;

public class CreatePromotionEffectivenessCommandHandler : ICommandHandler<CreatePromotionEffectivenessCommand, PromotionEffectivenessDto>
{
    private readonly IPromotionEffectivenessRepository _promotionEffectivenessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePromotionEffectivenessCommandHandler(
        IPromotionEffectivenessRepository promotionEffectivenessRepository,
        IUnitOfWork unitOfWork)
    {
        _promotionEffectivenessRepository = promotionEffectivenessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PromotionEffectivenessDto> HandleAsync(CreatePromotionEffectivenessCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Create value objects
        var promotionId = PromotionId.From(request.PromotionId);

        // Create entity
        var promotionEffectiveness = new PromotionEffectiveness(
            PromotionEffectivenessId.New(),
            promotionId,
            request.RedemptionCount,
            request.RevenueImpact,
            request.AnalysisDate);

        // Add to repository
        await _promotionEffectivenessRepository.AddAsync(promotionEffectiveness, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(promotionEffectiveness);
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
            ROI = 0 // Will be calculated by the domain entity
        };
    }
} 