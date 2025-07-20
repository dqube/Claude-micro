namespace BuildingBlocks.Application.Inbox;

public interface IInboxService
{
    Task<bool> TryAddMessageAsync(string messageId, string messageType, string payload, string source, CancellationToken cancellationToken = default);
    Task<InboxMessage?> GetNextPendingMessageAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<InboxMessage>> GetFailedMessagesAsync(int maxRetries = 3, CancellationToken cancellationToken = default);
    Task<IEnumerable<InboxMessage>> GetExpiredMessagesAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
    Task MarkAsProcessingAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid messageId, string errorMessage, string? stackTrace = null, CancellationToken cancellationToken = default);
    Task MarkAsDiscardedAsync(Guid messageId, string reason, CancellationToken cancellationToken = default);
    Task<bool> IsMessageProcessedAsync(string messageId, CancellationToken cancellationToken = default);
    Task CleanupOldMessagesAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default);
}