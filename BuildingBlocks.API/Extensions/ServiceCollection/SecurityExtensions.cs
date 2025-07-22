using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Middleware.Security;
using BuildingBlocks.API.Configuration.Options;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class SecurityExtensions
{
    public static Microsoft.AspNetCore.Builder.WebApplication UseApiSecurity(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.UseApiSecurityHeaders();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseApiSecurityHeaders(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        var options = app.Services.GetService<IOptions<SecurityHeadersOptions>>()?.Value ?? new SecurityHeadersOptions();
        app.UseMiddleware<SecurityHeadersMiddleware>(options);
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseApiSecurityHeaders(this Microsoft.AspNetCore.Builder.WebApplication app, Action<SecurityHeadersOptions> configure)
    {
        var options = new SecurityHeadersOptions();
        configure(options);
        app.UseMiddleware<SecurityHeadersMiddleware>(options);
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseStrictSecurityHeaders(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        var options = new SecurityHeadersOptions
        {
            AddXFrameOptionsHeader = true,
            XFrameOptions = "DENY",
            AddXContentTypeOptionsHeader = true,
            AddReferrerPolicyHeader = true,
            ReferrerPolicy = "no-referrer",
            AddXXssProtectionHeader = true,
            AddStrictTransportSecurityHeader = true,
            StrictTransportSecurityMaxAge = TimeSpan.FromDays(365),
            StrictTransportSecurityIncludeSubdomains = true,
            RemoveServerHeader = true,
            AddContentSecurityPolicyHeader = true,
            ContentSecurityPolicy = "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; connect-src 'self'; frame-ancestors 'none';"
        };
        
        app.UseMiddleware<SecurityHeadersMiddleware>(options);
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseDevelopmentSecurityHeaders(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        var options = new SecurityHeadersOptions
        {
            AddXFrameOptionsHeader = true,
            XFrameOptions = "SAMEORIGIN",
            AddXContentTypeOptionsHeader = true,
            AddReferrerPolicyHeader = true,
            ReferrerPolicy = "strict-origin-when-cross-origin",
            AddXXssProtectionHeader = true,
            AddStrictTransportSecurityHeader = false, // Disabled for development
            RemoveServerHeader = true
        };
        
        app.UseMiddleware<SecurityHeadersMiddleware>(options);
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseProductionSecurityHeaders(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        return app.UseStrictSecurityHeaders();
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseContentSecurityPolicy(this Microsoft.AspNetCore.Builder.WebApplication app, string policy)
    {
        var options = new SecurityHeadersOptions
        {
            AddContentSecurityPolicyHeader = true,
            ContentSecurityPolicy = policy
        };
        
        app.UseMiddleware<SecurityHeadersMiddleware>(options);
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseApiAuthentication(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseForwardedHeaders(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                             Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto |
                             Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost
        });
        
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseHttpsRedirectionInProduction(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }
        
        return app;
    }
}