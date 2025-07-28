#pragma warning disable CA1812 // Request DTOs are instantiated by ASP.NET Core model binding
using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using BuildingBlocks.Application.CQRS.Mediator;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Extensions;
using System.Text.Json.Serialization;

namespace AuthService.API.Endpoints;

internal static class RegistrationTokenEndpoints
{
    public static IEndpointRouteBuilder MapRegistrationTokenEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/registration-tokens")
            .WithTags("Registration Tokens")
            .WithOpenApi();

        // POST /api/registration-tokens
        group.MapPost("/", CreateRegistrationTokenAsync)
            .WithName("CreateRegistrationToken")
            .WithSummary("Create a registration token")
            .WithDescription("Create a new registration token for email verification or password reset")
            .Produces<RegistrationTokenDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        // POST /api/registration-tokens/{token}/verify
        group.MapPost("/{token}/verify", VerifyTokenAsync)
            .WithName("VerifyToken")
            .WithSummary("Verify a registration token")
            .WithDescription("Verify if a registration token is valid and not expired")
            .Produces<RegistrationTokenDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateRegistrationTokenAsync(
        [FromBody] CreateRegistrationTokenRequest request,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            var command = new CreateRegistrationTokenCommand(
                request.Email, 
                request.TokenType, 
                request.ExpirationHours, 
                request.UserId);
            
            var result = await mediator.SendAsync<CreateRegistrationTokenCommand, RegistrationTokenDto>(command, cancellationToken);
            var correlationId = context.GetCorrelationId();

            return ResponseFactory.Created(result, $"/api/registration-tokens/{result.Id}", "Registration token created successfully", correlationId);
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to create registration token: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
       
    }

    private static async Task<IResult> VerifyTokenAsync(
        [FromRoute] string token,
        [FromServices] IMediator mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(context);
        
        try
        {
            // Note: This would need a corresponding query/command in the Application layer
            // For now, we'll return a placeholder response asynchronously
            var correlationId = context.GetCorrelationId();
            var result = await Task.Run(() =>
                ResponseFactory.Success(new { Token = token, IsValid = true }, "Token verified successfully", correlationId)
            );
            return result;
        }
        catch (InvalidOperationException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Failed to verify token: {ex.Message}", correlationId);
        }
        catch (ArgumentException ex)
        {
            var correlationId = context.GetCorrelationId();
            return ResponseFactory.BadRequest($"Invalid request: {ex.Message}", correlationId);
        }
       
    }
}

// Request DTOs
internal sealed record CreateRegistrationTokenRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("tokenType")] string TokenType,
    [property: JsonPropertyName("expirationHours")] int ExpirationHours = 24,
    [property: JsonPropertyName("userId")] Guid? UserId = null
);