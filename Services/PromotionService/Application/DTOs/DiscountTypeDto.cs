namespace PromotionService.Application.DTOs;

public record DiscountTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; init; }
} 