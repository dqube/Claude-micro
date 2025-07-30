#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using PromotionService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;

namespace PromotionService.API.Endpoints;

internal static class PromotionProductEndpoints
{
    public static IEndpointRouteBuilder MapPromotionProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/promotion-products")
            .WithTags("Promotion Products")
            .WithOpenApi();

        // Placeholder endpoint - can be expanded later
        group.MapGet("/health", () => Results.Ok("PromotionProduct endpoints are available"))
            .WithName("PromotionProductHealth")
            .WithSummary("Health check for promotion product endpoints")
            .WithDescription("Simple health check endpoint for promotion products")
            .Produces<string>(StatusCodes.Status200OK)
            .ExcludeFromDescription();

        return app;
    }
} 