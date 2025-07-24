#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace AuthService.API.Endpoints;

internal static class UserRoleEndpoints
{
    public static IEndpointRouteBuilder MapUserRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users/{userId:guid}/roles")
            .WithTags("User Roles")
            .WithOpenApi();

        // POST /api/users/{userId}/roles
        group.MapPost("/", AssignRoleToUserAsync)
            .WithName("AssignRoleToUser")
            .WithSummary("Assign a role to a user")
            .WithDescription("Assign a specific role to a user")
            .Produces<UserRoleDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> AssignRoleToUserAsync(
        [FromRoute] Guid userId,
        [FromBody] AssignRoleRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new AssignRoleCommand(userId, request.RoleId, request.AssignedBy);
            var result = await mediator.SendAsync<AssignRoleCommand, UserRoleDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/users/{userId}/roles", "Role assigned to user successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to assign role: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
        catch (Exception ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.InternalServerError($"An unexpected error occurred: {ex.Message}", correlationId);
        }
    }
}

// Request DTOs
internal record AssignRoleRequest(
    [property: JsonPropertyName("roleId")] int RoleId,
    [property: JsonPropertyName("assignedBy")] Guid AssignedBy
);