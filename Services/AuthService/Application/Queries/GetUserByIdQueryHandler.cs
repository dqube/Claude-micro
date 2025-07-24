using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Repositories;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.Queries;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<UserDto> HandleAsync(GetUserByIdQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = UserId.From(request.UserId);
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UserNotFoundException(userId);

        // Get roles for this user using domain-specific method
        var userRoles = await _userRoleRepository.GetRolesByUserIdAsync(userId, cancellationToken);
        var roles = userRoles.Select(MapRoleToDto).ToList();

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