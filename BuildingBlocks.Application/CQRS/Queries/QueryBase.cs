namespace BuildingBlocks.Application.CQRS.Queries;

public abstract class QueryBase<TResult> : IQuery<TResult>
{
    protected QueryBase()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public DateTime Timestamp { get; }
}