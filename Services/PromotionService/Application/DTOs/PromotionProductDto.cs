namespace PromotionService.Application.DTOs;

public record PromotionProductDto
{
    public Guid Id { get; init; }
    public Guid PromotionId { get; init; }
    public Guid? ProductId { get; init; }
    public int? CategoryId { get; init; }
    public int MinQuantity { get; init; }
    public decimal? DiscountPercent { get; init; }
    public decimal? BundlePrice { get; init; }
    public DateTime CreatedAt { get; init; }
} 