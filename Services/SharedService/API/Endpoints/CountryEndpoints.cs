using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedService.Application.Commands;
using SharedService.Application.Queries;
using SharedService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;

namespace SharedService.API.Endpoints;

public static class CountryEndpoints
{
    public static IEndpointRouteBuilder MapCountryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/countries")
            .WithTags("Countries")
            .WithOpenApi();

        group.MapGet("/", async ([FromServices] IMediator mediator) =>
        {
            var query = new GetAllCountriesQuery();
            var result = await mediator.QueryAsync<GetAllCountriesQuery, IEnumerable<CountryDto>>(query);
            return Results.Ok(result);
        })
        .WithName("GetAllCountries")
        .Produces<IEnumerable<CountryDto>>();

        group.MapGet("/{code}", async (string code, [FromServices] IMediator mediator) =>
        {
            var query = new GetCountryByCodeQuery(code);
            var result = await mediator.QueryAsync<GetCountryByCodeQuery, CountryDto?>(query);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetCountryByCode")
        .Produces<CountryDto>()
        .Produces(404);

        group.MapGet("/currency/{currencyCode}", async (string currencyCode, [FromServices] IMediator mediator) =>
        {
            var query = new GetCountriesByCurrencyQuery(currencyCode);
            var result = await mediator.QueryAsync<GetCountriesByCurrencyQuery, IEnumerable<CountryDto>>(query);
            return Results.Ok(result);
        })
        .WithName("GetCountriesByCurrency")
        .Produces<IEnumerable<CountryDto>>();

        group.MapPost("/", async ([FromBody] CreateCountryCommand command, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.SendAsync<CreateCountryCommand, CountryDto>(command);
            return Results.Created($"/api/countries/{result.Code}", result);
        })
        .WithName("CreateCountry")
        .Produces<CountryDto>(201)
        .Produces(400);

        group.MapPut("/{code}", async (string code, [FromBody] UpdateCountryCommand command, [FromServices] IMediator mediator) =>
        {
            if (code != command.Code)
                return Results.BadRequest("Code mismatch");

            var result = await mediator.SendAsync<UpdateCountryCommand, CountryDto>(command);
            return Results.Ok(result);
        })
        .WithName("UpdateCountry")
        .Produces<CountryDto>()
        .Produces(400)
        .Produces(404);

        group.MapDelete("/{code}", async (string code, [FromServices] IMediator mediator) =>
        {
            var command = new DeleteCountryCommand(code);
            var result = await mediator.SendAsync<DeleteCountryCommand, bool>(command);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteCountry")
        .Produces(204)
        .Produces(404);

        return endpoints;
    }
} 