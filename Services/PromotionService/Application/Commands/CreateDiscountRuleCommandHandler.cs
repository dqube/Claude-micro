using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using PromotionService.Application.DTOs;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;
using PromotionService.Domain.Exceptions;

namespace PromotionService.Application.Commands;

public class CreateDiscountRuleCommandHandler : ICommandHandler<CreateDiscountRuleCommand, DiscountRuleDto>
{
    private readonly IDiscountRuleRepository _ruleRepository;
    private readonly IDiscountCampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDiscountRuleCommandHandler(
        IDiscountRuleRepository ruleRepository,
        IDiscountCampaignRepository campaignRepository,
        IUnitOfWork unitOfWork)
    {
        _ruleRepository = ruleRepository;
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DiscountRuleDto> HandleAsync(CreateDiscountRuleCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Verify campaign exists
        var campaignId = CampaignId.From(request.CampaignId);
        var campaign = await _campaignRepository.GetByIdAsync(campaignId, cancellationToken)
            ?? throw new DiscountCampaignNotFoundException(campaignId);

        // At this point, FluentValidation has already validated the request
        // So we can safely create value objects and entity
        var ruleType = RuleType.From(request.RuleType);
        var discountMethod = DiscountMethod.From(request.DiscountMethod);

        var rule = new DiscountRule(
            RuleId.New(),
            campaignId,
            ruleType,
            request.ProductId,
            request.CategoryId,
            request.MinQuantity,
            request.MinAmount,
            request.DiscountValue,
            discountMethod,
            request.FreeProductId);

        // Add to repository
        await _ruleRepository.AddAsync(rule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(rule);
    }

    private static DiscountRuleDto MapToDto(DiscountRule rule)
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