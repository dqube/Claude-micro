namespace BuildingBlocks.API.Utilities.Constants;

public static class HeaderConstants
{
    // Standard Headers
    public const string Authorization = "Authorization";
    public const string ContentType = "Content-Type";
    public const string Accept = "Accept";
    public const string AcceptEncoding = "Accept-Encoding";
    public const string AcceptLanguage = "Accept-Language";
    public const string UserAgent = "User-Agent";
    public const string Referer = "Referer";
    public const string Origin = "Origin";
    public const string Host = "Host";
    public const string CacheControl = "Cache-Control";
    public const string ETag = "ETag";
    public const string IfNoneMatch = "If-None-Match";
    public const string LastModified = "Last-Modified";
    public const string IfModifiedSince = "If-Modified-Since";
    public const string Location = "Location";

    // Custom API Headers
    public const string CorrelationId = "X-Correlation-ID";
    public const string RequestId = "X-Request-ID";
    public const string TraceId = "X-Trace-ID";
    public const string SpanId = "X-Span-ID";
    public const string ApiVersion = "X-API-Version";
    public const string ClientId = "X-Client-ID";
    public const string ClientVersion = "X-Client-Version";
    public const string Feature = "X-Feature";
    public const string Environment = "X-Environment";

    // Rate Limiting Headers
    public const string RateLimitLimit = "X-RateLimit-Limit";
    public const string RateLimitRemaining = "X-RateLimit-Remaining";
    public const string RateLimitReset = "X-RateLimit-Reset";
    public const string RetryAfter = "Retry-After";

    // Security Headers
    public const string StrictTransportSecurity = "Strict-Transport-Security";
    public const string ContentSecurityPolicy = "Content-Security-Policy";
    public const string XFrameOptions = "X-Frame-Options";
    public const string XContentTypeOptions = "X-Content-Type-Options";
    public const string XXssProtection = "X-XSS-Protection";
    public const string ReferrerPolicy = "Referrer-Policy";
    public const string PermissionsPolicy = "Permissions-Policy";
    public const string Server = "Server";

    // CORS Headers
    public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
    public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
    public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
    public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
    public const string AccessControlMaxAge = "Access-Control-Max-Age";
    public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
    public const string AccessControlRequestMethod = "Access-Control-Request-Method";
    public const string AccessControlRequestHeaders = "Access-Control-Request-Headers";

    // Forwarded Headers (for proxy scenarios)
    public const string XForwardedFor = "X-Forwarded-For";
    public const string XForwardedHost = "X-Forwarded-Host";
    public const string XForwardedProto = "X-Forwarded-Proto";
    public const string XRealIp = "X-Real-IP";
    public const string Forwarded = "Forwarded";

    // API Specific Headers
    public const string ApiKey = "X-API-Key";
    public const string ApiDeprecated = "X-API-Deprecated";
    public const string ApiDeprecationInfo = "X-API-Deprecation-Info";
    public const string ApiSupported = "X-API-Supported-Versions";
    public const string TotalCount = "X-Total-Count";
    public const string PageCount = "X-Page-Count";
    public const string CurrentPage = "X-Current-Page";
    public const string PageSize = "X-Page-Size";

    // Health Check Headers
    public const string HealthStatus = "X-Health-Status";
    public const string HealthDuration = "X-Health-Duration";

    // Common Header Values
    public static class Values
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationXml = "application/xml";
        public const string TextPlain = "text/plain";
        public const string Bearer = "Bearer";
        public const string Basic = "Basic";
        public const string ApiKey = "ApiKey";
        public const string NoCache = "no-cache";
        public const string NoStore = "no-store";
    }
}