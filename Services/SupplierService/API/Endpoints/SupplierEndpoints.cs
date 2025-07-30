using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SupplierService.Application.Commands;
using SupplierService.Application.Queries;
using SupplierService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;

namespace SupplierService.API.Endpoints;

public static class SupplierEndpoints
{
    public static IEndpointRouteBuilder MapSupplierEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/suppliers")
            .WithTags("Suppliers")
            .WithOpenApi();

        group.MapGet("/", async ([FromServices] IMediator mediator, [FromQuery] bool activeOnly = true) =>
        {
            var query = new GetAllSuppliersQuery(activeOnly);
            var result = await mediator.QueryAsync<GetAllSuppliersQuery, IEnumerable<SupplierDto>>(query);
            return Results.Ok(result);
        })
        .WithName("GetAllSuppliers")
        .Produces<IEnumerable<SupplierDto>>();

        group.MapGet("/{id:guid}", async ([FromServices] IMediator mediator, [FromRoute] Guid id) =>
        {
            var query = new GetSupplierByIdQuery(id);
            var result = await mediator.QueryAsync<GetSupplierByIdQuery, SupplierDto?>(query);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetSupplierById")
        .Produces<SupplierDto>()
        .Produces(404);

        group.MapGet("/search", async ([FromServices] IMediator mediator, [FromQuery] string name) =>
        {
            var query = new GetSuppliersByNameQuery(name);
            var result = await mediator.QueryAsync<GetSuppliersByNameQuery, IEnumerable<SupplierDto>>(query);
            return Results.Ok(result);
        })
        .WithName("SearchSuppliersByName")
        .Produces<IEnumerable<SupplierDto>>();

        group.MapPost("/", async ([FromServices] IMediator mediator, [FromBody] CreateSupplierRequest request) =>
        {
            var command = new CreateSupplierCommand(request.Name, request.TaxIdentificationNumber, request.Website, request.Notes);
            var result = await mediator.SendAsync<CreateSupplierCommand, SupplierDto>(command);
            return Results.Created($"/api/suppliers/{result.Id}", result);
        })
        .WithName("CreateSupplier")
        .Produces<SupplierDto>(201);

        group.MapPut("/{id:guid}", async ([FromServices] IMediator mediator, [FromRoute] Guid id, [FromBody] UpdateSupplierRequest request) =>
        {
            var command = new UpdateSupplierCommand(id, request.Name, request.TaxIdentificationNumber, request.Website, request.Notes);
            var result = await mediator.SendAsync<UpdateSupplierCommand, SupplierDto>(command);
            return Results.Ok(result);
        })
        .WithName("UpdateSupplier")
        .Produces<SupplierDto>();

        group.MapDelete("/{id:guid}", async ([FromServices] IMediator mediator, [FromRoute] Guid id) =>
        {
            var command = new DeleteSupplierCommand(id);
            var result = await mediator.SendAsync<DeleteSupplierCommand, bool>(command);
            return result ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteSupplier")
        .Produces(204)
        .Produces(404);

        group.MapPost("/{id:guid}/activate", async ([FromServices] IMediator mediator, [FromRoute] Guid id) =>
        {
            var command = new ActivateSupplierCommand(id);
            var result = await mediator.SendAsync<ActivateSupplierCommand, SupplierDto>(command);
            return Results.Ok(result);
        })
        .WithName("ActivateSupplier")
        .Produces<SupplierDto>();

        group.MapPost("/{id:guid}/deactivate", async ([FromServices] IMediator mediator, [FromRoute] Guid id) =>
        {
            var command = new DeactivateSupplierCommand(id);
            var result = await mediator.SendAsync<DeactivateSupplierCommand, SupplierDto>(command);
            return Results.Ok(result);
        })
        .WithName("DeactivateSupplier")
        .Produces<SupplierDto>();

        return endpoints;
    }
}

public record CreateSupplierRequest(string Name, string? TaxIdentificationNumber = null, string? Website = null, string? Notes = null);
public record UpdateSupplierRequest(string Name, string? TaxIdentificationNumber = null, string? Website = null, string? Notes = null); 