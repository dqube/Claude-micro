namespace AuthService.Application.DTOs;

public record RegistrationTokenDto
{
    public Guid Id { get; init; }
    public string Token { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string TokenType { get; init; } = string.Empty;
    public bool IsUsed { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public DateTime? UsedAt { get; init; }
    public Guid? UsedBy { get; init; }
}