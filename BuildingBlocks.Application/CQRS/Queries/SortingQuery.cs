namespace BuildingBlocks.Application.CQRS.Queries;

public enum SortDirection
{
    Ascending,
    Descending
}

public class SortingInfo
{
    public SortingInfo(string field, SortDirection direction = SortDirection.Ascending)
    {
        Field = field;
        Direction = direction;
    }

    public string Field { get; }
    public SortDirection Direction { get; }
}

public abstract class SortingQuery<TResult> : QueryBase<TResult>
{
    public List<SortingInfo> SortBy { get; set; } = [];

    public void AddSort(string field, SortDirection direction = SortDirection.Ascending)
    {
        SortBy.Add(new SortingInfo(field, direction));
    }
}