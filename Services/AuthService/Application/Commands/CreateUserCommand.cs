using BuildingBlocks.Application.CQRS.Commands;
using AuthService.Application.DTOs;

namespace AuthService.Application.Commands;

public class CreateUserCommand : CommandBase<UserDto>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;

    public CreateUserCommand(string username, string email, string password, bool isActive = true)
    {
        Username = username;
        Email = email;
        Password = password;
        IsActive = isActive;
    }
}