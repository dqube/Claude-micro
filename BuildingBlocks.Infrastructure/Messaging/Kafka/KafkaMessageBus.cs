using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Infrastructure.Messaging.Configuration;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Infrastructure.Messaging.Kafka;

/// <summary>
/// Kafka-based implementation of IMessageBus from BuildingBlocks.Application.Messaging
/// </summary>
public sealed class KafkaMessageBus : IMessageBus, IDisposable
{
    private readonly ILogger<KafkaMessageBus> _logger;
    private readonly KafkaConfiguration _configuration;
    private readonly IProducer<string, byte[]> _producer;
    private readonly ConcurrentDictionary<string, ConsumerSubscription> _subscriptions = new();
    private readonly ConcurrentDictionary<Type, string> _topicCache = new();
    private bool _disposed;

    public KafkaMessageBus(
        ILogger<KafkaMessageBus> logger,
        IOptions<KafkaConfiguration> configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));

        // Configure producer
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _configuration.BootstrapServers,
            ClientId = _configuration.ClientId,
            Acks = (Acks)_configuration.RequiredAcks,
            MessageTimeoutMs = _configuration.MessageTimeoutMs,
            RequestTimeoutMs = _configuration.RequestTimeoutMs,
            CompressionType = ParseCompressionType(_configuration.CompressionType),
            EnableIdempotence = _configuration.EnableIdempotence,
            MaxInFlight = _configuration.MaxInFlight,
            RetryBackoffMs = _configuration.RetryBackoffMs,
            MessageMaxBytes = _configuration.MessageMaxBytes,
            SecurityProtocol = ParseSecurityProtocol(_configuration.SecurityProtocol),
        };

        // Add SASL configuration if specified
        if (!string.IsNullOrEmpty(_configuration.SaslMechanism))
        {
            producerConfig.SaslMechanism = ParseSaslMechanism(_configuration.SaslMechanism);
            producerConfig.SaslUsername = _configuration.SaslUsername;
            producerConfig.SaslPassword = _configuration.SaslPassword;
        }

        _producer = new ProducerBuilder<string, byte[]>(producerConfig).Build();

        _logger.LogInformation("KafkaMessageBus initialized with bootstrap servers: {BootstrapServers}", 
            _configuration.BootstrapServers);
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) 
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await PublishAsync(message, topic, cancellationToken);
    }

    public async Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) 
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);

            var kafkaMessage = new Message<string, byte[]>
            {
                Key = message.Id.ToString(),
                Value = messageBytes,
                Headers = new Headers
                {
                    { "message-type", Encoding.UTF8.GetBytes(typeof(T).AssemblyQualifiedName ?? typeof(T).FullName!) },
                    { "message-id", Encoding.UTF8.GetBytes(message.Id.ToString()) },
                    { "timestamp", Encoding.UTF8.GetBytes(message.Timestamp.ToString("O")) },
                    { "content-type", Encoding.UTF8.GetBytes("application/json") }
                }
            };

            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

            _logger.LogDebug(
                "Message published to Kafka - Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, MessageId: {MessageId}",
                topic, deliveryResult.Partition.Value, deliveryResult.Offset.Value, message.Id);
        }
        catch (ProduceException<string, byte[]> ex)
        {
            _logger.LogError(ex, 
                "Failed to publish message to Kafka topic {Topic}. Error: {Error}", 
                topic, ex.Error.Reason);
            throw new InvalidOperationException($"Failed to publish message to topic '{topic}'", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error publishing message to Kafka topic {Topic}", topic);
            throw;
        }
    }

    public async Task PublishAsync<T>(T message, MessageEnvelop metadata, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await PublishAsync(message, topic, metadata, cancellationToken);
    }

    public async Task PublishAsync<T>(T message, string topic, MessageEnvelop metadata, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(metadata);

        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);

            var kafkaMessage = new Message<string, byte[]>
            {
                Key = message.Id.ToString(),
                Value = messageBytes,
                Headers = new Headers
                {
                    { "message-type", Encoding.UTF8.GetBytes(typeof(T).AssemblyQualifiedName ?? typeof(T).FullName!) },
                    { "message-id", Encoding.UTF8.GetBytes(metadata.MessageId) },
                    { "timestamp", Encoding.UTF8.GetBytes(metadata.Timestamp.ToString("O")) },
                    { "content-type", Encoding.UTF8.GetBytes("application/json") }
                }
            };

            // Add metadata to headers
            if (!string.IsNullOrEmpty(metadata.CorrelationId))
                kafkaMessage.Headers.Add("correlation-id", Encoding.UTF8.GetBytes(metadata.CorrelationId));
            if (!string.IsNullOrEmpty(metadata.CausationId))
                kafkaMessage.Headers.Add("causation-id", Encoding.UTF8.GetBytes(metadata.CausationId));
            if (!string.IsNullOrEmpty(metadata.UserId))
                kafkaMessage.Headers.Add("user-id", Encoding.UTF8.GetBytes(metadata.UserId));
            if (!string.IsNullOrEmpty(metadata.TraceId))
                kafkaMessage.Headers.Add("trace-id", Encoding.UTF8.GetBytes(metadata.TraceId));
            if (!string.IsNullOrEmpty(metadata.Source))
                kafkaMessage.Headers.Add("source", Encoding.UTF8.GetBytes(metadata.Source));
            if (!string.IsNullOrEmpty(metadata.Destination))
                kafkaMessage.Headers.Add("destination", Encoding.UTF8.GetBytes(metadata.Destination));
            if (metadata.Priority.HasValue)
                kafkaMessage.Headers.Add("priority", Encoding.UTF8.GetBytes(metadata.Priority.Value.ToString()));
            if (metadata.TimeToLive.HasValue)
                kafkaMessage.Headers.Add("ttl", Encoding.UTF8.GetBytes(metadata.TimeToLive.Value.TotalSeconds.ToString()));

            // Add custom headers
            foreach (var header in metadata.Headers)
            {
                kafkaMessage.Headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
            }

            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

            _logger.LogDebug(
                "Message published to Kafka with metadata - Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, MessageId: {MessageId}, CorrelationId: {CorrelationId}",
                topic, deliveryResult.Partition.Value, deliveryResult.Offset.Value, metadata.MessageId, metadata.CorrelationId);
        }
        catch (ProduceException<string, byte[]> ex)
        {
            _logger.LogError(ex,
                "Failed to publish message to Kafka topic {Topic}. Error: {Error}",
                topic, ex.Error.Reason);
            throw new InvalidOperationException($"Failed to publish message to topic '{topic}'", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error publishing message to Kafka topic {Topic}", topic);
            throw;
        }
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await PublishBatchAsync(messages, topic, cancellationToken);
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, string topic, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(messages);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        var messageList = messages.ToList();
        if (messageList.Count == 0)
        {
            _logger.LogWarning("PublishBatchAsync called with empty message collection");
            return;
        }

        try
        {
            var tasks = new List<Task<DeliveryResult<string, byte[]>>>();

            foreach (var message in messageList)
            {
                var messageJson = JsonSerializer.Serialize(message);
                var messageBytes = Encoding.UTF8.GetBytes(messageJson);

                var kafkaMessage = new Message<string, byte[]>
                {
                    Key = message.Id.ToString(),
                    Value = messageBytes,
                    Headers = new Headers
                    {
                        { "message-type", Encoding.UTF8.GetBytes(typeof(T).AssemblyQualifiedName ?? typeof(T).FullName!) },
                        { "message-id", Encoding.UTF8.GetBytes(message.Id.ToString()) },
                        { "timestamp", Encoding.UTF8.GetBytes(message.Timestamp.ToString("O")) },
                        { "content-type", Encoding.UTF8.GetBytes("application/json") }
                    }
                };

                tasks.Add(_producer.ProduceAsync(topic, kafkaMessage, cancellationToken));
            }

            await Task.WhenAll(tasks);

            _logger.LogInformation(
                "Batch of {Count} messages published to Kafka topic {Topic}",
                messageList.Count, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing batch messages to Kafka topic {Topic}", topic);
            throw;
        }
    }

    public async Task ScheduleAsync<T>(T message, DateTime scheduledTime, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await ScheduleAsync(message, topic, scheduledTime, cancellationToken);
    }

    public async Task ScheduleAsync<T>(T message, string topic, DateTime scheduledTime, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        var delay = scheduledTime - DateTime.UtcNow;
        if (delay <= TimeSpan.Zero)
        {
            // If scheduled time is in the past or now, publish immediately
            await PublishAsync(message, topic, cancellationToken);
            return;
        }

        _logger.LogInformation(
            "Scheduling message {MessageId} for delivery at {ScheduledTime} (delay: {Delay})",
            message.Id, scheduledTime, delay);

        // Use Task.Delay to schedule the message
        // Note: For production, consider using a more robust scheduling mechanism
        // like Hangfire, Quartz.NET, or a dedicated scheduler service
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, cancellationToken);
                await PublishAsync(message, topic, cancellationToken);

                _logger.LogInformation(
                    "Scheduled message {MessageId} published to topic {Topic}",
                    message.Id, topic);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Scheduled message {MessageId} cancelled before delivery",
                    message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error publishing scheduled message {MessageId} to topic {Topic}",
                    message.Id, topic);
            }
        }, cancellationToken);

        await Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        return SubscribeAsync(topic, handler, cancellationToken);
    }

    public Task SubscribeAsync<T>(string topic, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : class, IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(handler);

        if (_subscriptions.ContainsKey(topic))
        {
            _logger.LogWarning("Already subscribed to topic {Topic}. Skipping duplicate subscription.", topic);
            return Task.CompletedTask;
        }

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration.BootstrapServers,
            GroupId = _configuration.GroupId,
            ClientId = $"{_configuration.ClientId}-consumer-{Guid.NewGuid():N}",
            AutoOffsetReset = ParseAutoOffsetReset(_configuration.AutoOffsetReset),
            EnableAutoCommit = _configuration.EnableAutoCommit,
            SessionTimeoutMs = _configuration.SessionTimeoutMs,
            MaxPollIntervalMs = _configuration.MaxPollIntervalMs,
            SecurityProtocol = ParseSecurityProtocol(_configuration.SecurityProtocol),
        };

        // Add SASL configuration if specified
        if (!string.IsNullOrEmpty(_configuration.SaslMechanism))
        {
            consumerConfig.SaslMechanism = ParseSaslMechanism(_configuration.SaslMechanism);
            consumerConfig.SaslUsername = _configuration.SaslUsername;
            consumerConfig.SaslPassword = _configuration.SaslPassword;
        }

        var consumer = new ConsumerBuilder<string, byte[]>(consumerConfig).Build();
        consumer.Subscribe(topic);

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var subscription = new ConsumerSubscription
        {
            Topic = topic,
            Consumer = consumer,
            CancellationTokenSource = cts,
            ConsumerTask = Task.Run(() => ConsumeMessagesAsync(consumer, handler, cts.Token), cts.Token)
        };

        _subscriptions[topic] = subscription;

        _logger.LogInformation("Subscribed to Kafka topic: {Topic} with message type: {MessageType}", 
            topic, typeof(T).Name);

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(Func<T, MessageEnvelop, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        return SubscribeAsync(topic, handler, cancellationToken);
    }

    public Task SubscribeAsync<T>(string topic, Func<T, MessageEnvelop, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(handler);

        if (_subscriptions.ContainsKey(topic))
        {
            _logger.LogWarning("Already subscribed to topic {Topic}. Skipping duplicate subscription.", topic);
            return Task.CompletedTask;
        }

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration.BootstrapServers,
            GroupId = _configuration.GroupId,
            ClientId = $"{_configuration.ClientId}-consumer-{Guid.NewGuid():N}",
            AutoOffsetReset = ParseAutoOffsetReset(_configuration.AutoOffsetReset),
            EnableAutoCommit = _configuration.EnableAutoCommit,
            SessionTimeoutMs = _configuration.SessionTimeoutMs,
            MaxPollIntervalMs = _configuration.MaxPollIntervalMs,
            SecurityProtocol = ParseSecurityProtocol(_configuration.SecurityProtocol),
        };

        // Add SASL configuration if specified
        if (!string.IsNullOrEmpty(_configuration.SaslMechanism))
        {
            consumerConfig.SaslMechanism = ParseSaslMechanism(_configuration.SaslMechanism);
            consumerConfig.SaslUsername = _configuration.SaslUsername;
            consumerConfig.SaslPassword = _configuration.SaslPassword;
        }

        var consumer = new ConsumerBuilder<string, byte[]>(consumerConfig).Build();
        consumer.Subscribe(topic);

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var subscription = new ConsumerSubscription
        {
            Topic = topic,
            Consumer = consumer,
            CancellationTokenSource = cts,
            ConsumerTask = Task.Run(() => ConsumeMessagesWithMetadataAsync(consumer, handler, cts.Token), cts.Token)
        };

        _subscriptions[topic] = subscription;

        _logger.LogInformation("Subscribed to Kafka topic: {Topic} with message type: {MessageType} (with metadata)", 
            topic, typeof(T).Name);

        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        return UnsubscribeAsync(topic, cancellationToken);
    }

    public Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        if (!_subscriptions.TryRemove(topic, out var subscription))
        {
            _logger.LogWarning("No active subscription found for topic {Topic}", topic);
            return Task.CompletedTask;
        }

        try
        {
            subscription.CancellationTokenSource?.Cancel();
            subscription.ConsumerTask?.Wait(TimeSpan.FromSeconds(5));
            subscription.Consumer?.Close();
            subscription.Consumer?.Dispose();
            subscription.CancellationTokenSource?.Dispose();

            _logger.LogInformation("Unsubscribed from Kafka topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unsubscribing from topic {Topic}", topic);
            throw;
        }

        return Task.CompletedTask;
    }

    private async Task ConsumeMessagesAsync<T>(
        IConsumer<string, byte[]> consumer,
        Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken) where T : class, IMessage
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);

                    if (consumeResult?.Message == null)
                        continue;

                    var messageJson = Encoding.UTF8.GetString(consumeResult.Message.Value);
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message == null)
                    {
                        _logger.LogWarning(
                            "Failed to deserialize message from topic {Topic}, partition {Partition}, offset {Offset}",
                            consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value);
                        consumer.Commit(consumeResult);
                        continue;
                    }

                    _logger.LogDebug(
                        "Consuming message from topic {Topic}, partition {Partition}, offset {Offset}, MessageId: {MessageId}",
                        consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value, message.Id);

                    try
                    {
                        await handler(message, cancellationToken);
                        consumer.Commit(consumeResult);

                        _logger.LogDebug(
                            "Successfully processed message from topic {Topic}, offset {Offset}, MessageId: {MessageId}",
                            consumeResult.Topic, consumeResult.Offset.Value, message.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error handling message from topic {Topic}, partition {Partition}, offset {Offset}, MessageId: {MessageId}",
                            consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value, message.Id);

                        // Don't commit on error - message will be reprocessed
                        // Implement dead letter queue or error handling strategy here if needed
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka: {Error}", ex.Error.Reason);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Kafka consumer loop");
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer closed");
        }
    }

    private async Task ConsumeMessagesWithMetadataAsync<T>(
        IConsumer<string, byte[]> consumer,
        Func<T, MessageEnvelop, CancellationToken, Task> handler,
        CancellationToken cancellationToken) where T : class, IMessage
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);

                    if (consumeResult?.Message == null)
                        continue;

                    var messageJson = Encoding.UTF8.GetString(consumeResult.Message.Value);
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message == null)
                    {
                        _logger.LogWarning(
                            "Failed to deserialize message from topic {Topic}, partition {Partition}, offset {Offset}",
                            consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value);
                        consumer.Commit(consumeResult);
                        continue;
                    }

                    // Extract metadata from headers
                    var metadata = ExtractMetadata(consumeResult.Message.Headers);

                    _logger.LogDebug(
                        "Consuming message from topic {Topic}, partition {Partition}, offset {Offset}, MessageId: {MessageId}, CorrelationId: {CorrelationId}",
                        consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value, message.Id, metadata.CorrelationId);

                    try
                    {
                        await handler(message, metadata, cancellationToken);
                        consumer.Commit(consumeResult);

                        _logger.LogDebug(
                            "Successfully processed message from topic {Topic}, offset {Offset}, MessageId: {MessageId}",
                            consumeResult.Topic, consumeResult.Offset.Value, message.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error handling message from topic {Topic}, partition {Partition}, offset {Offset}, MessageId: {MessageId}",
                            consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value, message.Id);

                        // Don't commit on error - message will be reprocessed
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka: {Error}", ex.Error.Reason);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Kafka consumer loop");
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer closed");
        }
    }

    private MessageEnvelop ExtractMetadata(Headers headers)
    {
        var metadata = new MessageEnvelop();

        foreach (var header in headers)
        {
            var value = Encoding.UTF8.GetString(header.GetValueBytes());

            switch (header.Key.ToLowerInvariant())
            {
                case "message-id":
                    metadata.MessageId = value;
                    break;
                case "correlation-id":
                    metadata.CorrelationId = value;
                    break;
                case "causation-id":
                    metadata.CausationId = value;
                    break;
                case "user-id":
                    metadata.UserId = value;
                    break;
                case "trace-id":
                    metadata.TraceId = value;
                    break;
                case "message-type":
                    metadata.MessageType = value;
                    break;
                case "source":
                    metadata.Source = value;
                    break;
                case "destination":
                    metadata.Destination = value;
                    break;
                case "priority":
                    if (int.TryParse(value, out var priority))
                        metadata.Priority = priority;
                    break;
                case "ttl":
                    if (double.TryParse(value, out var ttlSeconds))
                        metadata.TimeToLive = TimeSpan.FromSeconds(ttlSeconds);
                    break;
                case "timestamp":
                    if (DateTime.TryParse(value, out var timestamp))
                        metadata.Timestamp = timestamp;
                    break;
                default:
                    // Add unknown headers to the Headers dictionary
                    metadata.AddHeader(header.Key, value);
                    break;
            }
        }

        return metadata;
    }

    private string GetTopicName<T>()
    {
        return _topicCache.GetOrAdd(typeof(T), type => type.Name.ToLowerInvariant());
    }

    private static Confluent.Kafka.CompressionType ParseCompressionType(string compressionType)
    {
        return compressionType.ToLowerInvariant() switch
        {
            "gzip" => Confluent.Kafka.CompressionType.Gzip,
            "snappy" => Confluent.Kafka.CompressionType.Snappy,
            "lz4" => Confluent.Kafka.CompressionType.Lz4,
            "zstd" => Confluent.Kafka.CompressionType.Zstd,
            _ => Confluent.Kafka.CompressionType.None
        };
    }

    private static Confluent.Kafka.AutoOffsetReset ParseAutoOffsetReset(string autoOffsetReset)
    {
        return autoOffsetReset.ToLowerInvariant() switch
        {
            "earliest" => Confluent.Kafka.AutoOffsetReset.Earliest,
            "latest" => Confluent.Kafka.AutoOffsetReset.Latest,
            _ => Confluent.Kafka.AutoOffsetReset.Error
        };
    }

    private static Confluent.Kafka.SecurityProtocol ParseSecurityProtocol(string securityProtocol)
    {
        return securityProtocol.ToLowerInvariant() switch
        {
            "ssl" => Confluent.Kafka.SecurityProtocol.Ssl,
            "saslplaintext" => Confluent.Kafka.SecurityProtocol.SaslPlaintext,
            "saslssl" => Confluent.Kafka.SecurityProtocol.SaslSsl,
            _ => Confluent.Kafka.SecurityProtocol.Plaintext
        };
    }

    private static Confluent.Kafka.SaslMechanism ParseSaslMechanism(string saslMechanism)
    {
        return saslMechanism.ToLowerInvariant() switch
        {
            "plain" => Confluent.Kafka.SaslMechanism.Plain,
            "scramsha256" => Confluent.Kafka.SaslMechanism.ScramSha256,
            "scramsha512" => Confluent.Kafka.SaslMechanism.ScramSha512,
            "gssapi" => Confluent.Kafka.SaslMechanism.Gssapi,
            "oauthbearer" => Confluent.Kafka.SaslMechanism.OAuthBearer,
            _ => throw new ArgumentException($"Unknown SASL mechanism: {saslMechanism}")
        };
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        foreach (var subscription in _subscriptions.Values)
        {
            subscription.CancellationTokenSource?.Cancel();
            subscription.ConsumerTask?.Wait(TimeSpan.FromSeconds(5));
            subscription.Consumer?.Dispose();
            subscription.CancellationTokenSource?.Dispose();
        }

        _subscriptions.Clear();
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();

        _logger.LogInformation("KafkaMessageBus disposed");
    }

    private sealed class ConsumerSubscription
    {
        required public string Topic { get; init; }
        required public IConsumer<string, byte[]> Consumer { get; init; }
        required public CancellationTokenSource CancellationTokenSource { get; init; }
        required public Task ConsumerTask { get; init; }
    }
}
