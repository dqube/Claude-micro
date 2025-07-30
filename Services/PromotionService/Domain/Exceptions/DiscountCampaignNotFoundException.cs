using BuildingBlocks.Domain.Exceptions;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Exceptions;

public class DiscountCampaignNotFoundException : AggregateNotFoundException
{
    public DiscountCampaignNotFoundException() : base("Discount campaign was not found")
    {
    }

    public DiscountCampaignNotFoundException(string message) : base(message)
    {
    }

    public DiscountCampaignNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DiscountCampaignNotFoundException(CampaignId campaignId) 
        : base($"Discount campaign with ID '{campaignId?.Value}' was not found")
    {
    }

    public DiscountCampaignNotFoundException(string name, bool isName) 
        : base($"Discount campaign with name '{name}' was not found")
    {
    }
} 