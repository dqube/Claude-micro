using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Responses.Base;

public class ApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; set; }
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

public class ApiResponse<T> : ApiResponse
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}

public class PagedResponse<T> : ApiResponse<IEnumerable<T>>
{
    [JsonPropertyName("pagination")]
    public PaginationInfo Pagination { get; set; } = new();
}

public class PaginationInfo
{
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }
    
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
    
    [JsonPropertyName("totalCount")]
    public long TotalCount { get; set; }
    
    [JsonPropertyName("hasPrevious")]
    public bool HasPrevious => CurrentPage > 1;
    
    [JsonPropertyName("hasNext")]
    public bool HasNext => CurrentPage < TotalPages;
}

public class ErrorResponse : ApiResponse
{
    [JsonPropertyName("errors")]
    public IDictionary<string, object>? Errors { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("details")]
    public string? Details { get; set; }
}

public class ValidationErrorResponse : ApiResponse
{
    [JsonPropertyName("validationErrors")]
    public IDictionary<string, string[]>? Errors { get; set; }
}