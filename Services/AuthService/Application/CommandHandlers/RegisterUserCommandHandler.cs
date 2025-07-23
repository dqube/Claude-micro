using AuthService.Application.Commands;
using AuthService.Domain.Entities;
using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using AuthService.Application.DTOs;
using AuthService.Domain.Events;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.CommandHandlers;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, UserDto>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(IRepository<User, UserId> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // NOTE: This is a simplified implementation. In a real-world scenario, you would:
        // 1. Hash the password using a secure algorithm (e.g., Argon2, scrypt, or PBKDF2).
        // 2. Add more robust validation.
        // 3. Potentially send a verification email.

        var username = new Username(request.RegisterUserDto.Username);
        var email = new Email(request.RegisterUserDto.Email);

        // Placeholder for password hashing
        byte[] passwordHash = new byte[0];
        byte[] passwordSalt = new byte[0];

        var user = User.Create(username, email, passwordHash, passwordSalt);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            UserId = user.Id.Value,
            Username = user.Username.Value,
            Email = user.Email.Value,
            IsActive = user.IsActive
        };
    }
}
