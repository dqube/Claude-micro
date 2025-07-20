namespace BuildingBlocks.Application.Outbox;

public interface IOutboxService
{
    Task AddMessageAsync(string messageType, string payload, string destination, string? correlationId = null, CancellationToken cancellationToken = default);
    Task ScheduleMessageAsync(string messageType, string payload, string destination, DateTime scheduledAt, string? correlationId = null, CancellationToken cancellationToken = default);
    Task<OutboxMessage?> GetNextPendingMessageAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxMessage>> GetReadyToPublishMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxMessage>> GetFailedMessagesAsync(int maxRetries = 3, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxMessage>> GetExpiredMessagesAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
    Task MarkAsPublishingAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsPublishedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid messageId, string errorMessage, string? stackTrace = null, CancellationToken cancellationToken = default);
    Task MarkAsDiscardedAsync(Guid messageId, string reason, CancellationToken cancellationToken = default);
    Task CleanupOldMessagesAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default);
}