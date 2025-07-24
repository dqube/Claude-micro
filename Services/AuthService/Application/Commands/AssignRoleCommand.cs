using BuildingBlocks.Application.CQRS.Commands;
using AuthService.Application.DTOs;

namespace AuthService.Application.Commands;

public class AssignRoleCommand : CommandBase<UserRoleDto>
{
    public Guid UserId { get; init; }
    public int RoleId { get; init; }
    public Guid AssignedBy { get; init; }

    public AssignRoleCommand(Guid userId, int roleId, Guid assignedBy)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedBy = assignedBy;
    }
}