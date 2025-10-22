using BuildingBlocks.Application.CQRS.Messages;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Infrastructure.Messaging.Configuration;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using AppMessageMetadata = BuildingBlocks.Application.Messaging.MessageEnvelop;

namespace BuildingBlocks.Infrastructure.Messaging.Kafka;

/// <summary>
/// Kafka message publisher implementation
/// </summary>
public sealed class KafkaMessagePublisher : IDisposable
{
    private readonly ILogger<KafkaMessagePublisher> _logger;
    private readonly KafkaConfiguration _configuration;
    private readonly IProducer<string, byte[]> _producer;
    private bool _disposed;

    public KafkaMessagePublisher(
        ILogger<KafkaMessagePublisher> logger,
        IOptions<KafkaConfiguration> configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _configuration.BootstrapServers,
            ClientId = $"{_configuration.ClientId}-publisher",
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

        if (!string.IsNullOrEmpty(_configuration.SaslMechanism))
        {
            producerConfig.SaslMechanism = ParseSaslMechanism(_configuration.SaslMechanism);
            producerConfig.SaslUsername = _configuration.SaslUsername;
            producerConfig.SaslPassword = _configuration.SaslPassword;
        }

        _producer = new ProducerBuilder<string, byte[]>(producerConfig).Build();
        _logger.LogInformation("KafkaMessagePublisher initialized");
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await PublishAsync(message, topic, cancellationToken);
    }

    public async Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) where T : class, IMessage
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
                Headers = CreateHeaders<T>(message)
            };

            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

            _logger.LogDebug(
                "Published message to Kafka - Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, MessageId: {MessageId}",
                topic, deliveryResult.Partition.Value, deliveryResult.Offset.Value, message.Id);
        }
        catch (ProduceException<string, byte[]> ex)
        {
            _logger.LogError(ex, "Failed to publish message to Kafka topic {Topic}", topic);
            throw new InvalidOperationException($"Failed to publish message to topic '{topic}'", ex);
        }
    }

    public async Task PublishAsync<T>(T message, AppMessageMetadata metadata, CancellationToken cancellationToken = default) where T : class, IMessage
    {
        var topic = metadata.Destination ?? GetTopicName<T>();
        await PublishWithMetadataAsync(message, topic, metadata, cancellationToken);
    }

    public async Task PublishAsync<T>(T message, string topic, AppMessageMetadata metadata, CancellationToken cancellationToken = default) where T : class, IMessage
    {
        await PublishWithMetadataAsync(message, topic, metadata, cancellationToken);
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default) where T : class, IMessage
    {
        var topic = GetTopicName<T>();
        await PublishBatchAsync(messages, topic, cancellationToken);
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, string topic, CancellationToken cancellationToken = default) where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(messages);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);

        var tasks = messages.Select(message => PublishAsync(message, topic, cancellationToken));
        await Task.WhenAll(tasks);

        _logger.LogInformation("Published batch of {Count} messages to topic {Topic}", messages.Count(), topic);
    }

    private async Task PublishWithMetadataAsync<T>(T message, string topic, AppMessageMetadata metadata, CancellationToken cancellationToken) where T : class, IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(metadata);

        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);

            var kafkaMessage = new Message<string, byte[]>
            {
                Key = metadata.MessageId ?? message.Id.ToString(),
                Value = messageBytes,
                Headers = CreateHeadersWithMetadata<T>(message, metadata)
            };

            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

            _logger.LogDebug(
                "Published message with metadata to Kafka - Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, MessageId: {MessageId}",
                topic, deliveryResult.Partition.Value, deliveryResult.Offset.Value, metadata.MessageId);
        }
        catch (ProduceException<string, byte[]> ex)
        {
            _logger.LogError(ex, "Failed to publish message with metadata to Kafka topic {Topic}", topic);
            throw new InvalidOperationException($"Failed to publish message to topic '{topic}'", ex);
        }
    }

    private static Headers CreateHeaders<T>(IMessage message)
    {
        return new Headers
        {
            { "message-type", Encoding.UTF8.GetBytes(typeof(T).AssemblyQualifiedName ?? typeof(T).FullName!) },
            { "message-id", Encoding.UTF8.GetBytes(message.Id.ToString()) },
            { "timestamp", Encoding.UTF8.GetBytes(message.Timestamp.ToString("O")) },
            { "content-type", Encoding.UTF8.GetBytes("application/json") }
        };
    }

    private static Headers CreateHeadersWithMetadata<T>(IMessage message, AppMessageMetadata metadata)
    {
        var headers = new Headers
        {
            { "message-type", Encoding.UTF8.GetBytes(typeof(T).AssemblyQualifiedName ?? typeof(T).FullName!) },
            { "message-id", Encoding.UTF8.GetBytes(metadata.MessageId ?? message.Id.ToString()) },
            { "timestamp", Encoding.UTF8.GetBytes(message.Timestamp.ToString("O")) },
            { "content-type", Encoding.UTF8.GetBytes("application/json") }
        };

        if (!string.IsNullOrEmpty(metadata.CorrelationId))
            headers.Add("correlation-id", Encoding.UTF8.GetBytes(metadata.CorrelationId));

        if (!string.IsNullOrEmpty(metadata.CausationId))
            headers.Add("causation-id", Encoding.UTF8.GetBytes(metadata.CausationId));

        if (!string.IsNullOrEmpty(metadata.UserId))
            headers.Add("user-id", Encoding.UTF8.GetBytes(metadata.UserId));

        if (!string.IsNullOrEmpty(metadata.Source))
            headers.Add("source", Encoding.UTF8.GetBytes(metadata.Source));

        foreach (var header in metadata.Headers)
        {
            headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
        }

        return headers;
    }

    private static string GetTopicName<T>() => typeof(T).Name.ToLowerInvariant();

    private static Confluent.Kafka.CompressionType ParseCompressionType(string compressionType) =>
        compressionType.ToLowerInvariant() switch
        {
            "gzip" => Confluent.Kafka.CompressionType.Gzip,
            "snappy" => Confluent.Kafka.CompressionType.Snappy,
            "lz4" => Confluent.Kafka.CompressionType.Lz4,
            "zstd" => Confluent.Kafka.CompressionType.Zstd,
            _ => Confluent.Kafka.CompressionType.None
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

        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        _logger.LogInformation("KafkaMessagePublisher disposed");
    }
}
