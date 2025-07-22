using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BuildingBlocks.API.Configuration.Options;
using System.Text;

namespace BuildingBlocks.API.Authentication.JWT;

public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authOptions;

    public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authOptions)
    {
        _authOptions = authOptions.Value;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            Configure(options);
        }
    }

    public void Configure(JwtBearerOptions options)
    {
        var jwtOptions = _authOptions.Jwt;

        options.SaveToken = jwtOptions.SaveToken;
        options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrEmpty(jwtOptions.Issuer),
            ValidateAudience = !string.IsNullOrEmpty(jwtOptions.Audience),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = !string.IsNullOrEmpty(jwtOptions.SecretKey),
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            ClockSkew = jwtOptions.ClockSkew
        };

        // Set signing key if provided
        if (!string.IsNullOrEmpty(jwtOptions.SecretKey))
        {
            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
            options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(key);
        }

        // Set authority if provided (for external identity providers)
        if (!string.IsNullOrEmpty(jwtOptions.Authority))
        {
            options.Authority = jwtOptions.Authority;
        }

        // Configure events for logging and custom handling
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception != null)
                {
                    context.Response.Headers.Add("Token-Error", context.Exception.Message);
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Add custom claims or logic here if needed
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
                    message = "Authentication required",
                    timestamp = DateTime.UtcNow
                });
                
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Access forbidden",
                    timestamp = DateTime.UtcNow
                });
                
                return context.Response.WriteAsync(result);
            }
        };
    }
}