using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Repositories;

namespace AuthService.Application.Commands;

public class CreateRegistrationTokenCommandHandler : ICommandHandler<CreateRegistrationTokenCommand, RegistrationTokenDto>
{
    private readonly IRegistrationTokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRegistrationTokenCommandHandler(
        IRegistrationTokenRepository tokenRepository,
        IUnitOfWork unitOfWork)
    {
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegistrationTokenDto> HandleAsync(CreateRegistrationTokenCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Generate secure token
        var tokenValue = GenerateSecureToken();
        var expiresAt = DateTime.UtcNow.AddHours(request.ExpirationHours);

        // Create registration token
        var token = new RegistrationToken(
            TokenId.New(),
            tokenValue,
            new Email(request.Email),
            UserId.From(request.UserId ?? Guid.NewGuid()),
            TokenType.From(request.TokenType),
            expiresAt);

        await _tokenRepository.AddAsync(token, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(token);
    }

    private static string GenerateSecureToken()
    {
        // Generate a cryptographically secure random token
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static RegistrationTokenDto MapToDto(RegistrationToken token)
    {
        return new RegistrationTokenDto
        {
            Id = token.Id.Value,
            Token = token.Token,
            Email = token.Email.Value,
            TokenType = token.TokenType.Value,
            IsUsed = token.IsUsed,
            CreatedAt = token.CreatedAt,
            ExpiresAt = token.ExpiresAt,
            UsedAt = token.UsedAt,
            UsedBy = token.UsedBy?.Value
        };
    }
}