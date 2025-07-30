using BuildingBlocks.Application.CQRS.Queries;
using PromotionService.Application.DTOs;
using PromotionService.Domain.Entities;
using PromotionService.Domain.Repositories;

namespace PromotionService.Application.Queries;

public class GetAllDiscountTypesQueryHandler : IQueryHandler<GetAllDiscountTypesQuery, List<DiscountTypeDto>>
{
    private readonly IDiscountTypeRepository _discountTypeRepository;

    public GetAllDiscountTypesQueryHandler(IDiscountTypeRepository discountTypeRepository)
    {
        _discountTypeRepository = discountTypeRepository;
    }

    public async Task<List<DiscountTypeDto>> HandleAsync(GetAllDiscountTypesQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var discountTypes = await _discountTypeRepository.GetAllAsync(cancellationToken);

        return discountTypes.Select(MapToDto).ToList();
    }

    private static DiscountTypeDto MapToDto(DiscountType discountType)
    {
        return new DiscountTypeDto
        {
            Id = discountType.Id.Value,
            Name = discountType.Name,
            Description = discountType.Description,
            CreatedAt = discountType.CreatedAt,
            LastUpdatedAt = discountType.UpdatedAt
        };
    }
} 