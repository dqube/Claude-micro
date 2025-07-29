using BuildingBlocks.Application.CQRS.Commands;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Commands;

public class CreateProductCommand : CommandBase<ProductDto>
{
    public string SKU { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int CategoryId { get; init; }
    public decimal BasePrice { get; init; }
    public decimal CostPrice { get; init; }
    public bool IsTaxable { get; init; } = true;

    public CreateProductCommand(
        string sku,
        string name,
        int categoryId,
        decimal basePrice,
        decimal costPrice,
        string? description = null,
        bool isTaxable = true)
    {
        SKU = sku;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        BasePrice = basePrice;
        CostPrice = costPrice;
        IsTaxable = isTaxable;
    }
}