using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.Commands;
using StoreService.Application.DTOs;
using StoreService.Domain.ValueObjects;

namespace StoreService.API.Endpoints;

internal static class RegisterEndpoints
{
    public static IEndpointRouteBuilder MapRegisterEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/stores/registers")
            .WithTags("Registers")
            .WithOpenApi();

        group.MapPost("/", CreateRegisterAsync)
            .WithName("CreateRegister")
            .WithSummary("Create a new register")
            .WithDescription("Creates a new register for a store")
            .Produces<RegisterDto>(201)
            .Produces(400);

        group.MapPost("/{id:int}/open", OpenRegisterAsync)
            .WithName("OpenRegister")
            .WithSummary("Open a register")
            .WithDescription("Opens a register for transactions")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        group.MapPost("/{id:int}/close", CloseRegisterAsync)
            .WithName("CloseRegister")
            .WithSummary("Close a register")
            .WithDescription("Closes a register and records final cash count")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        group.MapPost("/{id:int}/add-cash", AddCashAsync)
            .WithName("AddCash")
            .WithSummary("Add cash to register")
            .WithDescription("Adds cash to the register drawer")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        group.MapPost("/{id:int}/remove-cash", RemoveCashAsync)
            .WithName("RemoveCash")
            .WithSummary("Remove cash from register")
            .WithDescription("Removes cash from the register drawer")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        return app;
    }

    private static async Task<IResult> CreateRegisterAsync(
        CreateRegisterRequest request,
        [FromServices] IMediator mediator)
    {
        var storeId = StoreId.From(request.StoreId);
        var command = new CreateRegisterCommand(storeId, request.Name);

        var result = await mediator.SendAsync<CreateRegisterCommand, RegisterDto>(command);
        return Results.Created($"/api/stores/registers/{result.Id}", result);
    }

    private static async Task<IResult> OpenRegisterAsync(
        int id,
        OpenRegisterRequest request,
        [FromServices] IMediator mediator)
    {
        var registerId = RegisterId.From(id);
        var command = new OpenRegisterCommand(registerId, request.StartingCash);

        await mediator.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> CloseRegisterAsync(
        int id,
        CloseRegisterRequest request,
        [FromServices] IMediator mediator)
    {
        var registerId = RegisterId.From(id);
        var command = new CloseRegisterCommand(registerId, request.EndingCash);

        await mediator.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> AddCashAsync(
        int id,
        AddCashRequest request,
        [FromServices] IMediator mediator)
    {
        var registerId = RegisterId.From(id);
        var command = new AddCashCommand(registerId, request.Amount, request.Note);

        await mediator.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> RemoveCashAsync(
        int id,
        RemoveCashRequest request,
        [FromServices] IMediator mediator)
    {
        var registerId = RegisterId.From(id);
        var command = new RemoveCashCommand(registerId, request.Amount, request.Note);

        await mediator.SendAsync(command);
        return Results.NoContent();
    }
}

public record CreateRegisterRequest(int StoreId, string Name);
public record OpenRegisterRequest(decimal StartingCash);
public record CloseRegisterRequest(decimal EndingCash);
public record AddCashRequest(decimal Amount, string Note);
public record RemoveCashRequest(decimal Amount, string Note); 