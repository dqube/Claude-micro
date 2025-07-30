using BuildingBlocks.Application.CQRS.Mediator;
using InventoryService.Application.Commands;
using InventoryService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Endpoints;

internal static class StockMovementEndpoints
{
    public static IEndpointRouteBuilder MapStockMovementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-movements")
            .WithTags("Stock Movements")
            .WithOpenApi();

        group.MapPost("/", CreateStockMovement)
            .WithName("CreateStockMovement")
            .WithSummary("Create a new stock movement")
            .WithDescription("Record a new stock movement transaction for inventory tracking")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/", GetStockMovements)
            .WithName("GetStockMovements")
            .WithSummary("Get stock movements with optional filters")
            .WithDescription("Retrieve stock movement history with optional filtering by product, store, date range, and movement type")
            .Produces<IReadOnlyList<Application.DTOs.StockMovementDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> CreateStockMovement(
        [FromBody] CreateStockMovementRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateStockMovementCommand(
            request.ProductId,
            request.StoreId,
            request.QuantityChange,
            request.MovementType,
            request.EmployeeId,
            request.ReferenceId,
            request.CreatedBy);

        await mediator.SendAsync(command, cancellationToken);
        return Results.Created($"/api/stock-movements", new { message = "Stock movement created successfully" });
    }

    private static async Task<IResult> GetStockMovements(
        [FromQuery] Guid? productId,
        [FromQuery] int? storeId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? movementType,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetStockMovementsQuery(productId, storeId, startDate, endDate, movementType);
        var result = await mediator.QueryAsync<GetStockMovementsQuery, IReadOnlyList<Application.DTOs.StockMovementDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }
}

internal record CreateStockMovementRequest(
    Guid ProductId,
    int StoreId,
    int QuantityChange,
    string MovementType,
    Guid EmployeeId,
    Guid? ReferenceId = null,
    Guid? CreatedBy = null);