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

internal static class DiscountCampaignEndpoints
{
    public static IEndpointRouteBuilder MapDiscountCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/discount-campaigns")
            .WithTags("Discount Campaigns")
            .WithOpenApi();

        // GET /api/discount-campaigns/{id}
        group.MapGet("/{id:guid}", GetDiscountCampaignByIdAsync)
            .WithName("GetDiscountCampaignById")
            .WithSummary("Get discount campaign by ID")
            .WithDescription("Retrieve a specific discount campaign by its unique identifier")
            .Produces<DiscountCampaignDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/discount-campaigns
        group.MapPost("/", CreateDiscountCampaignAsync)
            .WithName("CreateDiscountCampaign")
            .WithSummary("Create a new discount campaign")
            .WithDescription("Create a new discount campaign with the provided information")
            .Produces<DiscountCampaignDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetDiscountCampaignByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var query = new GetDiscountCampaignByIdQuery(id);
            var result = await mediator.QueryAsync<GetDiscountCampaignByIdQuery, DiscountCampaignDto>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Success(result, "Discount campaign retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.NotFound($"Discount campaign not found: {ex.Message}", correlationId);
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

    private static async Task<IResult> CreateDiscountCampaignAsync(
        [FromBody] CreateDiscountCampaignRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var command = new CreateDiscountCampaignCommand(
                request.Name,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.IsActive,
                request.MaxUsesPerCustomer);

            var result = await mediator.SendAsync<CreateDiscountCampaignCommand, DiscountCampaignDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"Discount campaign '{request.Name}' created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create discount campaign: {ex.Message}", correlationId);
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

public record CreateDiscountCampaignRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; init; }

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; init; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; init; } = true;

    [JsonPropertyName("maxUsesPerCustomer")]
    public int? MaxUsesPerCustomer { get; init; }
} 