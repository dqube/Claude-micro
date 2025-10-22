# Kafka Message Broker Implementation

This document provides comprehensive information about the Kafka message broker implementation in BuildingBlocks.Infrastructure.

## Overview

The Kafka message broker implementation provides a production-ready, scalable messaging solution using Apache Kafka through the Confluent.Kafka client library. It implements the `BuildingBlocks.Application.Messaging.IMessageBus` interface, enabling seamless message publishing and subscription with full Kafka capabilities.

## Features

- ? **Full Kafka Integration** - Producer and consumer implementation using Confluent.Kafka
- ? **JSON Serialization** - Automatic message serialization using System.Text.Json
- ? **Message Headers** - Rich metadata including message type, ID, timestamp, and content type
- ? **Consumer Groups** - Support for consumer groups with configurable settings
- ? **Offset Management** - Manual offset commit for guaranteed message processing
- ? **Security Support** - SASL/SSL authentication and encryption
- ? **Compression** - Support for Gzip, Snappy, LZ4, and Zstd compression
- ? **Idempotent Producer** - Exactly-once semantics with idempotent production
- ? **Configurable Settings** - Comprehensive configuration options
- ? **Error Handling** - Robust error handling with logging

## Configuration

### appsettings.json

Add the following configuration sections to your `appsettings.json`:

```json
{
  "Messaging": {
    "Provider": "Kafka"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "my-service-consumer-group",
    "ClientId": "my-service-client",
    "AutoOffsetReset": "earliest",
    "EnableAutoCommit": false,
    "SessionTimeoutMs": 30000,
    "MaxPollIntervalMs": 300000,
    "SecurityProtocol": "Plaintext",
    "MessageTimeoutMs": 30000,
    "RequestTimeoutMs": 30000,
    "RequiredAcks": -1,
    "CompressionType": "none",
    "RetryBackoffMs": 100,
    "MessageMaxBytes": 1048576,
    "EnableIdempotence": true,
    "MaxInFlight": 5
  }
}
```

### Configuration with Security (SASL/SSL)

For production environments with authentication:

```json
{
  "Kafka": {
    "BootstrapServers": "kafka.example.com:9093",
    "GroupId": "my-service-consumer-group",
    "ClientId": "my-service-client",
    "SecurityProtocol": "SaslSsl",
    "SaslMechanism": "ScramSha256",
    "SaslUsername": "your-username",
    "SaslPassword": "your-password",
    "CompressionType": "snappy",
    "EnableIdempotence": true
  }
}
```

## Service Registration

### Option 1: Automatic Registration (Recommended)

The Kafka message broker is automatically registered when you call `AddInfrastructureServices` and set the provider to "Kafka" in configuration:

```csharp
using BuildingBlocks.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Automatically registers Kafka if Provider=Kafka in configuration
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();
app.Run();
```

### Option 2: Manual Registration

You can also manually register the Kafka message bus:

```csharp
using BuildingBlocks.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Manually register Kafka messaging
builder.Services.AddKafkaMessaging(builder.Configuration);

var app = builder.Build();
app.Run();
```

### Option 3: Programmatic Configuration

Configure Kafka without appsettings.json:

```csharp
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Messaging.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafkaMessaging(options =>
{
    options.BootstrapServers = "localhost:9092";
    options.GroupId = "my-service-group";
    options.ClientId = "my-service";
    options.EnableAutoCommit = false;
    options.CompressionType = "snappy";
});

var app = builder.Build();
app.Run();
```

## Usage Examples

### Publishing Messages

```csharp
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.CQRS.Messages;

public class OrderCreatedEvent : MessageBase
{
    public Guid OrderId { get; init; }
    public decimal TotalAmount { get; init; }
    public string CustomerEmail { get; init; }
}

public class OrderService
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IMessageBus messageBus, ILogger<OrderService> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        // ... create order logic ...

        // Publish to default topic (based on message type name)
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            CustomerEmail = request.CustomerEmail
        };

        await _messageBus.PublishAsync(orderEvent, cancellationToken);
        
        _logger.LogInformation("Order {OrderId} created and event published", order.Id);
    }

    public async Task PublishToCustomTopicAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken)
    {
        // Publish to a specific topic
        await _messageBus.PublishAsync(orderEvent, "orders.created.v1", cancellationToken);
    }
}
```

### Subscribing to Messages

```csharp
using BuildingBlocks.Application.Messaging;

public class OrderEventConsumer : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OrderEventConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderEventConsumer(
        IMessageBus messageBus, 
        ILogger<OrderEventConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _messageBus = messageBus;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Subscribe to default topic
        await _messageBus.SubscribeAsync<OrderCreatedEvent>(HandleOrderCreatedAsync, stoppingToken);
        
        // Or subscribe to a specific topic
        await _messageBus.SubscribeAsync<OrderCreatedEvent>("orders.created.v1", HandleOrderCreatedAsync, stoppingToken);
        
        // Keep the service running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received OrderCreatedEvent for order {OrderId}", message.OrderId);

        using var scope = _scopeFactory.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        try
        {
            // Process the message
            await emailService.SendOrderConfirmationAsync(
                message.CustomerEmail, 
                message.OrderId, 
                cancellationToken);

            _logger.LogInformation("Order confirmation email sent for order {OrderId}", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process OrderCreatedEvent for order {OrderId}", message.OrderId);
            // Message will not be committed and will be reprocessed
            throw;
        }
    }
}

// Register the consumer
builder.Services.AddHostedService<OrderEventConsumer>();
```

### Integration Events Across Services

```csharp
// In Order Service - Publishing Integration Event
public class OrderCreatedIntegrationEvent : MessageBase
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime OrderDate { get; init; }
    public List<OrderItem> Items { get; init; } = new();
}

public class OrderApplicationService
{
    private readonly IMessageBus _messageBus;

    public async Task CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        // ... create order ...

        // Publish integration event for other services
        var integrationEvent = new OrderCreatedIntegrationEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            OrderDate = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };

        await _messageBus.PublishAsync(integrationEvent, "orders.integration.created", cancellationToken);
    }
}

// In Inventory Service - Consuming Integration Event
public class OrderCreatedEventHandler : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageBus.SubscribeAsync<OrderCreatedIntegrationEvent>(
            "orders.integration.created", 
            HandleOrderCreatedAsync, 
            stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleOrderCreatedAsync(OrderCreatedIntegrationEvent message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

        // Reserve inventory for the order
        await inventoryService.ReserveInventoryAsync(message.OrderId, message.Items, cancellationToken);
        
        _logger.LogInformation("Inventory reserved for order {OrderId}", message.OrderId);
    }
}
```

## Topic Naming Convention

By default, topics are automatically generated from the message type name (lowercase):

```csharp
// OrderCreatedEvent -> "ordercreatedevent"
await _messageBus.PublishAsync(new OrderCreatedEvent(), cancellationToken);

// For custom topics, specify explicitly:
await _messageBus.PublishAsync(new OrderCreatedEvent(), "orders.created.v1", cancellationToken);
```

## Message Headers

Each published message includes the following headers:

- **message-type**: Full type name of the message (for deserialization)
- **message-id**: Unique message identifier (GUID)
- **timestamp**: ISO 8601 timestamp
- **content-type**: "application/json"

## Error Handling

### Producer Errors

```csharp
try
{
    await _messageBus.PublishAsync(message, cancellationToken);
}
catch (InvalidOperationException ex)
{
    // Kafka producer error (broker unavailable, timeout, etc.)
    _logger.LogError(ex, "Failed to publish message to Kafka");
}
```

### Consumer Errors

If a message handler throws an exception, the message **will not be committed** and will be reprocessed:

```csharp
private async Task HandleMessageAsync(MyMessage message, CancellationToken cancellationToken)
{
    try
    {
        // Process message
        await ProcessMessageAsync(message, cancellationToken);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing message {MessageId}", message.Id);
        // Don't catch - let it bubble up so the message is not committed
        throw;
    }
}
```

## Advanced Configuration

### Consumer Group Strategy

For horizontal scaling, use the same `GroupId` across service instances:

```json
{
  "Kafka": {
    "GroupId": "order-service-group",  // Same for all instances
    "ClientId": "order-service-{hostname}"  // Unique per instance
  }
}
```

### Compression

Enable compression for better network utilization:

```json
{
  "Kafka": {
    "CompressionType": "snappy"  // Options: none, gzip, snappy, lz4, zstd
  }
}
```

### Exactly-Once Semantics

```json
{
  "Kafka": {
    "EnableIdempotence": true,
    "RequiredAcks": -1,  // Wait for all replicas
    "MaxInFlight": 5
  }
}
```

## Monitoring and Observability

The Kafka message bus integrates with the logging infrastructure and provides detailed logs:

```csharp
// Logs on message publish
[Information] Message published to Kafka - Topic: orders.created, Partition: 0, Offset: 12345, MessageId: abc-123

// Logs on message consume
[Debug] Consuming message from topic orders.created, partition 0, offset 12345, MessageId: abc-123
[Debug] Successfully processed message from topic orders.created, offset 12345, MessageId: abc-123

// Error logs
[Error] Failed to publish message to Kafka topic orders.created. Error: Broker not available
[Error] Error handling message from topic orders.created, partition 0, offset 12345, MessageId: abc-123
```

## Performance Considerations

1. **Batching**: Kafka automatically batches messages for better throughput
2. **Compression**: Use `snappy` or `lz4` for production workloads
3. **Partitioning**: Messages with the same key (Message.Id) go to the same partition
4. **Consumer Groups**: Scale horizontally by adding more consumer instances
5. **Offset Management**: Manual commit ensures at-least-once delivery

## Disposal and Cleanup

The `KafkaMessageBus` implements `IDisposable` and automatically:
- Closes all consumers gracefully
- Flushes pending producer messages
- Releases Kafka connections

This happens automatically when the application shuts down.

## Switching Between InMemory and Kafka

To switch between InMemory (for development) and Kafka (for production):

```json
// Development
{
  "Messaging": {
    "Provider": "InMemory"
  }
}

// Production
{
  "Messaging": {
    "Provider": "Kafka"
  },
  "Kafka": {
    "BootstrapServers": "kafka-prod:9092",
    "GroupId": "my-service-prod"
  }
}
```

No code changes required! The dependency injection container resolves the correct implementation based on configuration.

## Troubleshooting

### Issue: Messages not being consumed

**Solution**: Check that:
1. Consumer is subscribed before messages are published
2. `GroupId` is correctly configured
3. Kafka broker is accessible
4. Consumer is running in a background service

### Issue: Messages being processed multiple times

**Solution**: 
1. Ensure exceptions in handlers are not caught (let them bubble up)
2. Check that `EnableAutoCommit` is `false`
3. Verify message handlers are idempotent

### Issue: Connection timeout

**Solution**:
1. Verify `BootstrapServers` address
2. Check network connectivity to Kafka
3. Increase `RequestTimeoutMs` and `SessionTimeoutMs` if needed

## Testing

For unit tests, use the InMemory implementation:

```csharp
services.AddSingleton<IMessageBus, InMemoryMessageBus>();
```

For integration tests, use Testcontainers with Kafka:

```csharp
var kafka = new KafkaBuilder()
    .WithImage("confluentinc/cp-kafka:latest")
    .Build();

await kafka.StartAsync();

builder.Services.Configure<KafkaConfiguration>(options =>
{
    options.BootstrapServers = kafka.GetBootstrapAddress();
});
```

## Summary

The Kafka message broker implementation provides:

? Production-ready Kafka integration  
? Automatic configuration-based provider selection  
? Full message metadata and headers support  
? Consumer groups and horizontal scaling  
? Security and compression support  
? Comprehensive error handling and logging  
? Seamless switch between InMemory and Kafka  

For more information, see:
- [BuildingBlocks.Infrastructure README](../README.md)
- [Confluent.Kafka Documentation](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)
- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
