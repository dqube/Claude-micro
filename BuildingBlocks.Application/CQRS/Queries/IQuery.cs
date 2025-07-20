namespace BuildingBlocks.Application.CQRS.Queries;

public interface IQuery<out TResult>
{
    Guid Id { get; }
    DateTime Timestamp { get; }
}