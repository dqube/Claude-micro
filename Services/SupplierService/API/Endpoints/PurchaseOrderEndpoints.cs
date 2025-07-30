using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SupplierService.Application.Commands;
using SupplierService.Application.Queries;
using SupplierService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;

namespace SupplierService.API.Endpoints;

public static class PurchaseOrderEndpoints
{
    public static IEndpointRouteBuilder MapPurchaseOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/purchase-orders")
            .WithTags("Purchase Orders")
            .WithOpenApi();

        group.MapGet("/", async ([FromServices] IMediator mediator, 
            [FromQuery] Guid? supplierId = null,
            [FromQuery] int? storeId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null) =>
        {
            var query = new GetPurchaseOrdersQuery(supplierId, storeId, status, fromDate, toDate);
            var result = await mediator.QueryAsync<GetPurchaseOrdersQuery, IEnumerable<PurchaseOrderDto>>(query);
            return Results.Ok(result);
        })
        .WithName("GetPurchaseOrders")
        .Produces<IEnumerable<PurchaseOrderDto>>();

        group.MapGet("/{id:guid}", async ([FromServices] IMediator mediator, [FromRoute] Guid id, [FromQuery] bool includeDetails = true) =>
        {
            var query = new GetPurchaseOrderByIdQuery(id, includeDetails);
            var result = await mediator.QueryAsync<GetPurchaseOrderByIdQuery, PurchaseOrderDto?>(query);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetPurchaseOrderById")
        .Produces<PurchaseOrderDto>()
        .Produces(404);

        group.MapPost("/", async ([FromServices] IMediator mediator, [FromBody] CreatePurchaseOrderRequest request) =>
        {
            var command = new CreatePurchaseOrderCommand(request.SupplierId, request.StoreId, request.ExpectedDate, request.ShippingAddressId, request.ContactPersonId);
            var result = await mediator.SendAsync<CreatePurchaseOrderCommand, PurchaseOrderDto>(command);
            return Results.Created($"/api/purchase-orders/{result.Id}", result);
        })
        .WithName("CreatePurchaseOrder")
        .Produces<PurchaseOrderDto>(201);

        group.MapPost("/{id:guid}/submit", async ([FromServices] IMediator mediator, [FromRoute] Guid id) =>
        {
            var command = new SubmitPurchaseOrderCommand(id);
            var result = await mediator.SendAsync<SubmitPurchaseOrderCommand, PurchaseOrderDto>(command);
            return Results.Ok(result);
        })
        .WithName("SubmitPurchaseOrder")
        .Produces<PurchaseOrderDto>();

        return endpoints;
    }
}

public record CreatePurchaseOrderRequest(Guid SupplierId, int StoreId, DateTime? ExpectedDate = null, Guid? ShippingAddressId = null, Guid? ContactPersonId = null); 