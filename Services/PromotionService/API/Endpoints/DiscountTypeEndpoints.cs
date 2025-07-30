#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using PromotionService.Application.DTOs;
using PromotionService.Application.Queries;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace PromotionService.API.Endpoints;

internal static class DiscountTypeEndpoints
{
    public static IEndpointRouteBuilder MapDiscountTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/discount-types")
            .WithTags("Discount Types")
            .WithOpenApi();

        // GET /api/discount-types
        group.MapGet("/", GetDiscountTypesAsync)
            .WithName("GetDiscountTypes")
            .WithSummary("Get all discount types")
            .WithDescription("Retrieve a list of all available discount types in the system")
            .Produces<IEnumerable<DiscountTypeDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetDiscountTypesAsync(
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetAllDiscountTypesQuery();
            var result = await mediator.QueryAsync<GetAllDiscountTypesQuery, List<DiscountTypeDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.Success(result, "Discount types retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve discount types: {ex.Message}", correlationId);
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
        // Removed generic catch for Exception to comply with best practices and error reporting guidelines.
    }
} 