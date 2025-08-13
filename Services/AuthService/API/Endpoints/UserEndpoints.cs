#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using AuthService.Application.Queries;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Converters;
using System.Text.Json.Serialization;

namespace AuthService.API.Endpoints;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users")
            .WithTags("Users")
            .WithOpenApi();

        // GET /api/users
        group.MapGet("/", GetUsersAsync)
            .WithName("GetUsers")
            .WithSummary("Get all users with optional filtering and pagination")
            .WithDescription("Retrieve a paginated list of users with optional search and filtering capabilities")
            .Produces<PagedResult<UserDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // GET /api/users/{id}
        group.MapGet("/{id:guid}", GetUserByIdAsync)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieve a specific user by their unique identifier")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // GET /api/users/by-email/{email}
        group.MapGet("/by-email/{email}", GetUserByEmailAsync)
            .WithName("GetUserByEmail")
            .WithSummary("Get user by email")
            .WithDescription("Retrieve a specific user by their email address")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /api/users
        group.MapPost("/", CreateUserAsync)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .WithDescription("Create a new user account with the provided information")
            .Produces<UserDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // PUT /api/users/{id}/password
        group.MapPut("/{id:guid}/password", UpdatePasswordAsync)
            .WithName("UpdateUserPassword")
            .WithSummary("Update user password")
            .WithDescription("Update the password for a specific user")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetUsersAsync(
        [FromServices] IMediator mediator,
        [AsParameters] GetUsersRequest request,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetUsersQuery(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.IsActive,
                request.IsLocked);

            var result = await mediator.QueryAsync<GetUsersQuery, PagedResult<UserDto>>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.PagedResult(result.Items, result.TotalCount, result.Page, result.PageSize, "Users retrieved successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve users: {ex.Message}", correlationId);
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
        // Remove generic catch for Exception to comply with allowed exception types.
    }

    private static async Task<IResult> GetUserByIdAsync(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetUserByIdQuery(id);
            var result = await mediator.QueryAsync<GetUserByIdQuery, UserDto?>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return result != null
                ? ResponseFactory.Success(result, "User retrieved successfully", correlationId)
                : ResponseFactory.NotFound("User", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve user: {ex.Message}", correlationId);
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

    private static async Task<IResult> GetUserByEmailAsync(
        [FromRoute] string email,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var query = new GetUserByEmailQuery(email);
            var result = await mediator.QueryAsync<GetUserByEmailQuery, UserDto?>(query, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return result != null
                ? ResponseFactory.Success(result, "User retrieved successfully", correlationId)
                : ResponseFactory.NotFound("User", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to retrieve user: {ex.Message}", correlationId);
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

    private static async Task<IResult> CreateUserAsync(
        [FromBody] CreateUserRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new CreateUserCommand(request.Username, request.Email, request.Password, request.IsActive);
            var result = await mediator.SendAsync<CreateUserCommand, UserDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/users/{result.Id}", "User created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create user: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
        // Removed generic catch for Exception to comply with allowed exception types.
    }

    private static async Task<IResult> UpdatePasswordAsync(
        [FromRoute] Guid id,
        [FromBody] UpdatePasswordRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            // For password updates via admin endpoint, current password is not required
            // UpdatedBy should be the current user's ID or system admin ID
            var updatedBy = context.User.FindFirst("sub")?.Value != null && Guid.TryParse(context.User.FindFirst("sub")?.Value, out var userId) 
                ? userId 
                : Guid.Empty; // System user
            
            var command = new UpdatePasswordCommand(id, string.Empty, request.NewPassword, updatedBy);
            await mediator.SendAsync(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.NoContent(correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to update password: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
        // Removed generic catch for Exception to comply with allowed exception types.
    }
}

// Request DTOs
internal sealed record GetUsersRequest(
    [property: JsonPropertyName("page")] int Page = 1,
    [property: JsonPropertyName("pageSize")] int PageSize = 10,
    [property: JsonPropertyName("searchTerm")] string? SearchTerm = null,
    [property: JsonPropertyName("isActive")] bool? IsActive = null,
    [property: JsonPropertyName("isLocked")] bool? IsLocked = null
);

internal sealed record UpdatePasswordRequest(
    [property: JsonPropertyName("newPassword")] string NewPassword
);

internal sealed record CreateUserRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("isActive")] bool IsActive
);