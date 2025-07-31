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

internal static class SalesSnapshotEndpoints
{
    public static IEndpointRouteBuilder MapSalesSnapshotEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/sales-snapshots")
            .WithTags("Sales Snapshots")
            .WithOpenApi();

        // GET /api/sales-snapshots/{id}
        group.MapGet("/{id:guid}", GetSalesSnapshotByIdAsync)
            .WithName("GetSalesSnapshotById")
            .WithSummary("Get sales snapshot by ID")
            .WithDescription("Retrieve a specific sales snapshot by its unique identifier")
            .Produces<SalesSnapshotDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // GET /api/sales-snapshots/store/{storeId}
        group.MapGet("/store/{storeId:int}", GetSalesSnapshotsByStoreAsync)
            .WithName("GetSalesSnapshotsByStore")
            .WithSummary("Get sales snapshots by store")
            .WithDescription("Retrieve sales snapshots for a specific store with optional date filtering")
            .Produces<IEnumerable<SalesSnapshotDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /api/sales-snapshots
        group.MapPost("/", CreateSalesSnapshotAsync)
            .WithName("CreateSalesSnapshot")
            .WithSummary("Create a new sales snapshot")
            .WithDescription("Create a new sales snapshot record")
            .Produces<SalesSnapshotDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetSalesSnapshotByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetSalesSnapshotByIdQuery(id);
        var result = await mediator.QueryAsync<GetSalesSnapshotByIdQuery, SalesSnapshotDto?>(query, cancellationToken);
        
        return result is not null
            ? Results.Ok(result)
            : Results.Problem(
                detail: $"Sales snapshot with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetSalesSnapshotsByStoreAsync(
        int storeId,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetSalesSnapshotsByStoreQuery(storeId, startDate, endDate);
        var result = await mediator.QueryAsync<GetSalesSnapshotsByStoreQuery, IEnumerable<SalesSnapshotDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateSalesSnapshotAsync(
        CreateSalesSnapshotRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateSalesSnapshotCommand(
            request.SaleId,
            request.StoreId,
            request.SaleDate,
            request.TotalAmount,
            request.CustomerId);
            
        var result = await mediator.SendAsync<CreateSalesSnapshotCommand, SalesSnapshotDto>(command, cancellationToken);
        
        return Results.CreatedAtRoute(
            "GetSalesSnapshotById",
            new { id = result.Id },
            result);
    }
}

// Request DTOs
internal record CreateSalesSnapshotRequest(
    [property: JsonPropertyName("saleId")] Guid SaleId,
    [property: JsonPropertyName("storeId")] int StoreId,
    [property: JsonPropertyName("saleDate")] DateOnly SaleDate,
    [property: JsonPropertyName("totalAmount")] decimal TotalAmount,
    [property: JsonPropertyName("customerId")] Guid? CustomerId = null); 