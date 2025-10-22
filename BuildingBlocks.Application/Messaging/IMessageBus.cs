using BuildingBlocks.Application.CQRS.Messages;

namespace BuildingBlocks.Application.Messaging;

/// <summary>
/// Unified message bus interface for publishing and subscribing to messages.
/// Provides comprehensive messaging capabilities including metadata, batching, and scheduling.
/// </summary>
public interface IMessageBus
{
    // ============================================
    // Basic Publishing
    // ============================================
    
    /// <summary>
    /// Publish a message to the default topic (derived from message type)
    /// </summary>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) 
        where T : class, IMessage;
    
    /// <summary>
    /// Publish a message to a specific topic
    /// </summary>
    Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) 
        where T : class, IMessage;

    // ============================================
    // Publishing with Metadata
    // ============================================
    
    /// <summary>
    /// Publish a message with rich metadata (correlation ID, trace ID, headers, etc.)
    /// </summary>
    Task PublishAsync<T>(T message, MessageEnvelop metadata, CancellationToken cancellationToken = default) 
        where T : class, IMessage;
    
    /// <summary>
    /// Publish a message to a specific topic with metadata
    /// </summary>
    Task PublishAsync<T>(T message, string topic, MessageEnvelop metadata, CancellationToken cancellationToken = default) 
        where T : class, IMessage;

    // ============================================
    // Batch Publishing
    // ============================================
    
    /// <summary>
    /// Publish multiple messages efficiently (uses Kafka batching internally)
    /// </summary>
    Task PublishBatchAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default) 
        where T : class, IMessage;
    
    /// <summary>
    /// Publish multiple messages to a specific topic
    /// </summary>
    Task PublishBatchAsync<T>(IEnumerable<T> messages, string topic, CancellationToken cancellationToken = default) 
        where T : class, IMessage;

    // ============================================
    // Scheduled Publishing
    // ============================================
    
    /// <summary>
    /// Schedule a message to be published at a specific time (useful for delayed processing)
    /// </summary>
    Task ScheduleAsync<T>(T message, DateTime scheduledTime, CancellationToken cancellationToken = default) 
        where T : class, IMessage;
    
    /// <summary>
    /// Schedule a message to a specific topic
    /// </summary>
    Task ScheduleAsync<T>(T message, string topic, DateTime scheduledTime, CancellationToken cancellationToken = default) 
        where T : class, IMessage;

    // ============================================
    // Basic Subscription
    // ============================================
    
    /// <summary>
    /// Subscribe to messages of a specific type on the default topic
    /// </summary>
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : class, IMessage;
    
    /// <summary>
    /// Subscribe to messages of a specific type on a specific topic
    /// </summary>
    Task SubscribeAsync<T>(string topic, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : class, IMessage;

    // ============================================
    // Subscription with Metadata
    // ============================================
    
    /// <summary>
    /// Subscribe to messages with access to metadata (for correlation, tracing, etc.)
    /// </summary>
    Task SubscribeAsync<T>(Func<T, MessageEnvelop, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : class, IMessage;
    
    /// <summary>
    /// Subscribe to a specific topic with metadata access
    /// </summary>
    Task SubscribeAsync<T>(string topic, Func<T, MessageEnvelop, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : class, IMessage;

    // ============================================
    // Subscription Management
    // ============================================
    
    /// <summary>
    /// Unsubscribe from a message type (stop consuming messages)
    /// </summary>
    Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default) 
        where T : class, IMessage;
    
    /// <summary>
    /// Unsubscribe from a specific topic
    /// </summary>
    Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);
}