using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Infrastructure.Authentication.JWT;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(JwtConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    // ...existing methods and static fields...
    // (Move all methods and static fields here, remove stray closing brace after constructor)

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(_configuration.TokenLifetime),
                signingCredentials: credentials);

            return _tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            LogJwtTokenGenerationError(_logger, ex);
            throw;
        }
    }

    public string GenerateToken(string userId, string userName, IEnumerable<string>? roles = null, IDictionary<string, string>? additionalClaims = null)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(userName);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64)
        };

        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));
        }

        return GenerateToken(claims);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        ArgumentNullException.ThrowIfNull(token);
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.SecretKey));
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = _configuration.ValidateIssuer,
                ValidateAudience = _configuration.ValidateAudience,
                ValidateLifetime = _configuration.ValidateLifetime,
                ValidateIssuerSigningKey = _configuration.ValidateIssuerSigningKey,
                ValidIssuer = _configuration.Issuer,
                ValidAudience = _configuration.Audience,
                IssuerSigningKey = key,
                ClockSkew = _configuration.ClockSkew,
                RequireExpirationTime = _configuration.RequireExpirationTime
            };

            var principal = _tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (SecurityTokenException ex)
        {
            LogJwtTokenValidationWarning(_logger, ex);
            return null;
        }
        catch (ArgumentException ex)
        {
            LogJwtTokenValidationWarning(_logger, ex);
            return null;
        }
        catch (InvalidOperationException ex)
        {
            LogJwtTokenValidationWarning(_logger, ex);
            return null;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        try
        {
            var bytes = Convert.FromBase64String(refreshToken);
            return bytes.Length == 32;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public DateTime GetTokenExpiration(string token)
    {
        ArgumentNullException.ThrowIfNull(token);
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch (SecurityTokenException ex)
        {
            LogJwtTokenExpirationError(_logger, ex);
            return DateTime.MinValue;
        }
        catch (ArgumentException ex)
        {
            LogJwtTokenExpirationError(_logger, ex);
            return DateTime.MinValue;
        }
    }

    public string? GetClaimValue(string token, string claimType)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(claimType);
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
        catch (SecurityTokenException ex)
        {
            LogJwtClaimValueError(_logger, ex);
            return null;
        }
        catch (ArgumentException ex)
        {
            LogJwtClaimValueError(_logger, ex);
            return null;
        }
    }

    public bool IsTokenExpired(string token)
    {
        ArgumentNullException.ThrowIfNull(token);
        var expiration = GetTokenExpiration(token);
        return expiration < DateTime.UtcNow;
    }

    public string? GetUserIdFromToken(string token)
    {
        ArgumentNullException.ThrowIfNull(token);
        return GetClaimValue(token, ClaimTypes.NameIdentifier);
    }

    public IEnumerable<string> GetRolesFromToken(string token)
    {
        ArgumentNullException.ThrowIfNull(token);
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
        }
        catch (SecurityTokenException ex)
        {
            LogJwtRolesError(_logger, ex);
            return Enumerable.Empty<string>();
        }
        catch (ArgumentException ex)
        {
            LogJwtRolesError(_logger, ex);
            return Enumerable.Empty<string>();
        }
    }

    // LoggerMessage delegates for performance and code analysis compliance
    private static readonly Action<ILogger, Exception?> LogJwtTokenGenerationError =
        LoggerMessage.Define(LogLevel.Error, new EventId(2000, "JwtTokenGenerationError"), "Error generating JWT token");

    private static readonly Action<ILogger, Exception?> LogJwtTokenValidationWarning =
        LoggerMessage.Define(LogLevel.Warning, new EventId(2001, "JwtTokenValidationWarning"), "Token validation failed");

    private static readonly Action<ILogger, Exception?> LogJwtTokenExpirationError =
        LoggerMessage.Define(LogLevel.Error, new EventId(2002, "JwtTokenExpirationError"), "Error getting token expiration");

    private static readonly Action<ILogger, Exception?> LogJwtClaimValueError =
        LoggerMessage.Define(LogLevel.Error, new EventId(2003, "JwtClaimValueError"), "Error getting claim value from token");

    private static readonly Action<ILogger, Exception?> LogJwtRolesError =
        LoggerMessage.Define(LogLevel.Error, new EventId(2004, "JwtRolesError"), "Error getting roles from token");
}