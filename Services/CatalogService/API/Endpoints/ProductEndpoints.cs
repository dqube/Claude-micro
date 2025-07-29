#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using CatalogService.Application.Commands;
using CatalogService.Application.DTOs;
using CatalogService.Application.Queries;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Converters;
using System.Text.Json.Serialization;

namespace CatalogService.API.Endpoints;

internal static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products")
            .WithTags("Products")
            .WithOpenApi();

        // GET /api/products
        group.MapGet("/", GetProductsAsync)
            .WithName("GetProducts")
            .WithSummary("Get all products with optional filtering and pagination")
            .WithDescription("Retrieve a paginated list of products with optional search and filtering capabilities")
            .Produces<PagedResult<ProductDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/products/{id}
        group.MapGet("/{id:guid}", GetProductByIdAsync)
            .WithName("GetProductById")
            .WithSummary("Get product by ID")
            .WithDescription("Retrieve a specific product by their unique identifier")
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/products
        group.MapPost("/", CreateProductAsync)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription("Create a new product with the provided information")
            .Produces<ProductDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // PUT /api/products/{id}
        group.MapPut("/{id:guid}", UpdateProductAsync)
            .WithName("UpdateProduct")
            .WithSummary("Update product")
            .WithDescription("Update an existing product with new information")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/products/{id}/barcodes
        group.MapPost("/{id:guid}/barcodes", AddProductBarcodeAsync)
            .WithName("AddProductBarcode")
            .WithSummary("Add barcode to product")
            .WithDescription("Add a new barcode to an existing product")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetProductsAsync(
        [FromServices] IMediator mediator,
        [AsParameters] GetProductsRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetProductsQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.CategoryId,
                request.IsTaxable,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.SortDescending);

            var result = await mediator.QueryAsync<GetProductsQuery, PagedResult<ProductDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.PagedResult(result.Items, result.TotalCount, result.Page, result.PageSize, "Products retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve products: {ex.Message}", correlationId);
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

    private static async Task<IResult> GetProductByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetProductByIdQuery(id);
            var result = await mediator.QueryAsync<GetProductByIdQuery, ProductDto?>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return result != null
                ? ResponseFactory.Success(result, "Product retrieved successfully", correlationId)
                : ResponseFactory.NotFound("Product", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve product: {ex.Message}", correlationId);
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

    private static async Task<IResult> CreateProductAsync(
        [FromBody] CreateProductRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new CreateProductCommand(
                request.SKU, 
                request.Name, 
                request.CategoryId, 
                request.BasePrice, 
                request.CostPrice, 
                request.Description, 
                request.IsTaxable);
            var result = await mediator.SendAsync<CreateProductCommand, ProductDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/products/{result.Id}", "Product created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create product: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> UpdateProductAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new UpdateProductCommand(
                id, 
                request.Name, 
                request.CategoryId, 
                request.BasePrice, 
                request.CostPrice, 
                request.IsTaxable, 
                request.Description);
            await mediator.SendAsync(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.NoContent(correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to update product: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> AddProductBarcodeAsync(
        [FromRoute] Guid id,
        [FromBody] AddProductBarcodeRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new AddProductBarcodeCommand(id, request.BarcodeValue, request.BarcodeType);
            await mediator.SendAsync(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.NoContent(correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to add barcode: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }
}

// Request DTOs
internal sealed record GetProductsRequest(
    [property: JsonPropertyName("page")] int Page = 1,
    [property: JsonPropertyName("pageSize")] int PageSize = 10,
    [property: JsonPropertyName("searchTerm")] string? SearchTerm = null,
    [property: JsonPropertyName("categoryId")] int? CategoryId = null,
    [property: JsonPropertyName("isTaxable")] bool? IsTaxable = null,
    [property: JsonPropertyName("minPrice")] decimal? MinPrice = null,
    [property: JsonPropertyName("maxPrice")] decimal? MaxPrice = null,
    [property: JsonPropertyName("sortBy")] string SortBy = "CreatedAt",
    [property: JsonPropertyName("sortDescending")] bool SortDescending = true
);

internal sealed record CreateProductRequest(
    [property: JsonPropertyName("sku")] string SKU,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("categoryId")] int CategoryId,
    [property: JsonPropertyName("basePrice")] decimal BasePrice,
    [property: JsonPropertyName("costPrice")] decimal CostPrice,
    [property: JsonPropertyName("isTaxable")] bool IsTaxable
);

internal sealed record UpdateProductRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("categoryId")] int CategoryId,
    [property: JsonPropertyName("basePrice")] decimal BasePrice,
    [property: JsonPropertyName("costPrice")] decimal CostPrice,
    [property: JsonPropertyName("isTaxable")] bool IsTaxable
);

internal sealed record AddProductBarcodeRequest(
    [property: JsonPropertyName("barcodeValue")] string BarcodeValue,
    [property: JsonPropertyName("barcodeType")] string BarcodeType
);