using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using PromotionService.Application.DTOs;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;

namespace PromotionService.Application.Commands;

public class CreatePromotionCommandHandler : ICommandHandler<CreatePromotionCommand, PromotionDto>
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePromotionCommandHandler(
        IPromotionRepository promotionRepository,
        IUnitOfWork unitOfWork)
    {
        _promotionRepository = promotionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PromotionDto> HandleAsync(CreatePromotionCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // At this point, FluentValidation has already validated the request
        // So we can safely create the entity
        var promotion = new Promotion(
            PromotionId.New(),
            request.Name,
            request.StartDate,
            request.EndDate,
            request.Description,
            request.IsCombinable,
            request.MaxRedemptions);

        // Add to repository
        await _promotionRepository.AddAsync(promotion, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(promotion);
    }

    private static PromotionDto MapToDto(Promotion promotion)
    {
        return new PromotionDto
        {
            Id = promotion.Id.Value,
            Name = promotion.Name,
            Description = promotion.Description,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            IsCombinable = promotion.IsCombinable,
            MaxRedemptions = promotion.MaxRedemptions,
            CreatedAt = promotion.CreatedAt,
            LastUpdatedAt = promotion.UpdatedAt,
            PromotionProducts = new List<PromotionProductDto>() // Will be populated when promotion has products
        };
    }
} 