namespace CatalogService.Application.DTOs;

public record CountryPricingDto
{
    public Guid Id { get; init; }
    public string CountryCode { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public DateTime EffectiveDate { get; init; }
    public DateTime CreatedAt { get; init; }
}