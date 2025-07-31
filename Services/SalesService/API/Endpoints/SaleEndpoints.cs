#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using SalesService.Application.Commands;
using SalesService.Application.DTOs;
using SalesService.Application.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace SalesService.API.Endpoints;

internal static class SaleEndpoints
{
    public static IEndpointRouteBuilder MapSaleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/sales")
            .WithTags("Sales")
            .WithOpenApi();

        group.MapPost("/", CreateSaleAsync)
            .WithName("CreateSale")
            .WithSummary("Create a new sale")
            .WithDescription("Create a new sale transaction")
            .Produces<SaleDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/details", AddSaleDetailAsync)
            .WithName("AddSaleDetail")
            .WithSummary("Add item to sale")
            .WithDescription("Add a product detail to an existing sale")
            .Produces<SaleDetailDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/complete", CompleteSaleAsync)
            .WithName("CompleteSale")
            .WithSummary("Complete sale transaction")
            .WithDescription("Complete a sale transaction and finalize totals")
            .Produces<SaleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetSaleByIdAsync)
            .WithName("GetSaleById")
            .WithSummary("Get sale by ID")
            .WithDescription("Retrieve a specific sale by its unique identifier")
            .Produces<SaleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/store/{storeId:int}", GetSalesByStoreIdAsync)
            .WithName("GetSalesByStoreId")
            .WithSummary("Get sales by store")
            .WithDescription("Retrieve sales for a specific store with optional date filtering")
            .Produces<IEnumerable<SaleDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> CreateSaleAsync(
        [FromBody] CreateSaleRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new CreateSaleCommand(
                request.StoreId,
                request.EmployeeId,
                request.RegisterId,
                request.ReceiptNumber,
                request.CustomerId);

            var result = await mediator.SendAsync<CreateSaleCommand, SaleDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/sales/{result.Id}", "Sale created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create sale: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> AddSaleDetailAsync(
        [FromRoute] Guid id,
        [FromBody] AddSaleDetailRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new AddSaleDetailCommand(
                id,
                request.ProductId,
                request.Quantity,
                request.UnitPrice,
                request.TaxApplied);

            var result = await mediator.SendAsync<AddSaleDetailCommand, SaleDetailDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/sales/{id}/details/{result.Id}", "Sale detail added successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to add sale detail: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> CompleteSaleAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var completedBy = context.User.FindFirst("sub")?.Value != null && Guid.TryParse(context.User.FindFirst("sub")?.Value, out var userId) 
                ? userId 
                : (Guid?)null;

            var command = new CompleteSaleCommand(id, completedBy);
            var result = await mediator.SendAsync<CompleteSaleCommand, SaleDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Success(result, "Sale completed successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to complete sale: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> GetSaleByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetSaleByIdQuery(id);
            var result = await mediator.QueryAsync<GetSaleByIdQuery, SaleDto?>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return result != null
                ? ResponseFactory.Success(result, "Sale retrieved successfully", correlationId)
                : ResponseFactory.NotFound("Sale", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve sale: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> GetSalesByStoreIdAsync(
        [FromRoute] int storeId,
        [AsParameters] GetSalesByStoreRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetSalesByStoreIdQuery(storeId, request.FromDate, request.ToDate);
            var result = await mediator.QueryAsync<GetSalesByStoreIdQuery, IEnumerable<SaleDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Success(result, "Sales retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve sales: {ex.Message}", correlationId);
        }
    }
}

// Request DTOs
internal sealed record CreateSaleRequest(
    [property: JsonPropertyName("storeId")] int StoreId,
    [property: JsonPropertyName("employeeId")] Guid EmployeeId,
    [property: JsonPropertyName("registerId")] int RegisterId,
    [property: JsonPropertyName("receiptNumber")] string ReceiptNumber,
    [property: JsonPropertyName("customerId")] Guid? CustomerId = null
);

internal sealed record AddSaleDetailRequest(
    [property: JsonPropertyName("productId")] Guid ProductId,
    [property: JsonPropertyName("quantity")] int Quantity,
    [property: JsonPropertyName("unitPrice")] decimal UnitPrice,
    [property: JsonPropertyName("taxApplied")] decimal TaxApplied = 0
);

internal sealed record GetSalesByStoreRequest(
    [property: JsonPropertyName("fromDate")] DateTime? FromDate = null,
    [property: JsonPropertyName("toDate")] DateTime? ToDate = null
);