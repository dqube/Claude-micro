using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Infrastructure.Data.Context;

namespace BuildingBlocks.Infrastructure.Services;

public partial class OutboxService : IOutboxService
{
    private readonly IDbContext _context;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(IDbContext context, ILogger<OutboxService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddMessageAsync(string messageType, string payload, string destination, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        var message = new OutboxMessage(messageType, payload, destination, correlationId);
        _context.GetDbSet<OutboxMessage>().Add(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ScheduleMessageAsync(string messageType, string payload, string destination, DateTime scheduledAt, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        var message = new OutboxMessage(messageType, payload, destination, correlationId);
        message.Schedule(scheduledAt);
        _context.GetDbSet<OutboxMessage>().Add(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<OutboxMessage?> GetNextPendingMessageAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GetDbSet<OutboxMessage>()
            .Where(m => m.Status == OutboxMessageStatus.Pending)
            .OrderBy(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _context.GetDbSet<OutboxMessage>()
            .Where(m => m.Status == OutboxMessageStatus.Pending)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetReadyToPublishMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.GetDbSet<OutboxMessage>()
            .Where(m => m.Status == OutboxMessageStatus.Pending || 
                       (m.Status == OutboxMessageStatus.Scheduled && m.ScheduledAt <= now))
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesAsync(int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        return await _context.GetDbSet<OutboxMessage>()
            .Where(m => m.Status == OutboxMessageStatus.Failed && m.RetryCount < maxRetries)
            .OrderBy(m => m.LastAttemptAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetExpiredMessagesAsync(TimeSpan maxAge, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow - maxAge;
        return await _context.GetDbSet<OutboxMessage>()
            .Where(m => m.CreatedAt < cutoffDate && 
                       (m.Status == OutboxMessageStatus.Published || 
                        m.Status == OutboxMessageStatus.Discarded ||
                        m.Status == OutboxMessageStatus.Failed))
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsPublishingAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.GetDbSet<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.MarkAsPublishing();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsPublishedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.GetDbSet<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.MarkAsPublished();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid messageId, string errorMessage, string? stackTrace = null, CancellationToken cancellationToken = default)
    {
        var message = await _context.GetDbSet<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.MarkAsFailed(errorMessage, stackTrace);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsDiscardedAsync(Guid messageId, string reason, CancellationToken cancellationToken = default)
    {
        var message = await _context.GetDbSet<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.MarkAsDiscarded(reason);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task CleanupOldMessagesAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow - retentionPeriod;
        var expiredMessages = await _context.GetDbSet<OutboxMessage>()
            .Where(m => m.CreatedAt < cutoffDate && 
                       (m.Status == OutboxMessageStatus.Published || 
                        m.Status == OutboxMessageStatus.Discarded))
            .ToListAsync(cancellationToken);

        if (expiredMessages.Count > 0)
        {
            _context.GetDbSet<OutboxMessage>().RemoveRange(expiredMessages);
            await _context.SaveChangesAsync(cancellationToken);
            
            LogCleanedUpExpiredOutboxMessages(_logger, expiredMessages.Count);
        }
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Cleaned up {Count} expired outbox messages")]
    private static partial void LogCleanedUpExpiredOutboxMessages(ILogger logger, int count);
}