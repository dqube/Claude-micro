using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.Commands;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDto>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IRepository<User, UserId> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check if user with email already exists
        var existingUser = await _userRepository.FindFirstAsync(
            u => u.Email.Value == request.Email, 
            cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException($"User with email '{request.Email}' already exists");
        }

        // Create password hash and salt (simplified for this implementation)
        var passwordSalt = GeneratePasswordSalt();
        var passwordHash = HashPassword(request.Password, passwordSalt);

        // Create user entity
        var user = new User(
            UserId.New(),
            Username.From(request.Username),
            new Email(request.Email),
            PasswordHash.From(System.Text.Encoding.UTF8.GetBytes(passwordHash)),
            passwordSalt);

        // Add to repository
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    private static byte[] GeneratePasswordSalt()
    {
        // Simplified salt generation - in production use proper crypto
        return System.Text.Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
    }

    private static string HashPassword(string password, byte[] salt)
    {
        // Simplified password hashing - in production use BCrypt or Argon2
        return Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt)));
    }

    private static UserDto MapToDto(User user)
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
            Roles = new List<RoleDto>() // Will be populated when user has roles
        };
    }
}