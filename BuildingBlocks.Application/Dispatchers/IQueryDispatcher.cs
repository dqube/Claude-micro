using BuildingBlocks.Application.CQRS.Queries;

namespace BuildingBlocks.Application.Dispatchers;

public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}