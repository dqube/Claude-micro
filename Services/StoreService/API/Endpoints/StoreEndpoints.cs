using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.Commands;
using StoreService.Application.DTOs;
using StoreService.Application.Queries;
using StoreService.Domain.ValueObjects;

namespace StoreService.API.Endpoints;

internal static class StoreEndpoints
{
    public static IEndpointRouteBuilder MapStoreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/stores")
            .WithTags("Stores")
            .WithOpenApi();

        group.MapGet("/", GetStoresAsync)
            .WithName("GetStores")
            .WithSummary("Get stores with pagination and filtering")
            .WithDescription("Retrieves a paginated list of stores with optional filtering by search term, status, location, and operational state")
            .Produces<PagedResult<StoreDto>>();

        group.MapGet("/{id:int}", GetStoreByIdAsync)
            .WithName("GetStoreById")
            .WithSummary("Get store by ID")
            .WithDescription("Retrieves a specific store by its unique identifier")
            .Produces<StoreDto>()
            .Produces(404);

        group.MapPost("/", CreateStoreAsync)
            .WithName("CreateStore")
            .WithSummary("Create a new store")
            .WithDescription("Creates a new store with the provided information")
            .Produces<StoreDto>(201)
            .Produces(400);

        group.MapPut("/{id:int}/status", UpdateStoreStatusAsync)
            .WithName("UpdateStoreStatus")
            .WithSummary("Update store status")
            .WithDescription("Updates the operational status of a store (Active, Maintenance, Closed)")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        return app;
    }

    private static async Task<IResult> GetStoresAsync(
        [AsParameters] GetStoresRequest request,
        [FromServices] IMediator mediator)
    {
        var query = new GetStoresQuery(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            request.LocationId,
            request.IsOperational,
            request.SortBy,
            request.SortDescending);

        var result = await mediator.QueryAsync<GetStoresQuery, PagedResult<StoreDto>>(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetStoreByIdAsync(
        int id,
        [FromServices] IMediator mediator)
    {
        var storeId = StoreId.From(id);
        var query = new GetStoreByIdQuery(storeId);

        var result = await mediator.QueryAsync<GetStoreByIdQuery, StoreDto?>(query);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateStoreAsync(
        CreateStoreRequest request,
        [FromServices] IMediator mediator)
    {
        var command = new CreateStoreCommand(
            request.Name,
            request.LocationId,
            request.Address,
            request.PhoneNumber,
            request.OpeningHours);

        var result = await mediator.SendAsync<CreateStoreCommand, StoreDto>(command);
        return Results.Created($"/api/stores/{result.Id}", result);
    }

    private static async Task<IResult> UpdateStoreStatusAsync(
        int id,
        UpdateStoreStatusRequest request,
        [FromServices] IMediator mediator)
    {
        var storeId = StoreId.From(id);
        var command = new UpdateStoreStatusCommand(storeId, request.Status);

        await mediator.SendAsync(command);
        return Results.NoContent();
    }
}

public record GetStoresRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? Status = null,
    int? LocationId = null,
    bool? IsOperational = null,
    string SortBy = "CreatedAt",
    bool SortDescending = true);

public record CreateStoreRequest(
    string Name,
    int LocationId,
    AddressDto Address,
    string PhoneNumber,
    string OpeningHours);

public record UpdateStoreStatusRequest(string Status); 