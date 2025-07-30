#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using PromotionService.Application.Commands;
using PromotionService.Application.DTOs;
using PromotionService.Application.Queries;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace PromotionService.API.Endpoints;

internal static class PromotionEndpoints
{
    public static IEndpointRouteBuilder MapPromotionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/promotions")
            .WithTags("Promotions")
            .WithOpenApi();

        // GET /api/promotions/{id}
        group.MapGet("/{id:guid}", GetPromotionByIdAsync)
            .WithName("GetPromotionById")
            .WithSummary("Get promotion by ID")
            .WithDescription("Retrieve a specific promotion by its unique identifier")
            .Produces<PromotionDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/promotions
        group.MapPost("/", CreatePromotionAsync)
            .WithName("CreatePromotion")
            .WithSummary("Create a new promotion")
            .WithDescription("Create a new promotion with the provided information")
            .Produces<PromotionDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetPromotionByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var query = new GetPromotionByIdQuery(id);
            var result = await mediator.QueryAsync<GetPromotionByIdQuery, PromotionDto>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Success(result, "Promotion retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.NotFound($"Promotion not found: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request cancelled: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request timeout: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> CreatePromotionAsync(
        [FromBody] CreatePromotionRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var command = new CreatePromotionCommand(
                request.Name,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.IsCombinable,
                request.MaxRedemptions);

            var result = await mediator.SendAsync<CreatePromotionCommand, PromotionDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"Promotion '{request.Name}' created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create promotion: {ex.Message}", correlationId);
        }
        catch (TaskCanceledException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request cancelled: {ex.Message}", correlationId);
        }
        catch (TimeoutException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Request timeout: {ex.Message}", correlationId);
        }
    }
}

public record CreatePromotionRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; init; }

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; init; }

    [JsonPropertyName("isCombinable")]
    public bool IsCombinable { get; init; } = false;

    [JsonPropertyName("maxRedemptions")]
    public int? MaxRedemptions { get; init; }
} 