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
