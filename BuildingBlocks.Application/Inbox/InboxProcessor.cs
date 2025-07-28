using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BuildingBlocks.Application.Inbox;

public partial class InboxProcessor : IInboxProcessor
{
    private readonly IInboxService _inboxService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxProcessor> _logger;
    private readonly InboxOptions _options;

    public InboxProcessor(
        IInboxService inboxService,
        IServiceProvider serviceProvider,
        ILogger<InboxProcessor> logger,
        InboxOptions options)
    {
        _inboxService = inboxService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options;
    }

    public async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _inboxService.GetPendingMessagesAsync(_options.BatchSize, cancellationToken);
        
        foreach (var message in messages)
        {
            try
            {
                await ProcessMessageAsync(message, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                LogFailedToProcessInboxMessage(_logger, ex, message.Id);
            }
            catch
            {
                throw;
            }
        }
    }

    public async Task ProcessMessageAsync(InboxMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        try
        {
            await _inboxService.MarkAsProcessingAsync(message.Id, cancellationToken);
            
            var handler = GetMessageHandler(message.MessageType);
            if (handler == null)
            {
                await _inboxService.MarkAsDiscardedAsync(message.Id, $"No handler found for message type: {message.MessageType}", cancellationToken);
                return;
            }

            var messageObject = DeserializeMessage(message.Payload, message.MessageType);
            await handler.HandleAsync(messageObject, cancellationToken);
            
            await _inboxService.MarkAsProcessedAsync(message.Id, cancellationToken);
            
            LogInboxMessageProcessedSuccessfully(_logger, message.Id, message.MessageType);
        }
        catch (InvalidOperationException ex)
        {
            LogFailedToProcessInboxMessageWithError(_logger, ex, message.Id, ex.Message);
            await _inboxService.MarkAsFailedAsync(message.Id, ex.Message, ex.StackTrace, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            LogFailedToProcessInboxMessageWithError(_logger, ex, message.Id, ex.Message);
            await _inboxService.MarkAsFailedAsync(message.Id, ex.Message, ex.StackTrace, cancellationToken);
        }
       
    }

    public async Task RetryFailedMessagesAsync(CancellationToken cancellationToken = default)
    {
        var failedMessages = await _inboxService.GetFailedMessagesAsync(_options.MaxRetries, cancellationToken);
        
        foreach (var message in failedMessages.Where(m => m.CanRetry(_options.MaxRetries)))
        {
            try
            {
                await ProcessMessageAsync(message, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                LogFailedToRetryInboxMessage(_logger, ex, message.Id);
            }
        }
    }

    public async Task CleanupExpiredMessagesAsync(CancellationToken cancellationToken = default)
    {
        await _inboxService.CleanupOldMessagesAsync(_options.RetentionPeriod, cancellationToken);
        LogCleanedUpExpiredInboxMessages(_logger, _options.RetentionPeriod);
    }

    private IInboxMessageHandler? GetMessageHandler(string messageType)
    {
        var handlers = _serviceProvider.GetServices<IInboxMessageHandler>();
        return handlers.FirstOrDefault(h => h.CanHandle(messageType));
    }

    private static object DeserializeMessage(string payload, string messageType)
    {
        // This would typically use a type registry to deserialize to the correct type
        return JsonSerializer.Deserialize<object>(payload) ?? new object();
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Failed to process inbox message {messageId}")]
    private static partial void LogFailedToProcessInboxMessage(ILogger logger, Exception exception, Guid messageId);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Successfully processed inbox message {messageId} of type {messageType}")]
    private static partial void LogInboxMessageProcessedSuccessfully(ILogger logger, Guid messageId, string messageType);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "Failed to process inbox message {messageId}: {error}")]
    private static partial void LogFailedToProcessInboxMessageWithError(ILogger logger, Exception exception, Guid messageId, string error);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "Failed to retry inbox message {messageId}")]
    private static partial void LogFailedToRetryInboxMessage(ILogger logger, Exception exception, Guid messageId);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Information,
        Message = "Cleaned up expired inbox messages older than {retentionPeriod}")]
    private static partial void LogCleanedUpExpiredInboxMessages(ILogger logger, TimeSpan retentionPeriod);
}

public class InboxOptions
{
    public int BatchSize { get; set; } = 100;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan ProcessingInterval { get; set; } = TimeSpan.FromSeconds(30);
}