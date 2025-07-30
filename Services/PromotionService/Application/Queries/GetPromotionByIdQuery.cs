using BuildingBlocks.Application.CQRS.Queries;
using PromotionService.Application.DTOs;

namespace PromotionService.Application.Queries;

public class GetPromotionByIdQuery : QueryBase<PromotionDto>
{
    public Guid PromotionId { get; init; }

    public GetPromotionByIdQuery(Guid promotionId)
    {
        PromotionId = promotionId;
    }
} 