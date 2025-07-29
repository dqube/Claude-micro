namespace CatalogService.Application.DTOs;

public record TaxConfigurationDto
{
    public Guid Id { get; init; }
    public int LocationId { get; init; }
    public int? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public decimal TaxRate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}