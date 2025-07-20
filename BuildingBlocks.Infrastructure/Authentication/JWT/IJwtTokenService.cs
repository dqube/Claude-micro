using System.Security.Claims;

namespace BuildingBlocks.Infrastructure.Authentication.JWT;

public interface IJwtTokenService
{
    string GenerateToken(IEnumerable<Claim> claims);
    string GenerateToken(string userId, string userName, IEnumerable<string>? roles = null, IDictionary<string, string>? additionalClaims = null);
    ClaimsPrincipal? ValidateToken(string token);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string refreshToken);
    DateTime GetTokenExpiration(string token);
    string? GetClaimValue(string token, string claimType);
    bool IsTokenExpired(string token);
    string? GetUserIdFromToken(string token);
    IEnumerable<string> GetRolesFromToken(string token);
}