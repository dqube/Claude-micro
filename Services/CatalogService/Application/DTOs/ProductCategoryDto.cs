namespace CatalogService.Application.DTOs;

public record ProductCategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int? ParentCategoryId { get; init; }
    public string? ParentCategoryName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<ProductCategoryDto> SubCategories { get; init; } = new();
    public int ProductCount { get; init; }
}