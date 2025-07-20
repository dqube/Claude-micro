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
        var group = endpoints.MapGroup(pattern);

        group.MapGet("/", GetAllAsync<TEntity, TResponse>)
            .WithName($"Get{entityName}s")
            .WithSummary($"Get all {entityName.ToLower()}s");

        group.MapGet("/{id}", GetByIdAsync<TEntity, TId, TResponse>)
            .WithName($"Get{entityName}ById")
            .WithSummary($"Get {entityName.ToLower()} by ID");

        group.MapPost("/", CreateAsync<TEntity, TCreateRequest, TResponse>)
            .WithName($"Create{entityName}")
            .WithSummary($"Create new {entityName.ToLower()}");

        group.MapPut("/{id}", UpdateAsync<TEntity, TId, TUpdateRequest, TResponse>)
            .WithName($"Update{entityName}")
            .WithSummary($"Update {entityName.ToLower()}");

        group.MapDelete("/{id}", DeleteAsync<TEntity, TId>)
            .WithName($"Delete{entityName}")
            .WithSummary($"Delete {entityName.ToLower()}");

        return group;
    }

    private static async Task<IResult> GetAllAsync<TEntity, TResponse>(
        HttpContext context,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        where TEntity : class
    {
        try
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

            return Results.Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Internal Server Error");
        }
    }

    private static async Task<IResult> GetByIdAsync<TEntity, TId, TResponse>(
        TId id,
        HttpContext context)
        where TEntity : class
        where TId : notnull
    {
        try
        {
            var correlationId = context.TraceIdentifier;
            
            // Placeholder implementation - replace with actual service call
            // if (entity == null)
            // {
            //     return EndpointBase.NotFound($"Entity with ID {id} not found", correlationId);
            // }

            // For now, returning a placeholder
            TResponse? data = default;
            
            if (data == null)
            {
                return Results.NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = $"Entity with ID {id} not found",
                    CorrelationId = correlationId,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Results.Ok(new ApiResponse<TResponse>
            {
                Success = true,
                Data = data,
                Message = "Retrieved successfully",
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Internal Server Error");
        }
    }

    private static async Task<IResult> CreateAsync<TEntity, TCreateRequest, TResponse>(
        TCreateRequest request,
        HttpContext context)
        where TEntity : class
    {
        try
        {
            var correlationId = context.TraceIdentifier;
            
            // Placeholder implementation - replace with actual service call
            // Validate request
            // Create entity
            // Return response
            
            TResponse? data = default;
            
            return Results.Created($"/{typeof(TEntity).Name.ToLower()}s", new ApiResponse<TResponse>
            {
                Success = true,
                Data = data,
                Message = "Created successfully",
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Internal Server Error");
        }
    }

    private static async Task<IResult> UpdateAsync<TEntity, TId, TUpdateRequest, TResponse>(
        TId id,
        TUpdateRequest request,
        HttpContext context)
        where TEntity : class
        where TId : notnull
    {
        try
        {
            var correlationId = context.TraceIdentifier;
            
            // Placeholder implementation - replace with actual service call
            // Find entity
            // Update entity
            // Return response
            
            TResponse? data = default;
            
            return Results.Ok(new ApiResponse<TResponse>
            {
                Success = true,
                Data = data,
                Message = "Updated successfully",
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Internal Server Error");
        }
    }

    private static async Task<IResult> DeleteAsync<TEntity, TId>(
        TId id,
        HttpContext context)
        where TEntity : class
        where TId : notnull
    {
        try
        {
            var correlationId = context.TraceIdentifier;
            
            // Placeholder implementation - replace with actual service call
            // Find entity
            // Delete entity
            
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Internal Server Error");
        }
    }
}