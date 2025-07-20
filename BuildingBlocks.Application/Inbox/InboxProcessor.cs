using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BuildingBlocks.Application.Inbox;

public class InboxProcessor : IInboxProcessor
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process inbox message {MessageId}", message.Id);
            }
        }
    }

    public async Task ProcessMessageAsync(InboxMessage message, CancellationToken cancellationToken = default)
    {
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
            
            _logger.LogInformation("Successfully processed inbox message {MessageId} of type {MessageType}", message.Id, message.MessageType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process inbox message {MessageId}: {Error}", message.Id, ex.Message);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retry inbox message {MessageId}", message.Id);
            }
        }
    }

    public async Task CleanupExpiredMessagesAsync(CancellationToken cancellationToken = default)
    {
        await _inboxService.CleanupOldMessagesAsync(_options.RetentionPeriod, cancellationToken);
        _logger.LogInformation("Cleaned up expired inbox messages older than {RetentionPeriod}", _options.RetentionPeriod);
    }

    private IInboxMessageHandler? GetMessageHandler(string messageType)
    {
        var handlers = _serviceProvider.GetServices<IInboxMessageHandler>();
        return handlers.FirstOrDefault(h => h.CanHandle(messageType));
    }

    private object DeserializeMessage(string payload, string messageType)
    {
        // This would typically use a type registry to deserialize to the correct type
        return JsonSerializer.Deserialize<object>(payload) ?? new object();
    }
}

public class InboxOptions
{
    public int BatchSize { get; set; } = 100;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan ProcessingInterval { get; set; } = TimeSpan.FromSeconds(30);
}