using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.Commands;

public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand>
{
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePasswordCommandHandler(
        IRepository<User, UserId> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdatePasswordCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = UserId.From(request.UserId);
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UserNotFoundException(userId);

        // Verify current password
        if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Current password is incorrect");
        }

        // Create new password hash
        var passwordSalt = GeneratePasswordSalt();
        var passwordHash = HashPassword(request.NewPassword, passwordSalt);
        var newPasswordHash = PasswordHash.From(passwordHash, passwordSalt);

        // Update password
        user.UpdatePassword(newPasswordHash, passwordSalt, UserId.From(request.UpdatedBy));

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static bool VerifyPassword(string password, PasswordHash storedHash)
    {
        // Simplified password verification - in production use proper crypto
        var hashedInput = HashPassword(password, storedHash.Salt);
        return hashedInput == storedHash.Hash;
    }

    private static byte[] GeneratePasswordSalt()
    {
        return System.Text.Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
    }

    private static string HashPassword(string password, byte[] salt)
    {
        return Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt)));
    }
}