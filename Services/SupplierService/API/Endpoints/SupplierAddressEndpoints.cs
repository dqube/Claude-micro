using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SupplierService.Application.Queries;
using SupplierService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;

namespace SupplierService.API.Endpoints;

public static class SupplierAddressEndpoints
{
    public static IEndpointRouteBuilder MapSupplierAddressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/suppliers/{supplierId:guid}/addresses")
            .WithTags("Supplier Addresses")
            .WithOpenApi();

        group.MapGet("/", async ([FromServices] IMediator mediator, [FromRoute] Guid supplierId) =>
        {
            var query = new GetSupplierAddressesQuery(supplierId);
            var result = await mediator.QueryAsync<GetSupplierAddressesQuery, IEnumerable<SupplierAddressDto>>(query);
            return Results.Ok(result);
        })
        .WithName("GetSupplierAddresses")
        .Produces<IEnumerable<SupplierAddressDto>>();

        return endpoints;
    }
} 