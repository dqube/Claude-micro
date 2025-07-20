namespace BuildingBlocks.Application.Sagas;

public interface ISaga<TData> where TData : class
{
    Guid Id { get; }
    string Name { get; }
    SagaStatus Status { get; }
    TData Data { get; }
    DateTime CreatedAt { get; }
    DateTime? CompletedAt { get; }
    DateTime? LastUpdatedAt { get; }
    IReadOnlyList<SagaStep> Steps { get; }
    
    Task StartAsync(CancellationToken cancellationToken = default);
    Task CompleteAsync(CancellationToken cancellationToken = default);
    Task CompensateAsync(CancellationToken cancellationToken = default);
    Task FailAsync(string reason, CancellationToken cancellationToken = default);
}

public enum SagaStatus
{
    NotStarted,
    Running,
    Completed,
    Compensating,
    Compensated,
    Failed
}