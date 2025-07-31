#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using ReportingService.Application.Commands;
using ReportingService.Application.DTOs;
using ReportingService.Application.Queries;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Converters;
using System.Text.Json.Serialization;

namespace ReportingService.API.Endpoints;

internal static class PromotionEffectivenessEndpoints
{
    public static IEndpointRouteBuilder MapPromotionEffectivenessEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/promotion-effectiveness")
            .WithTags("Promotion Effectiveness")
            .WithOpenApi();

        // GET /api/promotion-effectiveness
        group.MapGet("/", GetPromotionEffectivenessAsync)
            .WithName("GetPromotionEffectiveness")
            .WithSummary("Get promotion effectiveness data")
            .WithDescription("Retrieve promotion effectiveness metrics with optional filtering")
            .Produces<IEnumerable<PromotionEffectivenessDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/promotion-effectiveness/{promotionId}
        group.MapGet("/{promotionId:guid}", GetPromotionEffectivenessByIdAsync)
            .WithName("GetPromotionEffectivenessById")
            .WithSummary("Get promotion effectiveness by promotion ID")
            .WithDescription("Retrieve effectiveness metrics for a specific promotion")
            .Produces<PromotionEffectivenessDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // GET /api/promotion-effectiveness/top/redemptions
        group.MapGet("/top/redemptions", GetTopPromotionsByRedemptionsAsync)
            .WithName("GetTopPromotionsByRedemptions")
            .WithSummary("Get top promotions by redemption count")
            .WithDescription("Retrieve the most redeemed promotions")
            .Produces<IEnumerable<PromotionEffectivenessDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/promotion-effectiveness/top/revenue
        group.MapGet("/top/revenue", GetTopPromotionsByRevenueAsync)
            .WithName("GetTopPromotionsByRevenue")
            .WithSummary("Get top promotions by revenue impact")
            .WithDescription("Retrieve promotions with highest revenue impact")
            .Produces<IEnumerable<PromotionEffectivenessDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/promotion-effectiveness/underperforming
        group.MapGet("/underperforming", GetUnderperformingPromotionsAsync)
            .WithName("GetUnderperformingPromotions")
            .WithSummary("Get underperforming promotions")
            .WithDescription("Retrieve promotions below performance thresholds")
            .Produces<IEnumerable<PromotionEffectivenessDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /api/promotion-effectiveness
        group.MapPost("/", CreatePromotionEffectivenessAsync)
            .WithName("CreatePromotionEffectiveness")
            .WithSummary("Create promotion effectiveness record")
            .WithDescription("Create a new promotion effectiveness tracking record")
            .Produces<PromotionEffectivenessDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetPromotionEffectivenessAsync(
        [FromQuery] Guid? promotionId,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] int? topCount,
        [FromQuery] string? orderBy,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetPromotionEffectivenessQuery(promotionId, startDate, endDate, topCount, orderBy);
        var result = await mediator.QueryAsync<GetPromotionEffectivenessQuery, IEnumerable<PromotionEffectivenessDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> GetPromotionEffectivenessByIdAsync(
        Guid promotionId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetPromotionEffectivenessQuery(promotionId);
        var result = await mediator.QueryAsync<GetPromotionEffectivenessQuery, IEnumerable<PromotionEffectivenessDto>>(query, cancellationToken);
        var single = result.FirstOrDefault();
        
        return single is not null
            ? Results.Ok(single)
            : Results.Problem(
                detail: $"No effectiveness data found for promotion '{promotionId}'.",
                statusCode: StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetTopPromotionsByRedemptionsAsync(
        IMediator mediator,
        CancellationToken cancellationToken,
        [FromQuery] int count = 10)
    {
        var query = new GetPromotionEffectivenessQuery(null, null, null, count, "redemptions");
        var result = await mediator.QueryAsync<GetPromotionEffectivenessQuery, IEnumerable<PromotionEffectivenessDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTopPromotionsByRevenueAsync(
        IMediator mediator,
        CancellationToken cancellationToken,
        [FromQuery] int count = 10)
    {
        var query = new GetPromotionEffectivenessQuery(null, null, null, count, "revenue");
        var result = await mediator.QueryAsync<GetPromotionEffectivenessQuery, IEnumerable<PromotionEffectivenessDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUnderperformingPromotionsAsync(
        IMediator mediator,
        CancellationToken cancellationToken,
        [FromQuery] int redemptionThreshold = 5,
        [FromQuery] decimal revenueThreshold = 100.00m)
    {
        var query = new GetUnderperformingPromotionsQuery(redemptionThreshold, revenueThreshold);
        var result = await mediator.QueryAsync<GetUnderperformingPromotionsQuery, IEnumerable<PromotionEffectivenessDto>>(query, cancellationToken);
        
        return Results.Ok(result);
    }

    private static async Task<IResult> CreatePromotionEffectivenessAsync(
        CreatePromotionEffectivenessRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreatePromotionEffectivenessCommand(
            request.PromotionId,
            request.RedemptionCount,
            request.RevenueImpact,
            request.AnalysisDate);
            
        var result = await mediator.SendAsync<CreatePromotionEffectivenessCommand, PromotionEffectivenessDto>(command, cancellationToken);
        
        return Results.Created($"/api/promotion-effectiveness/{result.PromotionId}", result);
    }
}

// Request DTOs
internal record CreatePromotionEffectivenessRequest(
    [property: JsonPropertyName("promotionId")] Guid PromotionId,
    [property: JsonPropertyName("redemptionCount")] int RedemptionCount = 0,
    [property: JsonPropertyName("revenueImpact")] decimal RevenueImpact = 0,
    [property: JsonPropertyName("analysisDate")] DateOnly? AnalysisDate = null);

// Additional query classes that might be needed
internal class GetUnderperformingPromotionsQuery : BuildingBlocks.Application.CQRS.Queries.QueryBase<IEnumerable<PromotionEffectivenessDto>>
{
    public int RedemptionThreshold { get; init; }
    public decimal RevenueThreshold { get; init; }
    
    public GetUnderperformingPromotionsQuery(int redemptionThreshold, decimal revenueThreshold)
    {
        RedemptionThreshold = redemptionThreshold;
        RevenueThreshold = revenueThreshold;
    }
} 