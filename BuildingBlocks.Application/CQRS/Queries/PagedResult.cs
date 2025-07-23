namespace BuildingBlocks.Application.CQRS.Queries;

public class PagedResult<T>
{
    public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList().AsReadOnly();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        HasNextPage = page < TotalPages;
        HasPreviousPage = page > 1;
    }

    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }
}

public static class PagedResult
{
    public static PagedResult<T> Empty<T>(int page = 1, int pageSize = 10) =>
        new PagedResult<T>(Enumerable.Empty<T>(), 0, page, pageSize);
}