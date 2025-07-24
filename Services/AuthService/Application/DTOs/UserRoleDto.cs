namespace AuthService.Application.DTOs;

public record UserRoleDto
{
    public Guid UserId { get; init; }
    public int RoleId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
    public DateTime AssignedAt { get; init; }
    public Guid AssignedBy { get; init; }
}