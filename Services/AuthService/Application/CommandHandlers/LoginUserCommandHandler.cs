using AuthService.Application.Commands;
using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using AuthService.Domain.Entities;
using BuildingBlocks.Domain.Common;
using AuthService.Application.DTOs;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.CommandHandlers;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginResponseDto>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(IRepository<User, UserId> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // NOTE: This is a simplified implementation. In a real-world scenario, you would:
        // 1. Verify the password hash.
        // 2. Implement proper error handling for invalid credentials.
        // 3. Generate a JWT token.

        var username = new Username(request.LoginUserDto.Username);
        var user = await _userRepository.GetAsync(u => u.Username == username, cancellationToken);

        if (user == null)
        {
            // Handle invalid username
            return null; // Or throw an exception, depending on error handling strategy
        }

        // Verify password (simplified)
        // In a real implementation, you would compare the hashed password

        // Generate JWT token (simplified)
        var token = "GeneratedJwtToken"; // Placeholder

        return new LoginResponseDto
        {
            Token = token,
            User = new UserDto
            {
                UserId = user.Id.Value,
                Username = user.Username.Value,
                Email = user.Email.Value,
                IsActive = user.IsActive
            }
        };
    }
}