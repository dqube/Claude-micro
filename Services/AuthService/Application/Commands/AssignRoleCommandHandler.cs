using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.Commands;

public class AssignRoleCommandHandler : ICommandHandler<AssignRoleCommand, UserRoleDto>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<Role, RoleId> _roleRepository;
    private readonly IRepository<UserRole, UserId> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleCommandHandler(
        IRepository<User, UserId> userRepository,
        IRepository<Role, RoleId> roleRepository,
        IRepository<UserRole, UserId> userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserRoleDto> HandleAsync(AssignRoleCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = UserId.From(request.UserId);
        var roleId = RoleId.From(request.RoleId);

        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UserNotFoundException(userId);

        // Verify role exists
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken)
            ?? throw new RoleNotFoundException(roleId);

        // Check if user already has a role (since UserRole uses UserId as key)
        var existingUserRole = await _userRoleRepository.GetByIdAsync(userId, cancellationToken);

        if (existingUserRole is not null)
        {
            throw new InvalidOperationException($"User already has a role assigned");
        }

        // Create user role assignment
        var userRole = new UserRole(userId, roleId, UserId.From(request.AssignedBy));

        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserRoleDto
        {
            UserId = userRole.Id.Value, // UserId is the aggregate root Id (Guid)
            RoleId = userRole.RoleId.Value, // RoleId is int
            Username = user.Username.Value,
            RoleName = role.Name,
            AssignedAt = userRole.CreatedAt,
            AssignedBy = userRole.CreatedBy?.Value ?? request.AssignedBy
        };
    }
}