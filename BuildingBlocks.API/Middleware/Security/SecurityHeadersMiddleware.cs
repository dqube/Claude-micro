using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Middleware.Security;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersOptions? options = null)
    {
        _next = next;
        _options = options ?? new SecurityHeadersOptions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers
        AddSecurityHeaders(context.Response);

        await _next(context);
    }

    private void AddSecurityHeaders(HttpResponse response)
    {
        if (_options.AddXFrameOptionsHeader && !response.Headers.ContainsKey("X-Frame-Options"))
        {
            response.Headers.Add("X-Frame-Options", _options.XFrameOptions);
        }

        if (_options.AddXContentTypeOptionsHeader && !response.Headers.ContainsKey("X-Content-Type-Options"))
        {
            response.Headers.Add("X-Content-Type-Options", "nosniff");
        }

        if (_options.AddReferrerPolicyHeader && !response.Headers.ContainsKey("Referrer-Policy"))
        {
            response.Headers.Add("Referrer-Policy", _options.ReferrerPolicy);
        }

        if (_options.AddXXssProtectionHeader && !response.Headers.ContainsKey("X-XSS-Protection"))
        {
            response.Headers.Add("X-XSS-Protection", "1; mode=block");
        }

        if (_options.AddStrictTransportSecurityHeader && 
            !response.Headers.ContainsKey("Strict-Transport-Security") &&
            _options.StrictTransportSecurityMaxAge.HasValue)
        {
            var hstsValue = $"max-age={_options.StrictTransportSecurityMaxAge.Value.TotalSeconds:F0}";
            if (_options.StrictTransportSecurityIncludeSubdomains)
            {
                hstsValue += "; includeSubDomains";
            }
            if (_options.StrictTransportSecurityPreload)
            {
                hstsValue += "; preload";
            }
            response.Headers.Add("Strict-Transport-Security", hstsValue);
        }

        if (_options.AddContentSecurityPolicyHeader && 
            !string.IsNullOrEmpty(_options.ContentSecurityPolicy) &&
            !response.Headers.ContainsKey("Content-Security-Policy"))
        {
            response.Headers.Add("Content-Security-Policy", _options.ContentSecurityPolicy);
        }

        if (_options.AddPermissionsPolicyHeader && 
            !string.IsNullOrEmpty(_options.PermissionsPolicy) &&
            !response.Headers.ContainsKey("Permissions-Policy"))
        {
            response.Headers.Add("Permissions-Policy", _options.PermissionsPolicy);
        }

        // Remove server header if configured
        if (_options.RemoveServerHeader && response.Headers.ContainsKey("Server"))
        {
            response.Headers.Remove("Server");
        }
    }
}

public class SecurityHeadersOptions
{
    public bool AddXFrameOptionsHeader { get; set; } = true;
    public string XFrameOptions { get; set; } = "DENY";

    public bool AddXContentTypeOptionsHeader { get; set; } = true;

    public bool AddReferrerPolicyHeader { get; set; } = true;
    public string ReferrerPolicy { get; set; } = "strict-origin-when-cross-origin";

    public bool AddXXssProtectionHeader { get; set; } = true;

    public bool AddStrictTransportSecurityHeader { get; set; } = true;
    public TimeSpan? StrictTransportSecurityMaxAge { get; set; } = TimeSpan.FromDays(365);
    public bool StrictTransportSecurityIncludeSubdomains { get; set; } = true;
    public bool StrictTransportSecurityPreload { get; set; } = false;

    public bool AddContentSecurityPolicyHeader { get; set; } = false;
    public string? ContentSecurityPolicy { get; set; }

    public bool AddPermissionsPolicyHeader { get; set; } = false;
    public string? PermissionsPolicy { get; set; }

    public bool RemoveServerHeader { get; set; } = true;
}