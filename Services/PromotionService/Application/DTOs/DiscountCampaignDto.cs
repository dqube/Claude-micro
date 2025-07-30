namespace PromotionService.Application.DTOs;

public record DiscountCampaignDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsActive { get; init; }
    public int? MaxUsesPerCustomer { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; init; }
    public List<DiscountRuleDto> Rules { get; init; } = new();
} 