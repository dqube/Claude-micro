using BuildingBlocks.Application.CQRS.Commands;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Commands;

public class CreateProductCategoryCommand : CommandBase<ProductCategoryDto>
{
    public string Name { get; init; } = string.Empty;
    public int? ParentCategoryId { get; init; }

    public CreateProductCategoryCommand(string name, int? parentCategoryId = null)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
    }
}