namespace BuildingBlocks.API.Utilities.Constants;

public static class ApiConstants
{
    public static class Versions
    {
        public const string V1 = "1.0";
        public const string V2 = "2.0";
        public const string Latest = V2;
    }

    public static class Routes
    {
        public const string ApiPrefix = "api";
        public const string VersionPrefix = "v{version:apiVersion}";
        public const string HealthCheck = "health";
        public const string Ready = "health/ready";
        public const string Live = "health/live";
    }

    public static class PolicyNames
    {
        public const string DefaultCors = "DefaultCorsPolicy";
        public const string DefaultRateLimit = "DefaultRateLimitPolicy";
        public const string DefaultAuth = "DefaultAuthPolicy";
    }

    public static class ClaimTypes
    {
        public const string UserId = "user_id";
        public const string Email = "email";
        public const string Role = "role";
        public const string Permission = "permission";
        public const string TenantId = "tenant_id";
    }

    public static class Scopes
    {
        public const string Read = "read";
        public const string Write = "write";
        public const string Delete = "delete";
        public const string Admin = "admin";
    }

    public static class CacheKeys
    {
        public const string UserPrefix = "user:";
        public const string SessionPrefix = "session:";
        public const string RateLimitPrefix = "ratelimit:";
    }

    public static class DefaultValues
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
        public const int DefaultTimeout = 30; // seconds
        public const string DefaultCulture = "en-US";
    }

    public static class Messages
    {
        public const string Success = "Operation completed successfully";
        public const string Created = "Resource created successfully";
        public const string Updated = "Resource updated successfully";
        public const string Deleted = "Resource deleted successfully";
        public const string NotFound = "Resource not found";
        public const string Unauthorized = "Unauthorized access";
        public const string Forbidden = "Access forbidden";
        public const string ValidationFailed = "Validation failed";
        public const string InternalError = "An internal server error occurred";
        public const string RateLimitExceeded = "Rate limit exceeded";
    }
}