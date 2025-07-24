using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.Queries;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IReadOnlyRepository<User, UserId> _userRepository;
    private readonly IReadOnlyRepository<UserRole, UserId> _userRoleRepository;
    private readonly IReadOnlyRepository<Role, RoleId> _roleRepository;

    public GetUserByIdQueryHandler(
        IReadOnlyRepository<User, UserId> userRepository,
        IReadOnlyRepository<UserRole, UserId> userRoleRepository,
        IReadOnlyRepository<Role, RoleId> roleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
    }

    public async Task<UserDto> HandleAsync(GetUserByIdQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = UserId.From(request.UserId);
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UserNotFoundException(userId);

        // Get user role (since UserRole uses UserId as key, there's only one role per user)
        var userRole = await _userRoleRepository.GetByIdAsync(userId, cancellationToken);

        var roles = new List<RoleDto>();
        if (userRole is not null)
        {
            var role = await _roleRepository.GetByIdAsync(userRole.RoleId, cancellationToken);
            if (role is not null)
            {
                roles.Add(MapRoleToDto(role));
            }
        }

        return MapToDto(user, roles);
    }

    private static UserDto MapToDto(User user, List<RoleDto> roles)
    {
        return new UserDto
        {
            Id = user.Id.Value,
            Username = user.Username.Value,
            Email = user.Email.Value,
            IsActive = user.IsActive,
            IsLocked = user.IsLockedOut(),
            FailedLoginAttempts = user.FailedLoginAttempts,
            LastLoginAt = null, // Not tracked in this version
            LockedUntil = user.LockoutEnd,
            CreatedAt = user.CreatedAt,
            LastUpdatedAt = user.UpdatedAt,
            Roles = roles
        };
    }

    private static RoleDto MapRoleToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id.Value,
            Name = role.Name,
            Description = role.Description ?? string.Empty,
            CreatedAt = role.CreatedAt,
            LastUpdatedAt = role.UpdatedAt
        };
    }
}