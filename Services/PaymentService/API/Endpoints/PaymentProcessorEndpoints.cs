#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Commands;
using PaymentService.Application.DTOs;
using PaymentService.Application.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using System.Text.Json.Serialization;

namespace PaymentService.API.Endpoints;

internal static class PaymentProcessorEndpoints
{
    public static IEndpointRouteBuilder MapPaymentProcessorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/payment-processors")
            .WithTags("Payment Processors")
            .WithOpenApi();

        // GET /api/payment-processors
        group.MapGet("/", GetPaymentProcessorsAsync)
            .WithName("GetPaymentProcessors")
            .WithSummary("Get all payment processors with optional filtering")
            .WithDescription("Retrieve payment processors with optional active status filtering")
            .Produces<IEnumerable<PaymentProcessorDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/payment-processors/{id}
        group.MapGet("/{id:guid}", GetPaymentProcessorByIdAsync)
            .WithName("GetPaymentProcessorById")
            .WithSummary("Get payment processor by ID")
            .WithDescription("Retrieve a specific payment processor by its unique identifier")
            .Produces<PaymentProcessorDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/payment-processors
        group.MapPost("/", CreatePaymentProcessorAsync)
            .WithName("CreatePaymentProcessor")
            .WithSummary("Create a new payment processor")
            .WithDescription("Create a new payment processor")
            .Produces<PaymentProcessorDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetPaymentProcessorsAsync(
        [FromQuery] bool? isActive,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentProcessorsQuery(isActive);
        var result = await mediator.QueryAsync<GetPaymentProcessorsQuery, IEnumerable<PaymentProcessorDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPaymentProcessorByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        // This would need a GetPaymentProcessorByIdQuery
        return Results.Problem("Not implemented", statusCode: StatusCodes.Status501NotImplemented);
    }

    private static async Task<IResult> CreatePaymentProcessorAsync(
        CreatePaymentProcessorRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentProcessorCommand(
            request.Name,
            request.CommissionRate,
            request.IsActive);
            
        var result = await mediator.SendAsync<CreatePaymentProcessorCommand, PaymentProcessorDto>(command, cancellationToken);
        
        return Results.CreatedAtRoute(
            "GetPaymentProcessorById",
            new { id = result.Id },
            result);
    }
}

// Request DTOs
internal record CreatePaymentProcessorRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("commissionRate")] decimal CommissionRate = 0,
    [property: JsonPropertyName("isActive")] bool IsActive = true); 