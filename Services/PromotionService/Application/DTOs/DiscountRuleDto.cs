namespace PromotionService.Application.DTOs;

public record DiscountRuleDto
{
    public Guid Id { get; init; }
    public Guid CampaignId { get; init; }
    public string RuleType { get; init; } = string.Empty;
    public Guid? ProductId { get; init; }
    public int? CategoryId { get; init; }
    public int? MinQuantity { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal DiscountValue { get; init; }
    public string DiscountMethod { get; init; } = string.Empty;
    public Guid? FreeProductId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; init; }
} 