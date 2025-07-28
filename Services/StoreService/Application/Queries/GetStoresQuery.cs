using BuildingBlocks.Application.CQRS.Queries;
using StoreService.Application.DTOs;

namespace StoreService.Application.Queries;

public class GetStoresQuery : QueryBase<PagedResult<StoreDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? Status { get; init; }
    public int? LocationId { get; init; }
    public bool? IsOperational { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;

    public GetStoresQuery(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        string? status,
        int? locationId,
        bool? isOperational,
        string sortBy,
        bool sortDescending)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        Status = status;
        LocationId = locationId;
        IsOperational = isOperational;
        SortBy = sortBy;
        SortDescending = sortDescending;
    }
} 