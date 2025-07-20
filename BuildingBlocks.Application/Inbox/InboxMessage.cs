namespace BuildingBlocks.Application.Inbox;

public class InboxMessage
{
    public InboxMessage(string messageId, string messageType, string payload, string source)
    {
        Id = Guid.NewGuid();
        MessageId = messageId;
        MessageType = messageType;
        Payload = payload;
        Source = source;
        Status = InboxMessageStatus.Pending;
        ReceivedAt = DateTime.UtcNow;
        RetryCount = 0;
    }

    private InboxMessage() { } // For EF Core

    public Guid Id { get; private set; }
    public string MessageId { get; private set; } = string.Empty;
    public string MessageType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public string Source { get; private set; } = string.Empty;
    public InboxMessageStatus Status { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? StackTrace { get; private set; }

    public void MarkAsProcessing()
    {
        Status = InboxMessageStatus.Processing;
        LastAttemptAt = DateTime.UtcNow;
    }

    public void MarkAsProcessed()
    {
        Status = InboxMessageStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        ErrorMessage = null;
        StackTrace = null;
    }

    public void MarkAsFailed(string errorMessage, string? stackTrace = null)
    {
        Status = InboxMessageStatus.Failed;
        RetryCount++;
        ErrorMessage = errorMessage;
        StackTrace = stackTrace;
        LastAttemptAt = DateTime.UtcNow;
    }

    public void MarkAsDiscarded(string reason)
    {
        Status = InboxMessageStatus.Discarded;
        ErrorMessage = reason;
        LastAttemptAt = DateTime.UtcNow;
    }

    public bool CanRetry(int maxRetries = 3)
    {
        return Status == InboxMessageStatus.Failed && RetryCount < maxRetries;
    }

    public bool IsExpired(TimeSpan maxAge)
    {
        return DateTime.UtcNow - ReceivedAt > maxAge;
    }
}