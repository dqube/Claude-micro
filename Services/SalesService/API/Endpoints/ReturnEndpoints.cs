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

internal static class ReturnEndpoints
{
    public static IEndpointRouteBuilder MapReturnEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/returns")
            .WithTags("Returns")
            .WithOpenApi();

        group.MapPost("/", CreateReturnAsync)
            .WithName("CreateReturn")
            .WithSummary("Create a new return")
            .WithDescription("Create a new return for a sale")
            .Produces<ReturnDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/details", AddReturnDetailAsync)
            .WithName("AddReturnDetail")
            .WithSummary("Add item to return")
            .WithDescription("Add a product detail to an existing return")
            .Produces<ReturnDetailDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/process", ProcessReturnAsync)
            .WithName("ProcessReturn")
            .WithSummary("Process return transaction")
            .WithDescription("Process a return transaction and calculate refund")
            .Produces<ReturnDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetReturnByIdAsync)
            .WithName("GetReturnById")
            .WithSummary("Get return by ID")
            .WithDescription("Retrieve a specific return by its unique identifier")
            .Produces<ReturnDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/sale/{saleId:guid}", GetReturnsBySaleIdAsync)
            .WithName("GetReturnsBySaleId")
            .WithSummary("Get returns by sale")
            .WithDescription("Retrieve returns for a specific sale")
            .Produces<IEnumerable<ReturnDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> CreateReturnAsync(
        [FromBody] CreateReturnRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new CreateReturnCommand(
                request.SaleId,
                request.EmployeeId,
                request.CustomerId);

            var result = await mediator.SendAsync<CreateReturnCommand, ReturnDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/returns/{result.Id}", "Return created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create return: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> AddReturnDetailAsync(
        [FromRoute] Guid id,
        [FromBody] AddReturnDetailRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new AddReturnDetailCommand(
                id,
                request.ProductId,
                request.Quantity,
                request.Reason,
                request.Restock);

            var result = await mediator.SendAsync<AddReturnDetailCommand, ReturnDetailDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/returns/{id}/details/{result.Id}", "Return detail added successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to add return detail: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> ProcessReturnAsync(
        [FromRoute] Guid id,
        [FromBody] ProcessReturnRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var processedBy = context.User.FindFirst("sub")?.Value != null && Guid.TryParse(context.User.FindFirst("sub")?.Value, out var userId) 
                ? userId 
                : (Guid?)null;

            var command = new ProcessReturnCommand(id, request.TotalRefund, processedBy);
            var result = await mediator.SendAsync<ProcessReturnCommand, ReturnDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Success(result, "Return processed successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to process return: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> GetReturnByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetReturnByIdQuery(id);
            var result = await mediator.QueryAsync<GetReturnByIdQuery, ReturnDto?>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return result != null
                ? ResponseFactory.Success(result, "Return retrieved successfully", correlationId)
                : ResponseFactory.NotFound("Return", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve return: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> GetReturnsBySaleIdAsync(
        [FromRoute] Guid saleId,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetReturnsBySaleIdQuery(saleId);
            var result = await mediator.QueryAsync<GetReturnsBySaleIdQuery, IEnumerable<ReturnDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Success(result, "Returns retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve returns: {ex.Message}", correlationId);
        }
    }
}

// Request DTOs
internal sealed record CreateReturnRequest(
    [property: JsonPropertyName("saleId")] Guid SaleId,
    [property: JsonPropertyName("employeeId")] Guid EmployeeId,
    [property: JsonPropertyName("customerId")] Guid? CustomerId = null
);

internal sealed record AddReturnDetailRequest(
    [property: JsonPropertyName("productId")] Guid ProductId,
    [property: JsonPropertyName("quantity")] int Quantity,
    [property: JsonPropertyName("reason")] string Reason,
    [property: JsonPropertyName("restock")] bool Restock = true
);

internal sealed record ProcessReturnRequest(
    [property: JsonPropertyName("totalRefund")] decimal TotalRefund
);