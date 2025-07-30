using BuildingBlocks.Application.CQRS.Queries;
using PromotionService.Application.DTOs;

namespace PromotionService.Application.Queries;

public class GetAllDiscountTypesQuery : QueryBase<List<DiscountTypeDto>>
{
    public GetAllDiscountTypesQuery()
    {
    }
} 