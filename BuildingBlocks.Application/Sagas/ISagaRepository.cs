namespace BuildingBlocks.Application.Sagas;

public interface ISagaRepository<TSaga, TData>
    where TSaga : class, ISaga<TData>
    where TData : class
{
    Task SaveAsync(TSaga saga, CancellationToken cancellationToken = default);
    Task<TSaga?> GetAsync(Guid sagaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TSaga>> GetByStatusAsync(SagaStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TSaga>> GetExpiredSagasAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid sagaId, CancellationToken cancellationToken = default);
}