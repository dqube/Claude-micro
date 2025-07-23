using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuthService.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (RegisterUserDto registerUserDto, IMediator mediator) =>
        {
            var command = new RegisterUserCommand(registerUserDto);
            var userId = await mediator.Send(command);
            return Results.Ok(userId);
        })
        .WithName("RegisterUser")
        .WithSummary("Register a new user")
        .WithDescription("Registers a new user with the provided information.");

        app.MapPost("/login", async (LoginUserDto loginUserDto, IMediator mediator) =>
        {
            var command = new LoginUserCommand(loginUserDto);
            var token = await mediator.Send(command);

            if (token == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(new { Token = token });
        })
        .WithName("LoginUser")
        .WithSummary("Log in a user")
        .WithDescription("Logs in a user with the provided credentials and returns a JWT token.");
    }
}
