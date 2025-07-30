using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using InventoryService.Infrastructure.Persistence;

namespace InventoryService.Infrastructure.Services;

public class OutboxService : IOutboxService
{
    private readonly InventoryDbContext _context;

    public OutboxService(InventoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddMessageAsync(string messageType, string payload, string destination, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        var message = new OutboxMessage(messageType, payload, destination, correlationId);
        await _context.OutboxMessages.AddAsync(message, cancellationToken);
    }

    public async Task ScheduleMessageAsync(string messageType, string payload, string destination, DateTime scheduledAt, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        var message = new OutboxMessage(messageType, payload, destination, correlationId);
        message.Schedule(scheduledAt);
        await _context.OutboxMessages.AddAsync(message, cancellationToken);
    }

    public async Task<OutboxMessage?> GetNextPendingMessageAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Pending)
            .OrderBy(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Pending)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetReadyToPublishMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Pending || 
                       (m.Status == OutboxMessageStatus.Scheduled && m.ScheduledAt <= now))
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetFailedMessagesAsync(int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Failed && m.RetryCount < maxRetries)
            .OrderBy(m => m.LastAttemptAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetExpiredMessagesAsync(TimeSpan maxAge, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow - maxAge;
        return await _context.OutboxMessages
            .Where(m => m.CreatedAt < cutoffDate && 
                       (m.Status == OutboxMessageStatus.Failed || m.Status == OutboxMessageStatus.Discarded))
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsPublishingAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
        
        if (message != null)
        {
            message.MarkAsPublishing();
        }
    }

    public async Task MarkAsPublishedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
        
        if (message != null)
        {
            message.MarkAsPublished();
        }
    }

    public async Task MarkAsFailedAsync(Guid messageId, string errorMessage, string? stackTrace = null, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
        
        if (message != null)
        {
            message.MarkAsFailed(errorMessage, stackTrace);
        }
    }

    public async Task MarkAsDiscardedAsync(Guid messageId, string reason, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
        
        if (message != null)
        {
            message.MarkAsDiscarded(reason);
        }
    }

    public async Task CleanupOldMessagesAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow - retentionPeriod;
        var oldMessages = await _context.OutboxMessages
            .Where(m => m.CreatedAt < cutoffDate && 
                       (m.Status == OutboxMessageStatus.Published || 
                        m.Status == OutboxMessageStatus.Discarded))
            .ToListAsync(cancellationToken);

        if (oldMessages.Count != 0)
        {
            _context.OutboxMessages.RemoveRange(oldMessages);
        }
    }
}