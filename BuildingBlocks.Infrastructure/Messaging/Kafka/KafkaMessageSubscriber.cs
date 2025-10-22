using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Infrastructure.Messaging.Configuration;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using BuildingBlocks.Application.Messaging;

namespace BuildingBlocks.Infrastructure.Messaging.Kafka;

/// <summary>
/// Kafka message subscriber implementation with subscription management
/// </summary>
public sealed class KafkaMessageSubscriber : IDisposable
{
    private readonly ILogger<KafkaMessageSubscriber> _logger;
    private readonly KafkaConfiguration _configuration;
    private readonly ConcurrentDictionary<string, SubscriptionContext> _subscriptions = new();
    private bool _disposed;

    public KafkaMessageSubscriber(
        ILogger<KafkaMessageSubscriber> logger,
        IOptions<KafkaConfiguration> configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger.LogInformation("KafkaMessageSubscriber initialized");
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
            _logger.LogWarning("Already subscribed to topic {Topic}", topic);
            return Task.CompletedTask;
        }

        var consumer = CreateConsumer();
        consumer.Subscribe(topic);

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var context = new SubscriptionContext
        {
            Topic = topic,
            Consumer = consumer,
            CancellationTokenSource = cts,
            ConsumerTask = Task.Run(() => ConsumeAsync(consumer, handler, cts.Token), cts.Token)
        };

        _subscriptions[topic] = context;
        _logger.LogInformation("Subscribed to Kafka topic: {Topic}", topic);

        return Task.CompletedTask;
    }

    public Task SubscribeWithMetadataAsync<T>(
        Func<T, MessageEnvelop, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        return SubscribeWithMetadataAsync(topic, handler, cancellationToken);
    }

    public Task SubscribeWithMetadataAsync<T>(
        string topic,
        Func<T, MessageEnvelop, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentNullException.ThrowIfNull(handler);

        if (_subscriptions.ContainsKey(topic))
        {
            _logger.LogWarning("Already subscribed to topic {Topic}", topic);
            return Task.CompletedTask;
        }

        var consumer = CreateConsumer();
        consumer.Subscribe(topic);

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var context = new SubscriptionContext
        {
            Topic = topic,
            Consumer = consumer,
            CancellationTokenSource = cts,
            ConsumerTask = Task.Run(() => ConsumeWithMetadataAsync(consumer, handler, cts.Token), cts.Token)
        };

        _subscriptions[topic] = context;
        _logger.LogInformation("Subscribed to Kafka topic with metadata: {Topic}", topic);

        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        return UnsubscribeAsync(topic, cancellationToken);
    }

    public Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        if (_subscriptions.TryRemove(topic, out var context))
        {
            context.CancellationTokenSource?.Cancel();
            context.ConsumerTask?.Wait(TimeSpan.FromSeconds(5));
            context.Consumer?.Close();
            context.Consumer?.Dispose();
            context.CancellationTokenSource?.Dispose();

            _logger.LogInformation("Unsubscribed from Kafka topic: {Topic}", topic);
        }

        return Task.CompletedTask;
    }

    private async Task ConsumeAsync<T>(
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
                    if (consumeResult?.Message == null) continue;

                    var message = DeserializeMessage<T>(consumeResult);
                    if (message == null)
                    {
                        _logger.LogWarning("Failed to deserialize message from {Topic}", consumeResult.Topic);
                        consumer.Commit(consumeResult);
                        continue;
                    }

                    _logger.LogDebug(
                        "Consuming message from {Topic}, partition {Partition}, offset {Offset}",
                        consumeResult.Topic, consumeResult.Partition.Value, consumeResult.Offset.Value);

                    try
                    {
                        await handler(message, cancellationToken);
                        consumer.Commit(consumeResult);

                        _logger.LogDebug("Successfully processed message from {Topic}", consumeResult.Topic);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling message from {Topic}", consumeResult.Topic);
                        // Don't commit - message will be reprocessed
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (OperationCanceledException)
                {
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
        }
    }

    private async Task ConsumeWithMetadataAsync<T>(
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
                    if (consumeResult?.Message == null) continue;

                    var message = DeserializeMessage<T>(consumeResult);
                    if (message == null)
                    {
                        consumer.Commit(consumeResult);
                        continue;
                    }

                    var metadata = ExtractMetadata(consumeResult);

                    try
                    {
                        await handler(message, metadata, cancellationToken);
                        consumer.Commit(consumeResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling message with metadata from {Topic}", consumeResult.Topic);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (OperationCanceledException)
                {
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
        }
    }

    private static T? DeserializeMessage<T>(ConsumeResult<string, byte[]> consumeResult) where T : class
    {
        var messageJson = Encoding.UTF8.GetString(consumeResult.Message.Value);
        return JsonSerializer.Deserialize<T>(messageJson);
    }

    private static MessageEnvelop ExtractMetadata(ConsumeResult<string, byte[]> consumeResult)
    {
        var metadata = new MessageEnvelop();

        foreach (var header in consumeResult.Message.Headers)
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
                case "source":
                    metadata.Source = value;
                    break;
                case "message-type":
                    metadata.MessageType = value;
                    break;
                default:
                    metadata.AddHeader(header.Key, value);
                    break;
            }
        }

        return metadata;
    }

    private IConsumer<string, byte[]> CreateConsumer()
    {
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

        if (!string.IsNullOrEmpty(_configuration.SaslMechanism))
        {
            consumerConfig.SaslMechanism = ParseSaslMechanism(_configuration.SaslMechanism);
            consumerConfig.SaslUsername = _configuration.SaslUsername;
            consumerConfig.SaslPassword = _configuration.SaslPassword;
        }

        return new ConsumerBuilder<string, byte[]>(consumerConfig).Build();
    }

    private static string GetTopicName<T>() => typeof(T).Name.ToLowerInvariant();

    private static Confluent.Kafka.AutoOffsetReset ParseAutoOffsetReset(string autoOffsetReset) =>
        autoOffsetReset.ToLowerInvariant() switch
        {
            "earliest" => Confluent.Kafka.AutoOffsetReset.Earliest,
            "latest" => Confluent.Kafka.AutoOffsetReset.Latest,
            _ => Confluent.Kafka.AutoOffsetReset.Error
        };

    private static Confluent.Kafka.SecurityProtocol ParseSecurityProtocol(string securityProtocol) =>
        securityProtocol.ToLowerInvariant() switch
        {
            "ssl" => Confluent.Kafka.SecurityProtocol.Ssl,
            "saslplaintext" => Confluent.Kafka.SecurityProtocol.SaslPlaintext,
            "saslssl" => Confluent.Kafka.SecurityProtocol.SaslSsl,
            _ => Confluent.Kafka.SecurityProtocol.Plaintext
        };

    private static Confluent.Kafka.SaslMechanism ParseSaslMechanism(string saslMechanism) =>
        saslMechanism.ToLowerInvariant() switch
        {
            "plain" => Confluent.Kafka.SaslMechanism.Plain,
            "scramsha256" => Confluent.Kafka.SaslMechanism.ScramSha256,
            "scramsha512" => Confluent.Kafka.SaslMechanism.ScramSha512,
            "gssapi" => Confluent.Kafka.SaslMechanism.Gssapi,
            "oauthbearer" => Confluent.Kafka.SaslMechanism.OAuthBearer,
            _ => throw new ArgumentException($"Unknown SASL mechanism: {saslMechanism}")
        };

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var subscription in _subscriptions.Values)
        {
            subscription.CancellationTokenSource?.Cancel();
            subscription.ConsumerTask?.Wait(TimeSpan.FromSeconds(5));
            subscription.Consumer?.Close();
            subscription.Consumer?.Dispose();
            subscription.CancellationTokenSource?.Dispose();
        }

        _subscriptions.Clear();
        _logger.LogInformation("KafkaMessageSubscriber disposed");
    }

    private class SubscriptionContext
    {
        public string Topic { get; init; } = string.Empty;
        public IConsumer<string, byte[]>? Consumer { get; init; }
        public CancellationTokenSource? CancellationTokenSource { get; init; }
        public Task? ConsumerTask { get; init; }
    }
}
