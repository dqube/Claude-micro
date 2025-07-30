using BuildingBlocks.Application.CQRS.Queries;
using PromotionService.Application.DTOs;

namespace PromotionService.Application.Queries;

public class GetDiscountCampaignByIdQuery : QueryBase<DiscountCampaignDto>
{
    public Guid CampaignId { get; init; }

    public GetDiscountCampaignByIdQuery(Guid campaignId)
    {
        CampaignId = campaignId;
    }
} 