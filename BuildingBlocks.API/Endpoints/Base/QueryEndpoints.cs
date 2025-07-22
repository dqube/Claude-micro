using Microsoft.AspNetCore.Http;
using BuildingBlocks.API.Responses.Base;

namespace BuildingBlocks.API.Endpoints.Base;

/// <summary>
/// Base class for query-only endpoints (GET operations)
/// </summary>
public abstract class QueryEndpoints : EndpointBase
{
    /// <summary>
    /// Returns a paged response for collections
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="data">The collection data</param>
    /// <param name="totalCount">Total number of items</param>
    /// <param name="pageNumber">Current page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="message">Optional response message</param>
    /// <param name="correlationId">Optional correlation ID</param>
    /// <returns>Paged API response</returns>
    protected static IResult PagedResponse<T>(
        IEnumerable<T> data,
        int totalCount,
        int pageNumber,
        int pageSize,
        string? message = null,
        string? correlationId = null)
    {
        var response = new PagedResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Query completed successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasNextPage = pageNumber * pageSize < totalCount,
            HasPreviousPage = pageNumber > 1
        };

        return Results.Ok(response);
    }

    /// <summary>
    /// Returns an empty paged response when no data is found
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="pageNumber">Current page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="message">Optional response message</param>
    /// <param name="correlationId">Optional correlation ID</param>
    /// <returns>Empty paged API response</returns>
    protected static IResult EmptyPagedResponse<T>(
        int pageNumber,
        int pageSize,
        string? message = null,
        string? correlationId = null)
    {
        return PagedResponse(
            Enumerable.Empty<T>(),
            0,
            pageNumber,
            pageSize,
            message ?? "No data found",
            correlationId);
    }

    /// <summary>
    /// Returns a single item response or NotFound if item is null
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    /// <param name="item">The item to return</param>
    /// <param name="notFoundMessage">Message to return if item is null</param>
    /// <param name="successMessage">Message to return if item is found</param>
    /// <param name="correlationId">Optional correlation ID</param>
    /// <returns>API response with item or NotFound</returns>
    protected static IResult SingleItemResponse<T>(
        T? item,
        string? notFoundMessage = null,
        string? successMessage = null,
        string? correlationId = null)
    {
        if (item == null)
        {
            return NotFound(notFoundMessage ?? "Item not found", correlationId);
        }

        return ApiResponse(item, successMessage ?? "Item retrieved successfully", correlationId);
    }

    /// <summary>
    /// Returns a collection response
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="data">The collection data</param>
    /// <param name="message">Optional response message</param>
    /// <param name="correlationId">Optional correlation ID</param>
    /// <returns>Collection API response</returns>
    protected static IResult CollectionResponse<T>(
        IEnumerable<T> data,
        string? message = null,
        string? correlationId = null)
    {
        var items = data?.ToList() ?? new List<T>();
        return ApiResponse(items, message ?? $"Retrieved {items.Count} items", correlationId);
    }
}