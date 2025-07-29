namespace CatalogService.Application.DTOs;

public record ProductDto
{
    public Guid Id { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public decimal BasePrice { get; init; }
    public decimal CostPrice { get; init; }
    public bool IsTaxable { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<ProductBarcodeDto> Barcodes { get; init; } = new();
    public List<CountryPricingDto> CountryPricing { get; init; } = new();
}