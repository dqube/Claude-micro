using BuildingBlocks.Application.CQRS.Queries;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Queries;

public class GetProductByIdQuery : QueryBase<ProductDto?>
{
    public Guid ProductId { get; init; }

    public GetProductByIdQuery(Guid productId)
    {
        ProductId = productId;
    }
}