using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Responses.Base;

public class ErrorResponse : ApiResponse
{
    [JsonPropertyName("errors")]
    public IDictionary<string, object>? Errors { get; set; }
    
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
    
    [JsonPropertyName("instance")]
    public string? Instance { get; set; }
}

public class ValidationErrorResponse : ApiResponse
{
    [JsonPropertyName("validationErrors")]
    public IDictionary<string, string[]>? Errors { get; set; }
}