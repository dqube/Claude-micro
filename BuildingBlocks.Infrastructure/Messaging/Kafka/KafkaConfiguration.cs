namespace BuildingBlocks.Infrastructure.Messaging.Configuration;

/// <summary>
/// Configuration options for Kafka message broker
/// </summary>
public class KafkaConfiguration
{
    public const string SectionName = "Kafka";

    /// <summary>
    /// Comma-separated list of Kafka broker addresses (host:port)
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:9092";

    /// <summary>
    /// Consumer group identifier
    /// </summary>
    public string GroupId { get; set; } = "buildingblocks-consumer-group";

    /// <summary>
    /// Client identifier for Kafka connection
    /// </summary>
    public string ClientId { get; set; } = "buildingblocks-client";

    /// <summary>
    /// What to do when there is no initial offset in Kafka
    /// </summary>
    public string AutoOffsetReset { get; set; } = "earliest";

    /// <summary>
    /// Enable auto-commit of offsets
    /// </summary>
    public bool EnableAutoCommit { get; set; } = false;

    /// <summary>
    /// Session timeout in milliseconds
    /// </summary>
    public int SessionTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Maximum poll interval in milliseconds
    /// </summary>
    public int MaxPollIntervalMs { get; set; } = 300000;

    /// <summary>
    /// Security protocol (Plaintext, Ssl, SaslPlaintext, SaslSsl)
    /// </summary>
    public string SecurityProtocol { get; set; } = "Plaintext";

    /// <summary>
    /// SASL mechanism (Plain, ScramSha256, ScramSha512, etc.)
    /// </summary>
    public string? SaslMechanism { get; set; }

    /// <summary>
    /// SASL username for authentication
    /// </summary>
    public string? SaslUsername { get; set; }

    /// <summary>
    /// SASL password for authentication
    /// </summary>
    public string? SaslPassword { get; set; }

    /// <summary>
    /// Message timeout in milliseconds
    /// </summary>
    public int MessageTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Request timeout in milliseconds
    /// </summary>
    public int RequestTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Number of acknowledgements the producer requires (0, 1, or -1/all)
    /// </summary>
    public short RequiredAcks { get; set; } = -1; // All

    /// <summary>
    /// Compression type (none, gzip, snappy, lz4, zstd)
    /// </summary>
    public string CompressionType { get; set; } = "none";

    /// <summary>
    /// Retry backoff in milliseconds
    /// </summary>
    public int RetryBackoffMs { get; set; } = 100;

    /// <summary>
    /// Maximum message size in bytes
    /// </summary>
    public int MessageMaxBytes { get; set; } = 1048576; // 1MB

    /// <summary>
    /// Enable idempotent producer
    /// </summary>
    public bool EnableIdempotence { get; set; } = true;

    /// <summary>
    /// Maximum number of in-flight requests per connection
    /// </summary>
    public int MaxInFlight { get; set; } = 5;
}
