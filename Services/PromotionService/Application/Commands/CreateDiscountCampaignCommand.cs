using BuildingBlocks.Application.CQRS.Commands;
using PromotionService.Application.DTOs;

namespace PromotionService.Application.Commands;

public class CreateDiscountCampaignCommand : CommandBase<DiscountCampaignDto>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsActive { get; init; } = true;
    public int? MaxUsesPerCustomer { get; init; }

    public CreateDiscountCampaignCommand(string name, string description, DateTime startDate, DateTime endDate, bool isActive = true, int? maxUsesPerCustomer = null)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
        MaxUsesPerCustomer = maxUsesPerCustomer;
    }
} 