using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SupplierService.Application.Commands;
using SupplierService.Application.Queries;
using SupplierService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;

namespace SupplierService.API.Endpoints;

public static class SupplierContactEndpoints
{
    public static IEndpointRouteBuilder MapSupplierContactEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/suppliers/{supplierId:guid}/contacts")
            .WithTags("Supplier Contacts")
            .WithOpenApi();

        group.MapGet("/", async ([FromServices] IMediator mediator, [FromRoute] Guid supplierId) =>
        {
            var query = new GetSupplierContactsQuery(supplierId);
            var result = await mediator.QueryAsync<GetSupplierContactsQuery, IEnumerable<SupplierContactDto>>(query);
            return Results.Ok(result);
        })
        .WithName("GetSupplierContacts")
        .Produces<IEnumerable<SupplierContactDto>>();

        group.MapPost("/", async ([FromServices] IMediator mediator, [FromRoute] Guid supplierId, [FromBody] CreateSupplierContactRequest request) =>
        {
            var command = new CreateSupplierContactCommand(supplierId, request.FirstName, request.LastName, request.Email, request.Position, request.IsPrimary, request.Notes);
            var result = await mediator.SendAsync<CreateSupplierContactCommand, SupplierContactDto>(command);
            return Results.Created($"/api/suppliers/{supplierId}/contacts/{result.Id}", result);
        })
        .WithName("CreateSupplierContact")
        .Produces<SupplierContactDto>(201);

        return endpoints;
    }
}

public record CreateSupplierContactRequest(string FirstName, string LastName, string? Email = null, string? Position = null, bool IsPrimary = false, string? Notes = null); 