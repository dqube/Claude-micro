using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BuildingBlocks.API.Authentication.JWT;

public class JwtBearerOptionsSetup : IConfigureOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;

    public JwtBearerOptionsSetup(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
    }

    public void Configure(JwtBearerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var jwtSettings = _configuration.GetSection("Authentication:Jwt");
        if (jwtSettings == null)
        {
            throw new InvalidOperationException("JWT settings section 'Authentication:Jwt' is missing in configuration");
        }

        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var secretKey = jwtSettings["SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is required in configuration");
        }
        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException("JWT Issuer is required in configuration");
        }
        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("JWT Audience is required in configuration");
        }

        var key = Encoding.UTF8.GetBytes(secretKey);

        options.RequireHttpsMetadata = false; // Set to true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "You are not authorized to access this resource",
                    timestamp = DateTime.UtcNow
                });
                
                return context.Response.WriteAsync(result);
            }
        };
    }
}