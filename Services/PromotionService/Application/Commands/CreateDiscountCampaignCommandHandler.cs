using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using PromotionService.Application.DTOs;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;
using PromotionService.Domain.Repositories;

namespace PromotionService.Application.Commands;

public class CreateDiscountCampaignCommandHandler : ICommandHandler<CreateDiscountCampaignCommand, DiscountCampaignDto>
{
    private readonly IDiscountCampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDiscountCampaignCommandHandler(
        IDiscountCampaignRepository campaignRepository,
        IUnitOfWork unitOfWork)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DiscountCampaignDto> HandleAsync(CreateDiscountCampaignCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // At this point, FluentValidation has already validated the request
        // So we can safely create the entity
        var campaign = new DiscountCampaign(
            CampaignId.New(),
            request.Name,
            request.StartDate,
            request.EndDate,
            request.Description,
            request.MaxUsesPerCustomer);

        // Add to repository
        await _campaignRepository.AddAsync(campaign, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(campaign);
    }

    private static DiscountCampaignDto MapToDto(DiscountCampaign campaign)
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
            Rules = new List<DiscountRuleDto>() // Will be populated when campaign has rules
        };
    }
} 