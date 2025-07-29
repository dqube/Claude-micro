namespace CatalogService.Application.DTOs;

public record ProductBarcodeDto
{
    public Guid Id { get; init; }
    public string BarcodeValue { get; init; } = string.Empty;
    public string BarcodeType { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}