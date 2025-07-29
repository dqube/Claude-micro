using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Common;
using CatalogService.Application.DTOs;

namespace CatalogService.Application.Queries;

public class GetProductsQuery : QueryBase<PagedResult<ProductDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public int? CategoryId { get; init; }
    public bool? IsTaxable { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;

    public GetProductsQuery(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        int? categoryId,
        bool? isTaxable,
        decimal? minPrice,
        decimal? maxPrice,
        string sortBy,
        bool sortDescending)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        CategoryId = categoryId;
        IsTaxable = isTaxable;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        SortBy = sortBy;
        SortDescending = sortDescending;
    }
}