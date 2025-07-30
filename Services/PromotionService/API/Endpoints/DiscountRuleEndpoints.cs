#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using PromotionService.Application.Commands;
using PromotionService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace PromotionService.API.Endpoints;

internal static class DiscountRuleEndpoints
{
    public static IEndpointRouteBuilder MapDiscountRuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/discount-rules")
            .WithTags("Discount Rules")
            .WithOpenApi();

        // POST /api/discount-rules
        group.MapPost("/", CreateDiscountRuleAsync)
            .WithName("CreateDiscountRule")
            .WithSummary("Create a new discount rule")
            .WithDescription("Create a new discount rule for a campaign with the provided information")
            .Produces<DiscountRuleDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> CreateDiscountRuleAsync(
        [FromBody] CreateDiscountRuleRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var command = new CreateDiscountRuleCommand(
                request.CampaignId,
                request.RuleType,
                request.DiscountValue,
                request.DiscountMethod,
                request.ProductId,
                request.CategoryId,
                request.MinQuantity,
                request.MinAmount,
                request.FreeProductId);

            var result = await mediator.SendAsync<CreateDiscountRuleCommand, DiscountRuleDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, "Discount rule created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create discount rule: {ex.Message}", correlationId);
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

public record CreateDiscountRuleRequest
{
    [JsonPropertyName("campaignId")]
    public Guid CampaignId { get; init; }

    [JsonPropertyName("ruleType")]
    public string RuleType { get; init; } = string.Empty;

    [JsonPropertyName("productId")]
    public Guid? ProductId { get; init; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; init; }

    [JsonPropertyName("minQuantity")]
    public int? MinQuantity { get; init; }

    [JsonPropertyName("minAmount")]
    public decimal? MinAmount { get; init; }

    [JsonPropertyName("discountValue")]
    public decimal DiscountValue { get; init; }

    [JsonPropertyName("discountMethod")]
    public string DiscountMethod { get; init; } = string.Empty;

    [JsonPropertyName("freeProductId")]
    public Guid? FreeProductId { get; init; }
} 