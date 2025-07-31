#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using ReportingService.Application.Commands;
using ReportingService.Application.DTOs;
using ReportingService.Application.Queries;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Converters;
using System.Text.Json.Serialization;

namespace ReportingService.API.Endpoints;

internal static class InventorySnapshotEndpoints
{
    public static IEndpointRouteBuilder MapInventorySnapshotEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/inventory-snapshots")
            .WithTags("Inventory Snapshots")
            .WithOpenApi();

        // GET /api/inventory-snapshots/product/{productId}
        group.MapGet("/product/{productId:guid}", GetInventorySnapshotsByProductAsync)
            .WithName("GetInventorySnapshotsByProduct")
            .WithSummary("Get inventory snapshots by product")
            .WithDescription("Retrieve inventory snapshots for a specific product with optional date filtering")
            .Produces<IEnumerable<InventorySnapshotDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/inventory-snapshots/store/{storeId}/low-stock
        group.MapGet("/store/{storeId:int}/low-stock", GetLowStockInventoryAsync)
            .WithName("GetLowStockInventory")
            .WithSummary("Get low stock inventory for store")
            .WithDescription("Retrieve products with low inventory levels for a specific store")
            .Produces<IEnumerable<InventorySnapshotDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/inventory-snapshots/latest/{productId}/{storeId}
        group.MapGet("/latest/{productId:guid}/{storeId:int}", GetLatestInventorySnapshotAsync)
            .WithName("GetLatestInventorySnapshot")
            .WithSummary("Get latest inventory snapshot")
            .WithDescription("Retrieve the most recent inventory snapshot for a product at a specific store")
            .Produces<InventorySnapshotDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/inventory-snapshots
        group.MapPost("/", CreateInventorySnapshotAsync)
            .WithName("CreateInventorySnapshot")
            .WithSummary("Create a new inventory snapshot")
            .WithDescription("Create a new inventory snapshot record")
            .Produces<InventorySnapshotDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetInventorySnapshotsByProductAsync(
        Guid productId,
        [FromQuery] int? storeId,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetInventorySnapshotsByProductQuery(productId, storeId, startDate, endDate);
        var result = await mediator.QueryAsync<GetInventorySnapshotsByProductQuery, IEnumerable<InventorySnapshotDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> GetLowStockInventoryAsync(
        int storeId,
        IMediator mediator,
        CancellationToken cancellationToken,
        [FromQuery] int threshold = 10)
    {
        var query = new GetLowStockInventoryQuery(storeId, threshold);
        var result = await mediator.QueryAsync<GetLowStockInventoryQuery, IEnumerable<InventorySnapshotDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> GetLatestInventorySnapshotAsync(
        Guid productId,
        int storeId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetLatestInventorySnapshotQuery(productId, storeId);
        var result = await mediator.QueryAsync<GetLatestInventorySnapshotQuery, InventorySnapshotDto?>(query, cancellationToken);
        
        return result is not null
            ? Results.Ok(result)
            : Results.Problem(
                detail: $"No inventory snapshot found for product '{productId}' at store '{storeId}'.",
                statusCode: StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateInventorySnapshotAsync(
        CreateInventorySnapshotRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateInventorySnapshotCommand(
            request.ProductId,
            request.StoreId,
            request.Quantity,
            request.SnapshotDate);
            
        var result = await mediator.SendAsync<CreateInventorySnapshotCommand, InventorySnapshotDto>(command, cancellationToken);
        
        return Results.Created($"/api/inventory-snapshots/{result.Id}", result);
    }
}

// Request DTOs
internal record CreateInventorySnapshotRequest(
    [property: JsonPropertyName("productId")] Guid ProductId,
    [property: JsonPropertyName("storeId")] int StoreId,
    [property: JsonPropertyName("quantity")] int Quantity,
    [property: JsonPropertyName("snapshotDate")] DateOnly? SnapshotDate = null);

// Additional query classes that might be needed
internal class GetLowStockInventoryQuery : BuildingBlocks.Application.CQRS.Queries.QueryBase<IEnumerable<InventorySnapshotDto>>
{
    public int StoreId { get; init; }
    public int Threshold { get; init; }
    
    public GetLowStockInventoryQuery(int storeId, int threshold)
    {
        StoreId = storeId;
        Threshold = threshold;
    }
}

internal class GetLatestInventorySnapshotQuery : BuildingBlocks.Application.CQRS.Queries.QueryBase<InventorySnapshotDto?>
{
    public Guid ProductId { get; init; }
    public int StoreId { get; init; }
    
    public GetLatestInventorySnapshotQuery(Guid productId, int storeId)
    {
        ProductId = productId;
        StoreId = storeId;
    }
} 