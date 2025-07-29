using BuildingBlocks.Application.CQRS.Commands;

namespace CatalogService.Application.Commands;

public class UpdateProductCommand : CommandBase
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int CategoryId { get; init; }
    public decimal BasePrice { get; init; }
    public decimal CostPrice { get; init; }
    public bool IsTaxable { get; init; }

    public UpdateProductCommand(
        Guid productId,
        string name,
        int categoryId,
        decimal basePrice,
        decimal costPrice,
        bool isTaxable,
        string? description = null)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        BasePrice = basePrice;
        CostPrice = costPrice;
        IsTaxable = isTaxable;
    }
}