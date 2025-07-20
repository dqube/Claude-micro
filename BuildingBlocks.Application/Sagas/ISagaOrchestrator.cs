namespace BuildingBlocks.Application.Sagas;

public interface ISagaOrchestrator
{
    Task<TSaga> StartSagaAsync<TSaga, TData>(TData data, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga<TData>
        where TData : class;
    
    Task<TSaga?> GetSagaAsync<TSaga, TData>(Guid sagaId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga<TData>
        where TData : class;
    
    Task CompensateSagaAsync<TSaga, TData>(Guid sagaId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga<TData>
        where TData : class;
    
    Task<IEnumerable<TSaga>> GetSagasByStatusAsync<TSaga, TData>(SagaStatus status, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga<TData>
        where TData : class;
}