namespace BuildingBlocks.Application.Outbox;

public class OutboxMessage
{
    public OutboxMessage(string messageType, string payload, string destination, string? correlationId = null)
    {
        Id = Guid.NewGuid();
        MessageType = messageType;
        Payload = payload;
        Destination = destination;
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        Status = OutboxMessageStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        RetryCount = 0;
    }

    private OutboxMessage() { } // For EF Core

    public Guid Id { get; private set; }
    public string MessageType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public string CorrelationId { get; private set; } = string.Empty;
    public OutboxMessageStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? StackTrace { get; private set; }
    public DateTime? ScheduledAt { get; private set; }

    public void MarkAsPublishing()
    {
        Status = OutboxMessageStatus.Publishing;
        LastAttemptAt = DateTime.UtcNow;
    }

    public void MarkAsPublished()
    {
        Status = OutboxMessageStatus.Published;
        PublishedAt = DateTime.UtcNow;
        ErrorMessage = null;
        StackTrace = null;
    }

    public void MarkAsFailed(string errorMessage, string? stackTrace = null)
    {
        Status = OutboxMessageStatus.Failed;
        RetryCount++;
        ErrorMessage = errorMessage;
        StackTrace = stackTrace;
        LastAttemptAt = DateTime.UtcNow;
    }

    public void MarkAsDiscarded(string reason)
    {
        Status = OutboxMessageStatus.Discarded;
        ErrorMessage = reason;
        LastAttemptAt = DateTime.UtcNow;
    }

    public void Schedule(DateTime scheduledAt)
    {
        ScheduledAt = scheduledAt;
        Status = OutboxMessageStatus.Scheduled;
    }

    public bool CanRetry(int maxRetries = 3)
    {
        return Status == OutboxMessageStatus.Failed && RetryCount < maxRetries;
    }

    public bool IsReadyToPublish()
    {
        return Status == OutboxMessageStatus.Pending || 
               (Status == OutboxMessageStatus.Scheduled && ScheduledAt <= DateTime.UtcNow);
    }

    public bool IsExpired(TimeSpan maxAge)
    {
        return DateTime.UtcNow - CreatedAt > maxAge;
    }
}