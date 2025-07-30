using BuildingBlocks.Application.CQRS.Queries;
using PromotionService.Application.DTOs;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Domain.Exceptions;

namespace PromotionService.Application.Queries;

public class GetDiscountCampaignByIdQueryHandler : IQueryHandler<GetDiscountCampaignByIdQuery, DiscountCampaignDto>
{
    private readonly IDiscountCampaignRepository _campaignRepository;
    private readonly IDiscountRuleRepository _ruleRepository;

    public GetDiscountCampaignByIdQueryHandler(
        IDiscountCampaignRepository campaignRepository,
        IDiscountRuleRepository ruleRepository)
    {
        _campaignRepository = campaignRepository;
        _ruleRepository = ruleRepository;
    }

    public async Task<DiscountCampaignDto> HandleAsync(GetDiscountCampaignByIdQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var campaignId = CampaignId.From(request.CampaignId);
        var campaign = await _campaignRepository.GetByIdAsync(campaignId, cancellationToken)
            ?? throw new DiscountCampaignNotFoundException(campaignId);

        // Get rules for this campaign
        var rules = await _ruleRepository.GetByCampaignIdAsync(campaignId, cancellationToken);
        var ruleDtos = rules.Select(MapRuleToDto).ToList();

        return MapToDto(campaign, ruleDtos);
    }

    private static DiscountCampaignDto MapToDto(DiscountCampaign campaign, List<DiscountRuleDto> rules)
    {
        return new DiscountCampaignDto
        {
            Id = campaign.Id.Value,
            Name = campaign.Name,
            Description = campaign.Description,
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            IsActive = campaign.IsActive,
            MaxUsesPerCustomer = campaign.MaxUsesPerCustomer,
            CreatedAt = campaign.CreatedAt,
            LastUpdatedAt = campaign.UpdatedAt,
            Rules = rules
        };
    }

    private static DiscountRuleDto MapRuleToDto(DiscountRule rule)
    {
        return new DiscountRuleDto
        {
            Id = rule.Id.Value,
            CampaignId = rule.CampaignId.Value,
            RuleType = rule.RuleType.Value,
            ProductId = rule.ProductId,
            CategoryId = rule.CategoryId,
            MinQuantity = rule.MinQuantity,
            MinAmount = rule.MinAmount,
            DiscountValue = rule.DiscountValue,
            DiscountMethod = rule.DiscountMethod.Value,
            FreeProductId = rule.FreeProductId,
            CreatedAt = rule.CreatedAt,
            LastUpdatedAt = rule.UpdatedAt
        };
    }
} 