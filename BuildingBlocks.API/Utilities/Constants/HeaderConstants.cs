namespace BuildingBlocks.API.Utilities.Constants;

public static class HeaderConstants
{
    public const string CorrelationId = "X-Correlation-ID";
    public const string RequestId = "X-Request-ID";
    public const string ApiKey = "X-API-Key";
    public const string ClientId = "X-Client-ID";
    public const string UserAgent = "User-Agent";
    public const string ContentType = "Content-Type";
    public const string Authorization = "Authorization";
    
    public static class Security
    {
        public const string ContentTypeOptions = "X-Content-Type-Options";
        public const string FrameOptions = "X-Frame-Options";
        public const string XssProtection = "X-XSS-Protection";
        public const string ReferrerPolicy = "Referrer-Policy";
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        public const string StrictTransportSecurity = "Strict-Transport-Security";
    }
    
    public static class RateLimit
    {
        public const string RetryAfter = "Retry-After";
        public const string RateLimitRemaining = "X-RateLimit-Remaining";
        public const string RateLimitLimit = "X-RateLimit-Limit";
        public const string RateLimitReset = "X-RateLimit-Reset";
    }
}