using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Services;

public partial class InboxService : IInboxService
{
    private readonly IDbContext _context;
    private readonly ILogger<InboxService> _logger;

    public InboxService(IDbContext context, ILogger<InboxService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> TryAddMessageAsync(string messageId, string messageType, string payload, string source, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if message already exists
            var existingMessage = await _context.GetDbSet<InboxMessage>()
                .FirstOrDefaultAsync(m => m.MessageId == messageId, cancellationToken);

            if (existingMessage != null)
            {
                return false; // Message already exists
            }

            var inboxMessage = new InboxMessage(messageId, messageType, payload, source);
            _context.GetDbSet<InboxMessage>().Add(inboxMessage);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (DbUpdateException ex)
        {
            LogFailedToAddInboxMessage(_logger, ex, messageId);
            return false;
        }
        catch (InvalidOperationException ex)
        {
            LogFailedToAddInboxMessage(_logger, ex, messageId);
            return false;
        }
    }

    public async Task<InboxMessage?> GetNextPendingMessageAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GetDbSet<InboxMessage>()
            .Where(m => m.Status == InboxMessageStatus.Pending)
            .OrderBy(m => m.ReceivedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<InboxMessage>> GetPendingMessagesAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _context.GetDbSet<InboxMessage>()
            .Where(m => m.Status == InboxMessageStatus.Pending)
            .OrderBy(m => m.ReceivedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InboxMessage>> GetFailedMessagesAsync(int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        return await _context.GetDbSet<InboxMessage>()
            .Where(m => m.Status == InboxMessageStatus.Failed && m.RetryCount < maxRetries)
            .OrderBy(m => m.LastAttemptAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InboxMessage>> GetExpiredMessagesAsync(TimeSpan maxAge, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow - maxAge;
        return await _context.GetDbSet<InboxMessage>()
            .Where(m => m.ReceivedAt < cutoffDate && 
                       (m.Status == InboxMessageStatus.Processed || 
                        m.Status == InboxMessageStatus.Discarded ||
                        m.Status == InboxMessageStatus.Failed))
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessingAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.GetDbSet<InboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.MarkAsProcessing();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.GetDbSet<InboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.MarkAsProcessed();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid messageId, string errorMessage, string? stackTrace = null, CancellationToken cancellationToken = default)
    {
        var message = await _context.GetDbSet<InboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.MarkAsFailed(errorMessage, stackTrace);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsDiscardedAsync(Guid messageId, string reason, CancellationToken cancellationToken = default)
    {
        var message = await _context.GetDbSet<InboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.MarkAsDiscarded(reason);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> IsMessageProcessedAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return await _context.GetDbSet<InboxMessage>()
            .AnyAsync(m => m.MessageId == messageId && m.Status == InboxMessageStatus.Processed, cancellationToken);
    }

    public async Task CleanupOldMessagesAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow - retentionPeriod;
        var expiredMessages = await _context.GetDbSet<InboxMessage>()
            .Where(m => m.ReceivedAt < cutoffDate && 
                       (m.Status == InboxMessageStatus.Processed || 
                        m.Status == InboxMessageStatus.Discarded))
            .ToListAsync(cancellationToken);

        if (expiredMessages.Count != 0)
        {
            _context.GetDbSet<InboxMessage>().RemoveRange(expiredMessages);
            await _context.SaveChangesAsync(cancellationToken);
            
            LogCleanedUpExpiredInboxMessages(_logger, expiredMessages.Count);
        }
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Failed to add inbox message {MessageId}")]
    private static partial void LogFailedToAddInboxMessage(ILogger logger, Exception exception, string messageId);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Cleaned up {Count} expired inbox messages")]
    private static partial void LogCleanedUpExpiredInboxMessages(ILogger logger, int count);
}