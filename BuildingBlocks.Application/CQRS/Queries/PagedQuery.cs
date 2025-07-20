namespace BuildingBlocks.Application.CQRS.Queries;

public abstract class PagedQuery<TResult> : QueryBase<PagedResult<TResult>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;
}