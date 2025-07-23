namespace BuildingBlocks.API.Utilities.Constants;

public static class ApiConstants
{
    public const string DefaultCorrelationId = "unknown";
    public const string DefaultSuccessMessage = "Operation completed successfully";
    public const string DefaultErrorMessage = "An error occurred while processing the request";
    
    internal static class StatusMessages
    {
        public const string Created = "Resource created successfully";
        public const string Updated = "Resource updated successfully";
        public const string Deleted = "Resource deleted successfully";
        public const string NotFound = "Resource not found";
        public const string Unauthorized = "Unauthorized access";
        public const string Forbidden = "Access forbidden";
        public const string BadRequest = "Invalid request";
        public const string Conflict = "Resource conflict";
        public const string ValidationFailed = "Validation failed";
    }

    internal static class DefaultPageSize
    {
        public const int Small = 10;
        public const int Medium = 25;
        public const int Large = 50;
        public const int Maximum = 100;
    }
}