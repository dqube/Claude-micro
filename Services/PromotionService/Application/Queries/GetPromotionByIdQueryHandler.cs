using BuildingBlocks.Application.CQRS.Queries;
using PromotionService.Application.DTOs;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Domain.Exceptions;

namespace PromotionService.Application.Queries;

public class GetPromotionByIdQueryHandler : IQueryHandler<GetPromotionByIdQuery, PromotionDto>
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IPromotionProductRepository _promotionProductRepository;

    public GetPromotionByIdQueryHandler(
        IPromotionRepository promotionRepository,
        IPromotionProductRepository promotionProductRepository)
    {
        _promotionRepository = promotionRepository;
        _promotionProductRepository = promotionProductRepository;
    }

    public async Task<PromotionDto> HandleAsync(GetPromotionByIdQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var promotionId = PromotionId.From(request.PromotionId);
        var promotion = await _promotionRepository.GetByIdAsync(promotionId, cancellationToken)
            ?? throw new PromotionNotFoundException(promotionId);

        // Get promotion products for this promotion
        var promotionProducts = await _promotionProductRepository.GetByPromotionIdAsync(promotionId, cancellationToken);
        var promotionProductDtos = promotionProducts.Select(MapPromotionProductToDto).ToList();

        return MapToDto(promotion, promotionProductDtos);
    }

    private static PromotionDto MapToDto(Promotion promotion, List<PromotionProductDto> promotionProducts)
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
            PromotionProducts = promotionProducts
        };
    }

    private static PromotionProductDto MapPromotionProductToDto(PromotionProduct promotionProduct)
    {
        return new PromotionProductDto
        {
            Id = promotionProduct.Id.Value,
            PromotionId = promotionProduct.PromotionId.Value,
            ProductId = promotionProduct.ProductId,
            CategoryId = promotionProduct.CategoryId,
            MinQuantity = promotionProduct.MinQuantity,
            DiscountPercent = promotionProduct.DiscountPercent,
            BundlePrice = promotionProduct.BundlePrice,
            CreatedAt = promotionProduct.CreatedAt
        };
    }
} 