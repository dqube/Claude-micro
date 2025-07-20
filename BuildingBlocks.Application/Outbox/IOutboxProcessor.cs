namespace BuildingBlocks.Application.Outbox;

public interface IOutboxProcessor
{
    Task ProcessPendingMessagesAsync(CancellationToken cancellationToken = default);
    Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task RetryFailedMessagesAsync(CancellationToken cancellationToken = default);
    Task CleanupExpiredMessagesAsync(CancellationToken cancellationToken = default);
}

public interface IOutboxMessagePublisher
{
    Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    bool CanPublish(string destination);
}