using BuildingBlocks.Application.CQRS.Commands;

namespace AuthService.Application.Commands;

public class UpdatePasswordCommand : CommandBase
{
    public Guid UserId { get; init; }
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public Guid UpdatedBy { get; init; }

    public UpdatePasswordCommand(Guid userId, string currentPassword, string newPassword, Guid updatedBy)
    {
        UserId = userId;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
        UpdatedBy = updatedBy;
    }
}