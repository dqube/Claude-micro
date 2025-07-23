using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Outbox;

public partial class OutboxProcessor : IOutboxProcessor
{
    private readonly IOutboxService _outboxService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly OutboxOptions _options;

    [LoggerMessage(LogLevel.Error, "Failed to process outbox message {MessageId}")]
    private static partial void LogProcessingError(ILogger logger, Exception exception, Guid messageId);

    [LoggerMessage(LogLevel.Information, "Successfully published outbox message {MessageId} to {Destination}")]
    private static partial void LogMessagePublished(ILogger logger, Guid messageId, string destination);

    [LoggerMessage(LogLevel.Error, "Failed to publish outbox message {MessageId}: {Error}")]
    private static partial void LogPublishError(ILogger logger, Exception exception, Guid messageId, string error);

    [LoggerMessage(LogLevel.Error, "Failed to retry outbox message {MessageId}")]
    private static partial void LogRetryError(ILogger logger, Exception exception, Guid messageId);

    [LoggerMessage(LogLevel.Information, "Cleaned up expired outbox messages older than {RetentionPeriod}")]
    private static partial void LogCleanupCompleted(ILogger logger, TimeSpan retentionPeriod);

    public OutboxProcessor(
        IOutboxService outboxService,
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessor> logger,
        OutboxOptions options)
    {
        _outboxService = outboxService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options;
    }

    public async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _outboxService.GetReadyToPublishMessagesAsync(_options.BatchSize, cancellationToken);
        
        foreach (var message in messages)
        {
            try
            {
                await ProcessMessageAsync(message, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                LogProcessingError(_logger, ex, message.Id);
            }
            catch (TaskCanceledException ex)
            {
                LogProcessingError(_logger, ex, message.Id);
            }
            catch (TimeoutException ex)
            {
                LogProcessingError(_logger, ex, message.Id);
            }
        }
    }

    public async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        try
        {
            await _outboxService.MarkAsPublishingAsync(message.Id, cancellationToken);
            
            var publisher = GetMessagePublisher(message.Destination);
            if (publisher == null)
            {
                await _outboxService.MarkAsDiscardedAsync(message.Id, $"No publisher found for destination: {message.Destination}", cancellationToken);
                return;
            }

            await publisher.PublishAsync(message, cancellationToken);
            
            await _outboxService.MarkAsPublishedAsync(message.Id, cancellationToken);
            
            LogMessagePublished(_logger, message.Id, message.Destination);
        }
        catch (InvalidOperationException ex)
        {
            LogPublishError(_logger, ex, message.Id, ex.Message);
            await _outboxService.MarkAsFailedAsync(message.Id, ex.Message, ex.StackTrace, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            LogPublishError(_logger, ex, message.Id, ex.Message);
            await _outboxService.MarkAsFailedAsync(message.Id, ex.Message, ex.StackTrace, cancellationToken);
        }
        catch (TimeoutException ex)
        {
            LogPublishError(_logger, ex, message.Id, ex.Message);
            await _outboxService.MarkAsFailedAsync(message.Id, ex.Message, ex.StackTrace, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            LogPublishError(_logger, ex, message.Id, ex.Message);
            await _outboxService.MarkAsFailedAsync(message.Id, ex.Message, ex.StackTrace, cancellationToken);
        }
    }

    public async Task RetryFailedMessagesAsync(CancellationToken cancellationToken = default)
    {
        var failedMessages = await _outboxService.GetFailedMessagesAsync(_options.MaxRetries, cancellationToken);
        
        foreach (var message in failedMessages.Where(m => m.CanRetry(_options.MaxRetries)))
        {
            try
            {
                await ProcessMessageAsync(message, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                LogRetryError(_logger, ex, message.Id);
            }
            catch (TaskCanceledException ex)
            {
                LogRetryError(_logger, ex, message.Id);
            }
            catch (TimeoutException ex)
            {
                LogRetryError(_logger, ex, message.Id);
            }
        }
    }

    public async Task CleanupExpiredMessagesAsync(CancellationToken cancellationToken = default)
    {
        await _outboxService.CleanupOldMessagesAsync(_options.RetentionPeriod, cancellationToken);
        LogCleanupCompleted(_logger, _options.RetentionPeriod);
    }

    private IOutboxMessagePublisher? GetMessagePublisher(string destination)
    {
        var publishers = _serviceProvider.GetServices<IOutboxMessagePublisher>();
        return publishers.FirstOrDefault(p => p.CanPublish(destination));
    }
}

public class OutboxOptions
{
    public int BatchSize { get; set; } = 100;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan ProcessingInterval { get; set; } = TimeSpan.FromSeconds(30);
}