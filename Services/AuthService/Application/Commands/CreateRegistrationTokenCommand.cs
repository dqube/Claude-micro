using BuildingBlocks.Application.CQRS.Commands;
using AuthService.Application.DTOs;

namespace AuthService.Application.Commands;

public class CreateRegistrationTokenCommand : CommandBase<RegistrationTokenDto>
{
    public string Email { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public int ExpirationHours { get; init; } = 24;
    public Guid? UserId { get; init; }

    public CreateRegistrationTokenCommand(string email, string tokenType, int expirationHours = 24, Guid? userId = null)
    {
        Email = email;
        TokenType = tokenType;
        ExpirationHours = expirationHours;
        UserId = userId;
    }
}