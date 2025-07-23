using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Middleware.Security;
using BuildingBlocks.API.Utilities.Constants;

namespace BuildingBlocks.API.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddApiSecurity(this IServiceCollection services)
    {
        // Note: Middleware classes should not be registered as services in DI container
        // They are instantiated directly by the middleware pipeline when UseMiddleware<T>() is called
        
        return services;
    }

    public static WebApplication UseApiSecurity(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseApiSecurityHeaders();
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication UseApiSecurityHeaders(this WebApplication app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
        return app;
    }

    public static WebApplication UseCustomSecurityHeaders(this WebApplication app, Action<SecurityHeaderOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configure);
        
        var options = new SecurityHeaderOptions();
        configure(options);

        app.Use(async (context, next) =>
        {
            ApplySecurityHeaders(context, options);
            await next();
        });

        return app;
    }

    public static WebApplication UseStrictSecurityHeaders(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;
            
            // Strict security headers for production
            headers.TryAdd(HeaderConstants.Security.ContentTypeOptions, "nosniff");
            headers.TryAdd(HeaderConstants.Security.FrameOptions, "DENY");
            headers.TryAdd(HeaderConstants.Security.XssProtection, "1; mode=block");
            headers.TryAdd(HeaderConstants.Security.ReferrerPolicy, "strict-origin-when-cross-origin");
            headers.TryAdd(HeaderConstants.Security.ContentSecurityPolicy, "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self'; font-src 'self'");
            headers.TryAdd(HeaderConstants.Security.StrictTransportSecurity, "max-age=31536000; includeSubDomains");
            headers.TryAdd("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            
            // Remove server identification
            headers.Remove("Server");
            headers.Remove("X-Powered-By");
            headers.Remove("X-AspNet-Version");
            headers.Remove("X-AspNetMvc-Version");

            await next();
        });

        return app;
    }

    public static RouteHandlerBuilder RequireSecure(this RouteHandlerBuilder builder)
    {
        builder.RequireHost("https://*");
        return builder;
    }

    public static RouteGroupBuilder RequireSecure(this RouteGroupBuilder group)
    {
        group.RequireHost("https://*");
        return group;
    }

    public static RouteHandlerBuilder AllowAnonymous(this RouteHandlerBuilder builder)
    {
        builder.AllowAnonymous();
        return builder;
    }

    public static RouteGroupBuilder AllowAnonymous(this RouteGroupBuilder group)
    {
        group.AllowAnonymous();
        return group;
    }

    private static void ApplySecurityHeaders(HttpContext context, SecurityHeaderOptions options)
    {
        var headers = context.Response.Headers;

        if (options.EnableContentTypeOptions)
            headers.TryAdd(HeaderConstants.Security.ContentTypeOptions, "nosniff");

        if (options.EnableFrameOptions)
            headers.TryAdd(HeaderConstants.Security.FrameOptions, options.FrameOptionsValue);

        if (options.EnableXssProtection)
            headers.TryAdd(HeaderConstants.Security.XssProtection, options.XssProtectionValue);

        if (options.EnableReferrerPolicy)
            headers.TryAdd(HeaderConstants.Security.ReferrerPolicy, options.ReferrerPolicyValue);

        if (!string.IsNullOrEmpty(options.ContentSecurityPolicy))
            headers.TryAdd(HeaderConstants.Security.ContentSecurityPolicy, options.ContentSecurityPolicy);

        if (options.EnableHsts && !string.IsNullOrEmpty(options.HstsValue))
            headers.TryAdd(HeaderConstants.Security.StrictTransportSecurity, options.HstsValue);

        if (options.RemoveServerHeader)
        {
            headers.Remove("Server");
            headers.Remove("X-Powered-By");
            headers.Remove("X-AspNet-Version");
            headers.Remove("X-AspNetMvc-Version");
        }
    }
}

public class SecurityHeaderOptions
{
    public bool EnableContentTypeOptions { get; set; } = true;
    public bool EnableFrameOptions { get; set; } = true;
    public string FrameOptionsValue { get; set; } = "DENY";
    public bool EnableXssProtection { get; set; } = true;
    public string XssProtectionValue { get; set; } = "1; mode=block";
    public bool EnableReferrerPolicy { get; set; } = true;
    public string ReferrerPolicyValue { get; set; } = "strict-origin-when-cross-origin";
    public string ContentSecurityPolicy { get; set; } = "default-src 'self'";
    public bool EnableHsts { get; set; } = true;
    public string HstsValue { get; set; } = "max-age=31536000; includeSubDomains";
    public bool RemoveServerHeader { get; set; } = true;
}