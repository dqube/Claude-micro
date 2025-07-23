using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using BuildingBlocks.API.Responses.Base;

namespace BuildingBlocks.API.Endpoints.Base;

public static class CrudEndpoints
{
    public static RouteGroupBuilder MapCrudEndpoints<TEntity, TId, TCreateRequest, TUpdateRequest, TResponse>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        string entityName)
        where TEntity : class
        where TId : notnull
    {
        ArgumentNullException.ThrowIfNull(entityName);
        var group = endpoints.MapGroup(pattern);

        group.MapGet("/", GetAllAsync<TEntity, TResponse>)
            .WithName($"Get{entityName}s")
            .WithSummary($"Get all {entityName.ToUpperInvariant()}s");

        group.MapGet("/{id}", GetByIdAsync<TEntity, TId, TResponse>)
            .WithName($"Get{entityName}ById")
            .WithSummary($"Get {entityName.ToUpperInvariant()} by ID");

        group.MapPost("/", CreateAsync<TEntity, TCreateRequest, TResponse>)
            .WithName($"Create{entityName}")
            .WithSummary($"Create new {entityName.ToUpperInvariant()}");

        group.MapPut("/{id}", UpdateAsync<TEntity, TId, TUpdateRequest, TResponse>)
            .WithName($"Update{entityName}")
            .WithSummary($"Update {entityName.ToUpperInvariant()}");

        group.MapDelete("/{id}", DeleteAsync<TEntity, TId>)
            .WithName($"Delete{entityName}")
            .WithSummary($"Delete {entityName.ToUpperInvariant()}");

        return group;
    }

    private static async Task<IResult> GetAllAsync<TEntity, TResponse>(
        HttpContext context,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        where TEntity : class
    {
        // This would use your custom mediator or service layer
        // For now, returning a placeholder response
        var correlationId = context.TraceIdentifier;
        // Placeholder implementation - replace with actual service call
        var data = new List<TResponse>();
        var totalCount = 0;
        var pagedResponse = new PagedResponse<TResponse>
        {
            Success = true,
            Data = data,
            Message = "Retrieved successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            Pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            }
        };
        await Task.CompletedTask;
        return Results.Ok(pagedResponse);
    }

    private static async Task<IResult> GetByIdAsync<TEntity, TId, TResponse>(
        TId id,
        HttpContext context)
        where TEntity : class
        where TId : notnull
    {
        var correlationId = context.TraceIdentifier;
        // Placeholder implementation - replace with actual service call
        TResponse? data = default;
        await Task.CompletedTask;
        return Results.Ok(new ApiResponse<TResponse>
        {
            Success = true,
            Data = data,
            Message = "Retrieved successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });
    }

    private static async Task<IResult> CreateAsync<TEntity, TCreateRequest, TResponse>(
        TCreateRequest request,
        HttpContext context)
        where TEntity : class
    {
        var correlationId = context.TraceIdentifier;
        // Placeholder implementation - replace with actual service call
        TResponse? data = default;
        await Task.CompletedTask;
        return Results.Created($"/{typeof(TEntity).Name.ToUpperInvariant()}s", new ApiResponse<TResponse>
        {
            Success = true,
            Data = data,
            Message = "Created successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });
    }

    private static async Task<IResult> UpdateAsync<TEntity, TId, TUpdateRequest, TResponse>(
        TId id,
        TUpdateRequest request,
        HttpContext context)
        where TEntity : class
        where TId : notnull
    {
        var correlationId = context.TraceIdentifier;
        // Placeholder implementation - replace with actual service call
        TResponse? data = default;
        await Task.CompletedTask;
        return Results.Ok(new ApiResponse<TResponse>
        {
            Success = true,
            Data = data,
            Message = "Updated successfully",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });
    }

    private static async Task<IResult> DeleteAsync<TEntity, TId>(
        TId id,
        HttpContext context)
        where TEntity : class
        where TId : notnull
    {
        var correlationId = context.TraceIdentifier;
        // Placeholder implementation - replace with actual service call
        await Task.CompletedTask;
        return Results.NoContent();
    }
}