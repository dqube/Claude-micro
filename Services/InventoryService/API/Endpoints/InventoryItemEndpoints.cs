using BuildingBlocks.Application.CQRS.Mediator;
using InventoryService.Application.Commands;
using InventoryService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Endpoints;

internal static class InventoryItemEndpoints
{
    public static IEndpointRouteBuilder MapInventoryItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory-items")
            .WithTags("Inventory Items")
            .WithOpenApi();

        group.MapPost("/", CreateInventoryItem)
            .WithName("CreateInventoryItem")
            .WithSummary("Create a new inventory item")
            .WithDescription("Create a new inventory item for a specific store and product")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetInventoryItemById)
            .WithName("GetInventoryItemById")
            .WithSummary("Get inventory item by ID")
            .WithDescription("Retrieve a specific inventory item by its unique identifier")
            .Produces<Application.DTOs.InventoryItemDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/store/{storeId:int}", GetInventoryByStore)
            .WithName("GetInventoryByStore")
            .WithSummary("Get all inventory items for a store")
            .WithDescription("Retrieve all inventory items for a specific store with current stock levels")
            .Produces<IReadOnlyList<Application.DTOs.InventoryItemDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}/quantity", UpdateInventoryQuantity)
            .WithName("UpdateInventoryQuantity")
            .WithSummary("Update inventory item quantity")
            .WithDescription("Update the quantity of a specific inventory item")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> CreateInventoryItem(
        [FromBody] CreateInventoryItemRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateInventoryItemCommand(
            request.StoreId,
            request.ProductId,
            request.Quantity,
            request.ReorderLevel,
            request.CreatedBy);

        await mediator.SendAsync(command, cancellationToken);
        return Results.Created($"/api/inventory-items", new { message = "Inventory item created successfully" });
    }

    private static async Task<IResult> GetInventoryItemById(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetInventoryItemByIdQuery(id);
        var result = await mediator.QueryAsync<GetInventoryItemByIdQuery, Application.DTOs.InventoryItemDto?>(query, cancellationToken);
        
        return result != null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetInventoryByStore(
        int storeId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetInventoryByStoreQuery(storeId);
        var result = await mediator.QueryAsync<GetInventoryByStoreQuery, IReadOnlyList<Application.DTOs.InventoryItemDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateInventoryQuantity(
        Guid id,
        [FromBody] UpdateInventoryQuantityRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateInventoryQuantityCommand(id, request.NewQuantity, request.UpdatedBy);
        await mediator.SendAsync(command, cancellationToken);
        
        return Results.Ok(new { message = "Inventory quantity updated successfully" });
    }
}

internal record CreateInventoryItemRequest(
    int StoreId,
    Guid ProductId,
    int Quantity = 0,
    int ReorderLevel = 10,
    Guid? CreatedBy = null);

internal record UpdateInventoryQuantityRequest(
    int NewQuantity,
    Guid UpdatedBy);