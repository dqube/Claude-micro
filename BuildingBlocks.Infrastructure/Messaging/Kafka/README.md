# Kafka Message Broker Implementation

This document provides comprehensive information about the Kafka message broker implementation in BuildingBlocks.Infrastructure.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Configuration](#configuration)
- [Service Registration](#service-registration)
- [Publishing Messages](#publishing-messages)
- [Subscribing to Messages](#subscribing-to-messages)
- [Fluent Subscription Builder (KafkaMessageSubscriptionBuilder)](#fluent-subscription-builder-kafkamessagesubscriptionbuilder)
- [Advanced Patterns](#advanced-patterns)
- [Error Handling](#error-handling)
- [Performance & Best Practices](#performance--best-practices)
- [Troubleshooting](#troubleshooting)

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
- ? **Message Key Routing** - Uses message ID as Kafka key for partition affinity

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

### Configuration Parameters

| Parameter | Description | Default | Options |
|-----------|-------------|---------|---------|
| `BootstrapServers` | Kafka broker addresses | `localhost:9092` | Comma-separated list |
| `GroupId` | Consumer group ID | Required | Any string |
| `ClientId` | Client identifier | Required | Any string |
| `AutoOffsetReset` | Offset reset strategy | `earliest` | `earliest`, `latest`, `error` |
| `EnableAutoCommit` | Auto-commit offsets | `false` | `true`, `false` |
| `SessionTimeoutMs` | Session timeout | `30000` | Milliseconds |
| `MaxPollIntervalMs` | Max poll interval | `300000` | Milliseconds |
| `SecurityProtocol` | Security protocol | `Plaintext` | `Plaintext`, `Ssl`, `SaslPlaintext`, `SaslSsl` |
| `CompressionType` | Message compression | `none` | `none`, `gzip`, `snappy`, `lz4`, `zstd` |
| `RequiredAcks` | Acknowledgment mode | `-1` | `0`, `1`, `-1` (all replicas) |
| `EnableIdempotence` | Idempotent producer | `true` | `true`, `false` |

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
builder.Services.AddMessagingServices(builder.Configuration);

var app = builder.Build();
app.Run();
```

### Option 3: Programmatic Configuration

Configure Kafka without appsettings.json:

```csharp
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Messaging.Configuration;

var builder = WebApplication.CreateBuilder(args);

services.Configure<KafkaConfiguration>(options =>
{
    options.BootstrapServers = "localhost:9092";
    options.GroupId = "my-service-group";
    options.ClientId = "my-service";
    options.EnableAutoCommit = false;
    options.CompressionType = "snappy";
    options.EnableIdempotence = true;
});

builder.Services.AddSingleton<IMessageBus, KafkaMessageBus>();

var app = builder.Build();
app.Run();
```

## Publishing Messages

### 1. Basic Publishing

Publish a message to the default topic (derived from message type name):

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

        // Publish to default topic (ordercreatedevent)
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            CustomerEmail = request.CustomerEmail
        };

        await _messageBus.PublishAsync(orderEvent, cancellationToken);
        
        _logger.LogInformation("Order {OrderId} created and event published", order.Id);
    }
}
```

### 2. Publishing to Specific Topics

Publish to a custom topic name:

```csharp
public async Task PublishToCustomTopicAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken)
{
    // Publish to a specific topic with versioning
    await _messageBus.PublishAsync(orderEvent, "orders.created.v1", cancellationToken);
    
    _logger.LogInformation("Event published to orders.created.v1 topic");
}
```

### 3. Publishing with Metadata

Publish messages with rich metadata for correlation, tracing, and routing:

```csharp
public async Task PublishWithMetadataAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken)
{
    var metadata = new MessageEnvelop
    {
        MessageId = Guid.NewGuid().ToString(),
        CorrelationId = GetCorrelationId(), // From current request context
        CausationId = orderEvent.Id.ToString(),
        UserId = GetCurrentUserId(),
        TraceId = Activity.Current?.TraceId.ToString(),
        Source = "OrderService",
        Destination = "InventoryService",
        Priority = 1,
        TimeToLive = TimeSpan.FromHours(24)
    };
    
    // Add custom headers
    metadata.AddHeader("order-type", "online");
    metadata.AddHeader("region", "us-west");
    
    await _messageBus.PublishAsync(orderEvent, metadata, cancellationToken);
    
    _logger.LogInformation("Event published with correlation ID: {CorrelationId}", metadata.CorrelationId);
}
```

### 4. Batch Publishing

Publish multiple messages efficiently:

```csharp
public async Task PublishOrdersBatchAsync(List<Order> orders, CancellationToken cancellationToken)
{
    var events = orders.Select(order => new OrderCreatedEvent
    {
        OrderId = order.Id,
        TotalAmount = order.TotalAmount,
        CustomerEmail = order.CustomerEmail
    });
    
    // Kafka will batch these messages for optimal throughput
    await _messageBus.PublishBatchAsync(events, cancellationToken);
    
    _logger.LogInformation("Published batch of {Count} order events", orders.Count);
}
```

### 5. Scheduled Publishing

Schedule messages for future delivery:

```csharp
public async Task ScheduleOrderReminderAsync(Order order, CancellationToken cancellationToken)
{
    var reminderEvent = new OrderReminderEvent
    {
        OrderId = order.Id,
        CustomerEmail = order.CustomerEmail
    };
    
    // Schedule for 24 hours from now
    var scheduledTime = DateTime.UtcNow.AddHours(24);
    await _messageBus.ScheduleAsync(reminderEvent, scheduledTime, cancellationToken);
    
    _logger.LogInformation("Order reminder scheduled for {ScheduledTime}", scheduledTime);
}
```

## Subscribing to Messages

### 1. Basic Subscription with BackgroundService

The most common pattern for subscribing to Kafka messages:

```csharp
using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.Hosting;

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
        // Subscribe to default topic (ordercreatedevent)
        await _messageBus.SubscribeAsync<OrderCreatedEvent>(HandleOrderCreatedAsync, stoppingToken);
        
        _logger.LogInformation("OrderEventConsumer started");
        
        // Keep the service running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received OrderCreatedEvent for order {OrderId}", message.OrderId);

        // Create a new scope for scoped dependencies
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
            
            // Message will be committed automatically after successful processing
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process OrderCreatedEvent for order {OrderId}", message.OrderId);
            
            // IMPORTANT: Re-throw to prevent commit - message will be reprocessed
            throw;
        }
    }
}

// Register the consumer in Program.cs
builder.Services.AddHostedService<OrderEventConsumer>();
```

### 2. Subscribing to Specific Topics

Subscribe to a custom topic name:

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    // Subscribe to specific versioned topic
    await _messageBus.SubscribeAsync<OrderCreatedEvent>(
        "orders.created.v1", 
        HandleOrderCreatedAsync, 
        stoppingToken);
    
    _logger.LogInformation("Subscribed to orders.created.v1 topic");
    
    await Task.Delay(Timeout.Infinite, stoppingToken);
}
```

### 3. Subscription with Metadata

Access message metadata for correlation, tracing, and routing:

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    // Subscribe with metadata handler
    await _messageBus.SubscribeAsync<OrderCreatedEvent>(
        HandleOrderCreatedWithMetadataAsync, 
        stoppingToken);
    
    await Task.Delay(Timeout.Infinite, stoppingToken);
}

private async Task HandleOrderCreatedWithMetadataAsync(
    OrderCreatedEvent message, 
    MessageEnvelop metadata, 
    CancellationToken cancellationToken)
{
    _logger.LogInformation(
        "Received order {OrderId} with CorrelationId: {CorrelationId}, TraceId: {TraceId}",
        message.OrderId, metadata.CorrelationId, metadata.TraceId);

    using var scope = _scopeFactory.CreateScope();
    
    // Access metadata for context
    var correlationId = metadata.CorrelationId;
    var userId = metadata.UserId;
    var priority = metadata.Priority;
    var customHeader = metadata.GetHeader("order-type");
    
    // Set distributed tracing context
    if (!string.IsNullOrEmpty(metadata.TraceId))
    {
        Activity.Current?.SetTag("correlation.id", correlationId);
        Activity.Current?.SetTag("trace.id", metadata.TraceId);
    }
    
    var orderProcessor = scope.ServiceProvider.GetRequiredService<IOrderProcessor>();
    
    try
    {
        await orderProcessor.ProcessOrderAsync(message, correlationId, cancellationToken);
        
        _logger.LogInformation("Order {OrderId} processed successfully", message.OrderId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, 
            "Failed to process order {OrderId}, CorrelationId: {CorrelationId}", 
            message.OrderId, correlationId);
        throw;
    }
}
```

### 4. Multiple Subscriptions

Subscribe to multiple message types in a single consumer:

```csharp
public class MultiEventConsumer : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MultiEventConsumer> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Subscribe to multiple event types
        await _messageBus.SubscribeAsync<OrderCreatedEvent>(
            HandleOrderCreatedAsync, stoppingToken);
        
        await _messageBus.SubscribeAsync<OrderUpdatedEvent>(
            HandleOrderUpdatedAsync, stoppingToken);
        
        await _messageBus.SubscribeAsync<OrderCancelledEvent>(
            HandleOrderCancelledAsync, stoppingToken);
        
        _logger.LogInformation("MultiEventConsumer started with 3 subscriptions");
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var orderProcessor = scope.ServiceProvider.GetRequiredService<IOrderProcessor>();
        await orderProcessor.ProcessOrderAsync(message, cancellationToken);
    }

    private async Task HandleOrderUpdatedAsync(OrderUpdatedEvent message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var orderUpdater = scope.ServiceProvider.GetRequiredService<IOrderUpdater>();
        await orderUpdater.UpdateOrderAsync(message, cancellationToken);
    }

    private async Task HandleOrderCancelledAsync(OrderCancelledEvent message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var orderCanceller = scope.ServiceProvider.GetRequiredService<IOrderCanceller>();
        await orderCanceller.CancelOrderAsync(message, cancellationToken);
    }
}
```

### 5. Integration Events Across Microservices

**Publishing Service (OrderService):**

```csharp
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
    private readonly ILogger<OrderApplicationService> _logger;

    public async Task CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        // ... create order in database ...

        // Publish integration event for other microservices
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

        var metadata = new MessageEnvelop
        {
            CorrelationId = GetCorrelationId(),
            Source = "OrderService",
            Destination = "InventoryService",
            Priority = 1
        };

        await _messageBus.PublishAsync(
            integrationEvent, 
            "orders.integration.created", 
            metadata, 
            cancellationToken);
        
        _logger.LogInformation(
            "Integration event published for order {OrderId}", 
            order.Id);
    }
}
```

**Subscribing Service (InventoryService):**

```csharp
public class OrderCreatedEventHandler : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        IMessageBus messageBus,
        IServiceScopeFactory scopeFactory,
        ILogger<OrderCreatedEventHandler> logger)
    {
        _messageBus = messageBus;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageBus.SubscribeAsync<OrderCreatedIntegrationEvent>(
            "orders.integration.created", 
            HandleOrderCreatedAsync, 
            stoppingToken);
        
        _logger.LogInformation("Subscribed to orders.integration.created");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleOrderCreatedAsync(
        OrderCreatedIntegrationEvent message, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received order integration event for order {OrderId} from customer {CustomerId}",
            message.OrderId, message.CustomerId);

        using var scope = _scopeFactory.CreateScope();
        var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

        try
        {
            // Reserve inventory for the order
            await inventoryService.ReserveInventoryAsync(
                message.OrderId, 
                message.Items, 
                cancellationToken);
            
            _logger.LogInformation("Inventory reserved for order {OrderId}", message.OrderId);
        }
        catch (InsufficientInventoryException ex)
        {
            _logger.LogWarning(ex, 
                "Insufficient inventory for order {OrderId}", 
                message.OrderId);
            
            // Publish compensating event
            var compensatingEvent = new InventoryReservationFailedEvent
            {
                OrderId = message.OrderId,
                Reason = ex.Message
            };
            
            var eventBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            await eventBus.PublishAsync(
                compensatingEvent, 
                "inventory.reservation.failed", 
                cancellationToken);
            
            // Still throw to prevent commit and trigger retry
            throw;
        }
    }
}

// Register in Program.cs
builder.Services.AddHostedService<OrderCreatedEventHandler>();
```

### 6. Unsubscribing from Topics

Dynamically unsubscribe when needed:

```csharp
public class DynamicEventConsumer : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private bool _isSubscribed;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageBus.SubscribeAsync<OrderCreatedEvent>(
            HandleOrderCreatedAsync, stoppingToken);
        
        _isSubscribed = true;
        
        // Later, unsubscribe if needed
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        
        if (_isSubscribed)
        {
            await _messageBus.UnsubscribeAsync<OrderCreatedEvent>(stoppingToken);
            _isSubscribed = false;
            _logger.LogInformation("Unsubscribed from OrderCreatedEvent");
        }
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

// Or unsubscribe from specific topic
await _messageBus.UnsubscribeAsync("orders.created.v1", cancellationToken);
```

## Fluent Subscription Builder (KafkaMessageSubscriptionBuilder)

The `KafkaMessageSubscriptionBuilder` provides a fluent API for declaratively configuring multiple Kafka subscriptions during application startup. This approach is cleaner and more maintainable than manually calling `SubscribeAsync` multiple times.

### Benefits

? **Declarative Configuration** - Define all subscriptions in one place  
? **Auto-Discovery** - Automatically find and wire handlers from DI  
? **Type-Safe** - Compile-time checking of message and handler types  
? **Integration Events** - Built-in support for microservice integration events  
? **Centralized Setup** - Configure Kafka and subscriptions together  

### Basic Usage with AddMessageBus

Use the `AddMessageBus` extension method to configure both Kafka settings and subscriptions:

```csharp
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Messaging.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Kafka and subscriptions in one call
        services.AddMessageBus(
            options =>
            {
                // Kafka Configuration
                options.BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
                options.GroupId = "my-service-group";
                options.ClientId = "my-service";
                options.AutoOffsetReset = "earliest";
                options.EnableAutoCommit = false;
                options.CompressionType = "snappy";
                options.EnableIdempotence = true;
            },
            subscriptions =>
            {
                // Subscribe to multiple events declaratively
                subscriptions.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>();
                subscriptions.Subscribe<OrderUpdatedEvent, OrderUpdatedEventHandler>();
                subscriptions.Subscribe<OrderCancelledEvent, OrderCancelledEventHandler>();
            });

        return services;
    }
}
```

### Subscription Methods

#### 1. Subscribe with Auto-Discovery

Automatically discover handlers from DI container:

```csharp
services.AddMessageBus(
    options => { /* Kafka config */ },
    subscriptions =>
    {
        // Auto-discover handler for OrderCreatedEvent
        // Will search for IMessageHandler<OrderCreatedEvent> in DI
        subscriptions.Subscribe<OrderCreatedEvent>();
        
        // Auto-discover with custom topic
        subscriptions.Subscribe<OrderCreatedEvent>("orders.created.v1");
    });

// Register the handler in DI
builder.Services.AddScoped<IMessageHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
```

#### 2. Subscribe with Explicit Handler

Explicitly specify the handler class:

```csharp
services.AddMessageBus(
    options => { /* Kafka config */ },
    subscriptions =>
    {
        // Explicit handler - no auto-discovery needed
        subscriptions.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>();
        
        // Explicit handler with custom topic
        subscriptions.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>("orders.created.v1");
    });
```

#### 3. Subscribe to Integration Events

Subscribe to events from other microservices:

```csharp
services.AddMessageBus(
    options => { /* Kafka config */ },
    subscriptions =>
    {
        // Subscribe to integration event with topic and handler
        subscriptions.SubscribeIntegrationEvent<OrderCreatedIntegrationEvent, 
                                                 OrderCreatedIntegrationEventHandler>(
            "orders.integration.created");
        
        subscriptions.SubscribeIntegrationEvent<PaymentProcessedIntegrationEvent,
                                                 PaymentProcessedIntegrationEventHandler>(
            "payments.integration.processed");
    });
```

#### 4. Subscribe to Multiple Topics with Pattern

Subscribe to multiple topics using a single handler:

```csharp
services.AddMessageBus(
    options => { /* Kafka config */ },
    subscriptions =>
    {
        // One handler for multiple topic patterns
        subscriptions.SubscribeIntegrationEvents<GenericEventHandler>(
            "orders.*",
            "payments.*",
            "inventory.*"
        );
    });
```

### Complete Real-World Example (PatientService)

Here's a real example from the PatientService microservice:

**Infrastructure/DependencyInjection.cs:**

```csharp
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientService.Application.IntegrationEvents;
using PatientService.Application.IntegrationEvents.Handlers;

namespace PatientService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register repositories, DbContext, etc...

        // Configure Kafka Message Bus with Subscriptions
        services.AddMessageBus(
            options =>
            {
                // Kafka Connection Configuration
                options.BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
                options.GroupId = "patient-service-group";
                options.ClientId = "patient-service";
                options.AutoOffsetReset = "earliest";
                options.EnableAutoCommit = false;
                
                // Performance and Reliability
                options.CompressionType = "snappy";
                options.EnableIdempotence = true;
                options.SessionTimeoutMs = 30000;
                options.MaxPollIntervalMs = 300000;
                options.MessageTimeoutMs = 30000;
                options.RequestTimeoutMs = 30000;
                options.RequiredAcks = -1; // Wait for all replicas
            },
            subscriptions =>
            {
                // Subscribe to integration events from AppointmentService
                subscriptions.Subscribe<AppointmentCreatedIntegrationEvent,
                                       AppointmentCreatedIntegrationEventHandler>(
                    "appointment.created");
                
                // Subscribe to integration events from BillingService
                subscriptions.Subscribe<PaymentReceivedIntegrationEvent,
                                       PaymentReceivedIntegrationEventHandler>(
                    "billing.payment.received");
                
                // Subscribe to multiple notification events
                subscriptions.SubscribeIntegrationEvents<NotificationEventHandler>(
                    "notifications.email.*",
                    "notifications.sms.*"
                );
            });

        return services;
    }
}
```

**Application/IntegrationEvents/AppointmentCreatedIntegrationEvent.cs:**

```csharp
using BuildingBlocks.Application.CQRS.Messages;

namespace PatientService.Application.IntegrationEvents;

public class AppointmentCreatedIntegrationEvent : MessageBase
{
    public Guid AppointmentId { get; init; }
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public DateTime ScheduledAt { get; init; }
    public string AppointmentType { get; init; } = string.Empty;
}
```

**Application/IntegrationEvents/Handlers/AppointmentCreatedIntegrationEventHandler.cs:**

```csharp
using BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.Logging;
using PatientService.Domain.Entities;
using BuildingBlocks.Domain.Repository;
using PatientService.Domain.ValueObjects;

namespace PatientService.Application.IntegrationEvents.Handlers;

public class AppointmentCreatedIntegrationEventHandler 
    : IMessageHandler<AppointmentCreatedIntegrationEvent>
{
    private readonly IRepository<Patient, PatientId> _patientRepository;
    private readonly ILogger<AppointmentCreatedIntegrationEventHandler> _logger;

    public AppointmentCreatedIntegrationEventHandler(
        IRepository<Patient, PatientId> patientRepository,
        ILogger<AppointmentCreatedIntegrationEventHandler> logger)
    {
        _patientRepository = patientRepository;
        _logger = logger;
    }

    public async Task HandleAsync(
        AppointmentCreatedIntegrationEvent message, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received appointment created event for patient {PatientId}, appointment {AppointmentId}",
            message.PatientId, message.AppointmentId);

        // Get patient and update appointment history
        var patientId = new PatientId(message.PatientId);
        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

        if (patient == null)
        {
            _logger.LogWarning("Patient {PatientId} not found", message.PatientId);
            return;
        }

        // Update patient's appointment history (simplified example)
        // In real implementation, you might have an AppointmentHistory aggregate
        _logger.LogInformation(
            "Updated appointment history for patient {PatientId}",
            message.PatientId);

        await _patientRepository.UpdateAsync(patient, cancellationToken);
    }
}
```

**Program.cs - Activate Subscriptions:**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services including Kafka subscriptions
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Activate Kafka subscriptions on startup
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    
    // Get the subscription configuration action
    var subscriptionConfig = serviceProvider.GetService<Action<KafkaMessageSubscriptionBuilder>>();
    
    if (subscriptionConfig != null)
    {
        // Create subscription builder and activate all subscriptions
        await serviceProvider.ConfigureKafkaSubscriptionsAsync(
            subscriptionConfig, 
            app.Lifetime.ApplicationStopping);
    }
}

app.Run();
```

### Advanced Subscription Patterns

#### Conditional Subscriptions

Subscribe only when certain conditions are met:

```csharp
services.AddMessageBus(
    options => { /* config */ },
    subscriptions =>
    {
        // Subscribe only in production
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            subscriptions.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>();
        }
        
        // Subscribe only if feature flag is enabled
        if (configuration.GetValue<bool>("Features:EnableOrderProcessing"))
        {
            subscriptions.Subscribe<OrderProcessedEvent, OrderProcessedEventHandler>();
        }
    });
```

#### Multiple Handlers for Same Event

Register multiple handlers for the same event type:

```csharp
// Register multiple handlers in DI
builder.Services.AddScoped<IMessageHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
builder.Services.AddScoped<IMessageHandler<OrderCreatedEvent>, OrderCreatedAnalyticsHandler>();
builder.Services.AddScoped<IMessageHandler<OrderCreatedEvent>, OrderCreatedNotificationHandler>();

// Subscribe with auto-discovery - all handlers will be invoked
services.AddMessageBus(
    options => { /* config */ },
    subscriptions =>
    {
        subscriptions.Subscribe<OrderCreatedEvent>(); // All 3 handlers will process
    });
```

#### Versioned Topics

Handle different versions of events:

```csharp
services.AddMessageBus(
    options => { /* config */ },
    subscriptions =>
    {
        // v1 events - legacy
        subscriptions.Subscribe<OrderCreatedEventV1, OrderCreatedEventV1Handler>(
            "orders.created.v1");
        
        // v2 events - current
        subscriptions.Subscribe<OrderCreatedEventV2, OrderCreatedEventV2Handler>(
            "orders.created.v2");
        
        // v3 events - future
        subscriptions.Subscribe<OrderCreatedEventV3, OrderCreatedEventV3Handler>(
            "orders.created.v3");
    });
```

### Comparison: Builder vs Manual Subscription

**? Without Builder (Verbose, Hard to Maintain):**

```csharp
public class MultiEventConsumer : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceScopeFactory _scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Manually subscribe to each event
        await _messageBus.SubscribeAsync<OrderCreatedEvent>(
            "orders.created", HandleOrderCreatedAsync, stoppingToken);
        
        await _messageBus.SubscribeAsync<OrderUpdatedEvent>(
            "orders.updated", HandleOrderUpdatedAsync, stoppingToken);
        
        await _messageBus.SubscribeAsync<OrderCancelledEvent>(
            "orders.cancelled", HandleOrderCancelledAsync, stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    // Manually create handler methods
    private async Task HandleOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<OrderCreatedEvent>>();
        await handler.HandleAsync(message, cancellationToken);
    }
    
    // ... more handler methods ...
}

// Register background service
builder.Services.AddHostedService<MultiEventConsumer>();
```

**? With Builder (Clean, Declarative):**

```csharp
services.AddMessageBus(
    options => { /* config */ },
    subscriptions =>
    {
        // Declaratively configure all subscriptions
        subscriptions.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>("orders.created");
        subscriptions.Subscribe<OrderUpdatedEvent, OrderUpdatedEventHandler>("orders.updated");
        subscriptions.Subscribe<OrderCancelledEvent, OrderCancelledEventHandler>("orders.cancelled");
    });

// No background service needed!
// Handlers are registered in DI:
builder.Services.AddScoped<IMessageHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
builder.Services.AddScoped<IMessageHandler<OrderUpdatedEvent>, OrderUpdatedEventHandler>();
builder.Services.AddScoped<IMessageHandler<OrderCancelledEvent>, OrderCancelledEventHandler>();
```

### Activation Patterns

#### Option 1: Automatic Activation (Recommended)

Let the builder automatically activate subscriptions:

```csharp
var app = builder.Build();

// Activate subscriptions during startup
using (var scope = app.Services.CreateScope())
{
    var subscriptionConfig = scope.ServiceProvider
        .GetService<Action<KafkaMessageSubscriptionBuilder>>();
    
    if (subscriptionConfig != null)
    {
        await scope.ServiceProvider.ConfigureKafkaSubscriptionsAsync(
            subscriptionConfig, 
            app.Lifetime.ApplicationStopping);
    }
}

app.Run();
```

#### Option 2: Manual Activation with Extension Method

Use the extension method directly:

```csharp
var app = builder.Build();

await app.Services.ConfigureKafkaSubscriptionsAsync(subscriptions =>
{
    subscriptions.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>();
    subscriptions.Subscribe<OrderUpdatedEvent, OrderUpdatedEventHandler>();
}, app.Lifetime.ApplicationStopping);

app.Run();
```

#### Option 3: Explicit Builder Creation

Create and configure the builder manually:

```csharp
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var builder = scope.ServiceProvider.CreateKafkaSubscriptionBuilder();
    
    builder.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>()
           .Subscribe<OrderUpdatedEvent, OrderUpdatedEventHandler>()
           .Subscribe<OrderCancelledEvent, OrderCancelledEventHandler>();
    
    await builder.BuildAsync(app.Lifetime.ApplicationStopping);
}

app.Run();
```

### Error Handling in Subscriptions

The builder handles errors during subscription activation:

```csharp
services.AddMessageBus(
    options => { /* config */ },
    subscriptions =>
    {
        // If handler is not registered in DI, logs warning and continues
        subscriptions.Subscribe<OrderCreatedEvent>(); // Auto-discover
        
        // If handler is not found, throws exception during BuildAsync
        subscriptions.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>();
    });
```

**Error Scenarios:**
1. **Handler not registered in DI** - Throws `InvalidOperationException` during `BuildAsync`
2. **IMessageBus not registered** - Throws `InvalidOperationException` during `BuildAsync`
3. **Auto-discovery finds no handler** - Logs warning and skips subscription
4. **Handler has no HandleAsync method** - Throws `InvalidOperationException`

### Benefits Summary

| Feature | Manual Subscription | Fluent Builder |
|---------|-------------------|----------------|
| **Declarative Configuration** | ? | ? |
| **Auto-Discovery** | ? | ? |
| **Type Safety** | ? | ? |
| **Centralized Setup** | ? | ? |
| **Less Boilerplate** | ? | ? |
| **Integration Event Support** | ? | ? |
| **Multiple Topics/Patterns** | ? | ? |
| **Background Service Required** | ? | ? |

## Advanced Patterns

### Topic Naming Convention

By default, topics are automatically generated from the message type name (lowercase):

```csharp
// OrderCreatedEvent -> topic: "ordercreatedevent"
await _messageBus.PublishAsync(new OrderCreatedEvent(), cancellationToken);

// For custom topics with versioning and namespacing:
await _messageBus.PublishAsync(new OrderCreatedEvent(), "orders.created.v1", cancellationToken);
await _messageBus.PublishAsync(new OrderCreatedEvent(), "ecommerce.orders.created.v1", cancellationToken);
```

### Message Headers

Each published message includes comprehensive headers:

- **message-type**: Full type name of the message (for deserialization)
- **message-id**: Unique message identifier (GUID) - **also used as Kafka message key**
- **timestamp**: ISO 8601 timestamp
- **content-type**: "application/json"

With metadata, additional headers are included:
- **correlation-id**: For request correlation
- **causation-id**: For event causation tracking
- **user-id**: User who triggered the action
- **trace-id**: Distributed tracing ID
- **source**: Origin service/component
- **destination**: Target service/component
- **priority**: Message priority
- **ttl**: Time-to-live in seconds
- **custom headers**: Any additional headers you add

### Kafka Message Key

The message ID is used as the Kafka message key to ensure:

1. **Partition Affinity**: Messages with the same ID always go to the same partition
2. **Ordered Processing**: Messages for the same entity are processed in order
3. **Compaction**: Latest message per key is retained in compacted topics

```csharp
// Without metadata: Key = message.Id.ToString()
await _messageBus.PublishAsync(message, cancellationToken);

// With metadata: Key = metadata.MessageId
var metadata = new MessageEnvelop { MessageId = "custom-key" };
await _messageBus.PublishAsync(message, metadata, cancellationToken);
```

### Idempotent Message Handling

Ensure handlers are idempotent to safely handle reprocessing:

```csharp
private async Task HandleOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
{
    using var scope = _scopeFactory.CreateScope();
    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
    
    // Check if already processed (idempotency check)
    var existing = await orderRepository.GetByIdAsync(message.OrderId, cancellationToken);
    if (existing != null)
    {
        _logger.LogInformation("Order {OrderId} already processed, skipping", message.OrderId);
        return; // Message will be committed without reprocessing
    }
    
    // Process the order
    await orderRepository.CreateAsync(message.OrderId, message, cancellationToken);
    
    _logger.LogInformation("Order {OrderId} processed successfully", message.OrderId);
}
```

### Dead Letter Queue Pattern

Implement a dead letter queue for failed messages:

```csharp
private async Task HandleOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
{
    const int maxRetries = 3;
    var retryCount = GetRetryCount(message.Id); // From cache or database
    
    using var scope = _scopeFactory.CreateScope();
    var orderProcessor = scope.ServiceProvider.GetRequiredService<IOrderProcessor>();
    
    try
    {
        await orderProcessor.ProcessAsync(message, cancellationToken);
        ClearRetryCount(message.Id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to process order {OrderId}, retry {Retry}/{Max}", 
            message.OrderId, retryCount + 1, maxRetries);
        
        retryCount++;
        
        if (retryCount >= maxRetries)
        {
            // Send to dead letter queue
            var deadLetterEvent = new DeadLetterEvent
            {
                OriginalMessage = JsonSerializer.Serialize(message),
                ErrorMessage = ex.Message,
                RetryCount = retryCount,
                FailedAt = DateTime.UtcNow
            };
            
            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            await messageBus.PublishAsync(deadLetterEvent, "orders.dead-letter", cancellationToken);
            
            _logger.LogWarning("Order {OrderId} moved to dead letter queue after {Retries} retries", 
                message.OrderId, retryCount);
            
            // Clear retry count and commit to prevent infinite loop
            ClearRetryCount(message.Id);
            return;
        }
        
        SetRetryCount(message.Id, retryCount);
        throw; // Retry
    }
}
```

## Error Handling

### Producer Errors

Handle publishing failures gracefully:

```csharp
public async Task PublishWithRetryAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken)
{
    const int maxRetries = 3;
    var retryDelay = TimeSpan.FromSeconds(1);
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await _messageBus.PublishAsync(orderEvent, cancellationToken);
            _logger.LogInformation("Event published successfully on attempt {Attempt}", attempt);
            return;
        }
        catch (InvalidOperationException ex) when (attempt < maxRetries)
        {
            // Kafka producer error (broker unavailable, timeout, etc.)
            _logger.LogWarning(ex, 
                "Failed to publish message (attempt {Attempt}/{Max}). Retrying in {Delay}...",
                attempt, maxRetries, retryDelay);
            
            await Task.Delay(retryDelay, cancellationToken);
            retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, 30)); // Exponential backoff
        }
    }
    
    _logger.LogError("Failed to publish event after {MaxRetries} attempts", maxRetries);
    throw new InvalidOperationException($"Failed to publish event after {maxRetries} attempts");
}
```

### Consumer Errors

Handle consumption errors with proper logging and retries:

```csharp
private async Task HandleMessageAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
{
    using var scope = _scopeFactory.CreateScope();
    
    try
    {
        // Process message
        await ProcessMessageAsync(message, scope, cancellationToken);
    }
    catch (TemporaryException ex)
    {
        // Transient error - retry by throwing
        _logger.LogError(ex, "Temporary error processing message {MessageId}, will retry", message.Id);
        throw; // Message will NOT be committed and will be reprocessed
    }
    catch (PermanentException ex)
    {
        // Permanent error - log and skip
        _logger.LogError(ex, "Permanent error processing message {MessageId}, moving on", message.Id);
        
        // Send to dead letter queue
        await SendToDeadLetterQueueAsync(message, ex, scope, cancellationToken);
        
        // Don't throw - message will be committed and won't be reprocessed
        return;
    }
    catch (Exception ex)
    {
        // Unknown error - treat as transient
        _logger.LogError(ex, "Unknown error processing message {MessageId}", message.Id);
        throw; // Retry
    }
}
```

### Graceful Shutdown

Handle graceful shutdown in your consumer:

```csharp
public class OrderEventConsumer : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OrderEventConsumer> _logger;
    private readonly IHostApplicationLifetime _lifetime;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _lifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogInformation("Application stopping - completing message processing...");
        });

        try
        {
            await _messageBus.SubscribeAsync<OrderCreatedEvent>(HandleOrderCreatedAsync, stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OrderEventConsumer cancelled");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OrderEventConsumer stopping...");
        
        // Unsubscribe from all topics
        await _messageBus.UnsubscribeAsync<OrderCreatedEvent>(cancellationToken);
        
        await base.StopAsync(cancellationToken);
        
        _logger.LogInformation("OrderEventConsumer stopped");
    }
}
```

## Performance & Best Practices

### Consumer Group Strategy

For horizontal scaling, use the same `GroupId` across service instances:

```json
{
  "Kafka": {
    "GroupId": "order-service-group",  // Same for all instances
    "ClientId": "order-service-instance-{hostname}"  // Unique per instance
  }
}
```

Multiple instances of the same consumer group will automatically distribute partition load.

### Compression

Enable compression for better network utilization:

```json
{
  "Kafka": {
    "CompressionType": "snappy"  // Options: none, gzip, snappy, lz4, zstd
  }
}
```

**Recommended compression types:**
- **snappy**: Fast, good balance (recommended for most cases)
- **lz4**: Fastest, lowest CPU
- **gzip**: Best compression ratio, higher CPU
- **zstd**: Best modern compression (Kafka 2.1+)

### Exactly-Once Semantics

Configure for exactly-once message delivery:

```json
{
  "Kafka": {
    "EnableIdempotence": true,
    "RequiredAcks": -1,  // Wait for all replicas
    "MaxInFlight": 5,
    "EnableAutoCommit": false
  }
}
```

### Batching

Kafka automatically batches messages for better throughput. For high-volume scenarios:

```csharp
// Publish individual messages - Kafka will batch them automatically
for (int i = 0; i < 1000; i++)
{
    await _messageBus.PublishAsync(new OrderEvent { OrderId = i }, cancellationToken);
}

// Or use explicit batching for better control
var events = Enumerable.Range(0, 1000).Select(i => new OrderEvent { OrderId = i });
await _messageBus.PublishBatchAsync(events, cancellationToken);
```

### Partitioning Strategy

Messages with the same key (Message.Id) go to the same partition, ensuring:

1. **Order preservation** for related messages
2. **Cache locality** in consumers
3. **Efficient compaction** for stateful processing

```csharp
// All messages for the same order go to the same partition
var orderEvent1 = new OrderUpdatedEvent { OrderId = orderId };
var orderEvent2 = new OrderShippedEvent { OrderId = orderId };
var orderEvent3 = new OrderDeliveredEvent { OrderId = orderId };

// These will all have the same message.Id, thus same partition
await _messageBus.PublishAsync(orderEvent1, cancellationToken);
await _messageBus.PublishAsync(orderEvent2, cancellationToken);
await _messageBus.PublishAsync(orderEvent3, cancellationToken);
```

### Offset Management

Manual commit ensures at-least-once delivery:

```json
{
  "Kafka": {
    "EnableAutoCommit": false  // Manual commit for guaranteed processing
  }
}
```

The message is committed only after successful handler execution. If the handler throws an exception, the message will be reprocessed.

### Monitoring and Observability

The Kafka message bus integrates with logging infrastructure and provides detailed logs:

```csharp
// Logs on message publish
[Debug] Message published to Kafka - Topic: orders.created, Partition: 0, Offset: 12345, MessageId: abc-123

// Logs on message consume
[Debug] Consuming message from topic orders.created, partition 0, offset 12345, MessageId: abc-123
[Debug] Successfully processed message from topic orders.created, offset 12345, MessageId: abc-123

// Error logs
[Error] Failed to publish message to Kafka topic orders.created. Error: Broker not available
[Error] Error handling message from topic orders.created, partition 0, offset 12345, MessageId: abc-123
```

Add custom metrics:

```csharp
private static readonly Counter<long> MessageProcessedCounter = 
    Metrics.CreateCounter<long>("messages_processed", "Messages processed count");

private static readonly Histogram<double> MessageProcessingDuration =
    Metrics.CreateHistogram<double>("message_processing_duration", "Message processing duration in ms");

private async Task HandleOrderCreatedAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
{
    var startTime = Stopwatch.GetTimestamp();
    
    try
    {
        await ProcessMessageAsync(message, cancellationToken);
        
        MessageProcessedCounter.Add(1, new KeyValuePair<string, object?>("status", "success"));
    }
    catch (Exception ex)
    {
        MessageProcessedCounter.Add(1, new KeyValuePair<string, object?>("status", "error"));
        throw;
    }
    finally
    {
        var duration = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
        MessageProcessingDuration.Record(duration);
    }
}
```

## Troubleshooting

### Issue: Messages not being consumed

**Symptoms**: Messages are published but not consumed

**Solutions**:
1. Check that consumer is subscribed **before** messages are published
2. Verify `GroupId` is correctly configured in appsettings.json
3. Ensure Kafka broker is accessible from consumer
4. Confirm consumer is running as a `BackgroundService` or hosted service
5. Check consumer logs for subscription confirmation

```bash
# Check Kafka broker connectivity
telnet kafka.example.com 9092

# View consumer group status
kafka-consumer-groups --bootstrap-server localhost:9092 --group order-service-group --describe
```

### Issue: Messages being processed multiple times

**Symptoms**: Same message handled multiple times

**Solutions**:
1. **Don't catch exceptions** in handlers - let them bubble up to prevent commit
2. Ensure `EnableAutoCommit` is `false`
3. Implement **idempotent handlers** to safely handle reprocessing
4. Add idempotency checks using message ID

```csharp
// ? BAD - Catching exception commits the message
catch (Exception ex)
{
    _logger.LogError(ex, "Error");
    return; // Message committed even though processing failed
}

// ? GOOD - Letting exception bubble up prevents commit
catch (Exception ex)
{
    _logger.LogError(ex, "Error");
    throw; // Message NOT committed, will be reprocessed
}
```

### Issue: Connection timeout

**Symptoms**: `TimeoutException` or connection errors

**Solutions**:
1. Verify `BootstrapServers` address is correct
2. Check network connectivity to Kafka brokers
3. Increase timeout values:

```json
{
  "Kafka": {
    "RequestTimeoutMs": 60000,
    "SessionTimeoutMs": 45000,
    "MaxPollIntervalMs": 600000
  }
}
```

4. Check firewall rules allow connection
5. Verify Kafka broker is running: `docker ps` or `systemctl status kafka`

### Issue: Slow message processing

**Symptoms**: Consumer lag increasing

**Solutions**:
1. Increase `MaxPollIntervalMs` for long-running handlers
2. Add more consumer instances for horizontal scaling
3. Optimize handler processing logic
4. Use batch processing where possible
5. Consider async processing with worker queues

```csharp
// Process in parallel with controlled concurrency
private async Task HandleBatchAsync(List<OrderCreatedEvent> messages, CancellationToken cancellationToken)
{
    var options = new ParallelOptions 
    { 
        MaxDegreeOfParallelism = 4,
        CancellationToken = cancellationToken
    };
    
    await Parallel.ForEachAsync(messages, options, async (message, ct) =>
    {
        await ProcessMessageAsync(message, ct);
    });
}
```

### Issue: Deserialization errors

**Symptoms**: `Failed to deserialize message` warnings

**Solutions**:
1. Ensure message classes have **parameterless constructors**
2. Use **consistent JSON naming** between publisher and subscriber
3. Handle schema evolution with backward compatibility
4. Add version headers to messages

```csharp
// ? Good message class
public class OrderCreatedEvent : MessageBase
{
    public Guid OrderId { get; init; }
    public decimal TotalAmount { get; init; }
    
    // Parameterless constructor for deserialization
    public OrderCreatedEvent() { }
}
```

## Switching Between InMemory and Kafka

To switch between InMemory (for development) and Kafka (for production):

**Development:**
```json
{
  "Messaging": {
    "Provider": "InMemory"
  }
}
```

**Production:**
```json
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

**No code changes required!** The dependency injection container resolves the correct implementation based on configuration.

## Testing

### Unit Tests

Use the InMemory implementation for unit tests:

```csharp
public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrder_ShouldPublishEvent()
    {
        // Arrange
        var messageBus = new InMemoryMessageBus(
            new NullLogger<InMemoryMessageBus>());
        
        var service = new OrderService(messageBus, Mock.Of<ILogger<OrderService>>());
        
        // Act
        await service.CreateOrderAsync(new CreateOrderRequest(), CancellationToken.None);
        
        // Assert
        // Verify event was published
    }
}
```

### Integration Tests

Use Testcontainers with Kafka for integration tests:

```csharp
public class KafkaIntegrationTests : IAsyncLifetime
{
    private readonly KafkaContainer _kafkaContainer;
    private IServiceProvider _serviceProvider;

    public KafkaIntegrationTests()
    {
        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:latest")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _kafkaContainer.StartAsync();
        
        var services = new ServiceCollection();
        services.Configure<KafkaConfiguration>(options =>
        {
            options.BootstrapServers = _kafkaContainer.GetBootstrapAddress();
            options.GroupId = "test-group";
            options.ClientId = "test-client";
        });
        
        services.AddSingleton<IMessageBus, KafkaMessageBus>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task PublishAndConsume_ShouldWork()
    {
        // Arrange
        var messageBus = _serviceProvider.GetRequiredService<IMessageBus>();
        var received = false;
        
        await messageBus.SubscribeAsync<OrderCreatedEvent>(async (msg, ct) =>
        {
            received = true;
        });
        
        // Act
        await messageBus.PublishAsync(new OrderCreatedEvent { OrderId = Guid.NewGuid() });
        
        // Wait for message processing
        await Task.Delay(2000);
        
        // Assert
        Assert.True(received);
    }

    public async Task DisposeAsync()
    {
        await _kafkaContainer.StopAsync();
    }
}
```

## Summary

The Kafka message broker implementation provides:

? **Production-ready Kafka integration** with Confluent.Kafka  
? **Automatic configuration-based provider selection** (Kafka/InMemory)  
? **Full message metadata and headers support** for tracing and correlation  
? **Consumer groups and horizontal scaling** for high throughput  
? **Security and compression support** for production environments  
? **Comprehensive error handling and logging** for reliability  
? **Seamless switch between InMemory and Kafka** without code changes  
? **Message key routing** using message ID for partition affinity  
? **At-least-once delivery** with manual offset commit  
? **Idempotent producer** for exactly-once semantics  

## Additional Resources

For more information, see:
- [BuildingBlocks.Infrastructure README](../../README.md)
- [Confluent.Kafka Documentation](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)
- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
- [Kafka Best Practices](https://kafka.apache.org/documentation/#bestpractices)

---

**Need Help?** Check the [Troubleshooting](#troubleshooting) section or review the comprehensive examples above for common patterns and solutions.
