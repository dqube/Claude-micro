#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using CatalogService.Application.Commands;
using CatalogService.Application.DTOs;
using CatalogService.Application.Queries;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace CatalogService.API.Endpoints;

internal static class ProductCategoryEndpoints
{
    public static IEndpointRouteBuilder MapProductCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/categories")
            .WithTags("Product Categories")
            .WithOpenApi();

        // GET /api/categories
        group.MapGet("/", GetProductCategoriesAsync)
            .WithName("GetProductCategories")
            .WithSummary("Get all product categories")
            .WithDescription("Retrieve all product categories with optional hierarchy")
            .Produces<List<ProductCategoryDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /api/categories
        group.MapPost("/", CreateProductCategoryAsync)
            .WithName("CreateProductCategory")
            .WithSummary("Create a new product category")
            .WithDescription("Create a new product category with the provided information")
            .Produces<ProductCategoryDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetProductCategoriesAsync(
        [FromServices] IMediator mediator,
        [AsParameters] GetProductCategoriesRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetProductCategoriesQuery(request.IncludeHierarchy, request.ParentCategoryId);
            var result = await mediator.QueryAsync<GetProductCategoriesQuery, List<ProductCategoryDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.Success(result, "Product categories retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve product categories: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request cancelled: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request timeout: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> CreateProductCategoryAsync(
        [FromBody] CreateProductCategoryRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new CreateProductCategoryCommand(request.Name, request.ParentCategoryId);
            var result = await mediator.SendAsync<CreateProductCategoryCommand, ProductCategoryDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/categories/{result.Id}", "Product category created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create product category: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }
}

// Request DTOs
internal sealed record GetProductCategoriesRequest(
    [property: JsonPropertyName("includeHierarchy")] bool IncludeHierarchy = false,
    [property: JsonPropertyName("parentCategoryId")] int? ParentCategoryId = null
);

internal sealed record CreateProductCategoryRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("parentCategoryId")] int? ParentCategoryId
);