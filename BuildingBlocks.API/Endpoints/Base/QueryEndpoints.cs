using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.API.Utilities.Helpers;
using BuildingBlocks.API.Validation.Results;

namespace BuildingBlocks.API.Endpoints.Base;

public abstract class QueryEndpoints : EndpointBase
{
    protected static RouteHandlerBuilder MapGetQuery<TRequest, TResponse>(
        IEndpointRouteBuilder app,
        string pattern,
        Func<TRequest, Task<TResponse>> handler)
    {
        return app.MapGet(pattern, async (HttpContext context, [AsParameters] TRequest request) =>
        {
            try
            {
                var validationResult = ValidateRequest(request);
                if (!validationResult.IsValid)
                {
                    return ResponseHelper.ValidationError(validationResult.ToDictionary());
                }

                var result = await handler(request);
                return ResponseHelper.Success(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        });
    }

    protected static RouteHandlerBuilder MapGetPagedQuery<TRequest, TResponse>(
        IEndpointRouteBuilder app,
        string pattern,
        Func<TRequest, Task<PagedResult<TResponse>>> handler)
    {
        return app.MapGet(pattern, async (HttpContext context, [AsParameters] TRequest request) =>
        {
            try
            {
                var validationResult = ValidateRequest(request);
                if (!validationResult.IsValid)
                {
                    return ResponseHelper.ValidationError(validationResult.ToDictionary());
                }

                var result = await handler(request);
                return ResponseHelper.PagedSuccess(
                    result.Data,
                    result.CurrentPage,
                    result.PageSize,
                    result.TotalCount);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        });
    }

    protected static RouteHandlerBuilder MapGetById<TId, TResponse>(
        IEndpointRouteBuilder app,
        string pattern,
        Func<TId, Task<TResponse?>> handler)
    {
        return app.MapGet(pattern, async (TId id) =>
        {
            try
            {
                if (id == null)
                {
                    return ResponseHelper.Error("ID is required", 400);
                }

                var result = await handler(id);
                if (result == null)
                {
                    return ResponseHelper.NotFound($"Resource with ID {id} not found");
                }

                return ResponseHelper.Success(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        });
    }

    protected static RouteHandlerBuilder MapGetList<TResponse>(
        IEndpointRouteBuilder app,
        string pattern,
        Func<Task<IEnumerable<TResponse>>> handler)
    {
        return app.MapGet(pattern, async () =>
        {
            try
            {
                var result = await handler();
                return ResponseHelper.Success(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        });
    }

    protected static RouteHandlerBuilder MapGetWithFilter<TFilter, TResponse>(
        IEndpointRouteBuilder app,
        string pattern,
        Func<TFilter, Task<IEnumerable<TResponse>>> handler)
    {
        return app.MapGet(pattern, async ([AsParameters] TFilter filter) =>
        {
            try
            {
                var validationResult = ValidateRequest(filter);
                if (!validationResult.IsValid)
                {
                    return ResponseHelper.ValidationError(validationResult.ToDictionary());
                }

                var result = await handler(filter);
                return ResponseHelper.Success(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        });
    }

    protected static RouteHandlerBuilder MapSearchQuery<TSearchRequest, TResponse>(
        IEndpointRouteBuilder app,
        string pattern,
        Func<TSearchRequest, Task<PagedResult<TResponse>>> handler)
    {
        return app.MapGet(pattern, async ([AsParameters] TSearchRequest searchRequest) =>
        {
            try
            {
                var validationResult = ValidateRequest(searchRequest);
                if (!validationResult.IsValid)
                {
                    return ResponseHelper.ValidationError(validationResult.ToDictionary());
                }

                var result = await handler(searchRequest);
                return ResponseHelper.PagedSuccess(
                    result.Data,
                    result.CurrentPage,
                    result.PageSize,
                    result.TotalCount,
                    "Search completed successfully");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        });
    }
}

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; init; } = Enumerable.Empty<T>();
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public long TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}