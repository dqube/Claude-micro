namespace PromotionService.Application.DTOs;

public record PromotionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsCombinable { get; init; }
    public int? MaxRedemptions { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; init; }
    public List<PromotionProductDto> PromotionProducts { get; init; } = new();
} 