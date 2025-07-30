using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedService.Application.Commands;
using SharedService.Application.Queries;
using SharedService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;

namespace SharedService.API.Endpoints;

public static class CurrencyEndpoints
{
    public static IEndpointRouteBuilder MapCurrencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/currencies")
            .WithTags("Currencies")
            .WithOpenApi();

        group.MapGet("/", async ([FromServices] IMediator mediator) =>
        {
            var query = new GetAllCurrenciesQuery();
            var result = await mediator.QueryAsync<GetAllCurrenciesQuery, IEnumerable<CurrencyDto>>(query);
            return Results.Ok(result);
        })
        .WithName("GetAllCurrencies")
        .Produces<IEnumerable<CurrencyDto>>();

        group.MapGet("/{code}", async (string code, [FromServices] IMediator mediator) =>
        {
            var query = new GetCurrencyByCodeQuery(code);
            var result = await mediator.QueryAsync<GetCurrencyByCodeQuery, CurrencyDto?>(query);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetCurrencyByCode")
        .Produces<CurrencyDto>()
        .Produces(404);

        group.MapPost("/", async ([FromBody] CreateCurrencyCommand command, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.SendAsync<CreateCurrencyCommand, CurrencyDto>(command);
            return Results.Created($"/api/currencies/{result.Code}", result);
        })
        .WithName("CreateCurrency")
        .Produces<CurrencyDto>(201)
        .Produces(400);

        group.MapPut("/{code}", async (string code, [FromBody] UpdateCurrencyCommand command, [FromServices] IMediator mediator) =>
        {
            if (code != command.Code)
                return Results.BadRequest("Code mismatch");

            var result = await mediator.SendAsync<UpdateCurrencyCommand, CurrencyDto>(command);
            return Results.Ok(result);
        })
        .WithName("UpdateCurrency")
        .Produces<CurrencyDto>()
        .Produces(400)
        .Produces(404);

        group.MapDelete("/{code}", async (string code, [FromServices] IMediator mediator) =>
        {
            var command = new DeleteCurrencyCommand(code);
            var result = await mediator.SendAsync<DeleteCurrencyCommand, bool>(command);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteCurrency")
        .Produces(204)
        .Produces(404);

        return endpoints;
    }
} 