using System.Text.Json.Serialization;

namespace BuildingBlocks.API.Responses.Base;

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