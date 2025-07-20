namespace BuildingBlocks.Application.Inbox;

public interface IInboxProcessor
{
    Task ProcessPendingMessagesAsync(CancellationToken cancellationToken = default);
    Task ProcessMessageAsync(InboxMessage message, CancellationToken cancellationToken = default);
    Task RetryFailedMessagesAsync(CancellationToken cancellationToken = default);
    Task CleanupExpiredMessagesAsync(CancellationToken cancellationToken = default);
}