#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using AuthService.Application.DTOs;
using AuthService.Application.Queries;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace AuthService.API.Endpoints;

internal static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/roles")
            .WithTags("Roles")
            .WithOpenApi();

        // GET /api/roles
        group.MapGet("/", GetRolesAsync)
            .WithName("GetRoles")
            .WithSummary("Get all roles")
            .WithDescription("Retrieve a list of all available roles in the system")
            .Produces<IEnumerable<RoleDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetRolesAsync(
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetRolesQuery();
            var result = await mediator.QueryAsync<GetRolesQuery, IEnumerable<RoleDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();
            
            return ResponseFactory.Success(result, "Roles retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve roles: {ex.Message}", correlationId);
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