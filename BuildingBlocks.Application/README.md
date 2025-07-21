# BuildingBlocks.Application

A comprehensive application layer library providing essential building blocks for implementing clean architecture patterns in .NET applications.

## Features

### 🏗️ CQRS Support
- **Commands**: Command handlers and base classes
- **Queries**: Query handlers with pagination and sorting support
- **Events**: Domain and integration event handling
- **Messages**: Message handling abstractions

### 🔄 Pipeline Behaviors
- **Logging**: Automatic request/response logging
- **Validation**: Request validation pipeline
- **Performance Monitoring**: Execution time tracking
- **Transaction Management**: Automatic transaction handling
- **Caching**: Request/response caching
- **Retry**: Automatic retry mechanisms

### 📨 Messaging & Event Handling
- **Inbox/Outbox Pattern**: Reliable message processing
- **Event Bus**: Event publishing and handling
- **Message Bus**: Generic message handling
- **Background Services**: Automated message processing

### 🔐 Security & Context
- **User Context**: Current user information
- **Security Context**: Security-related services
- **Permission Service**: Authorization support

### ⚡ Caching
- **Memory Cache**: In-memory caching support
- **Distributed Cache**: Redis caching support
- **Cache Policies**: Configurable caching strategies

### 📝 Validation & Mapping
- **Validation Framework**: Flexible validation rules
- **Mapping Support**: Object mapping abstractions
- **DTOs**: Data transfer object base classes

### 🔄 Saga Pattern
- **Saga Orchestration**: Long-running process management
- **Compensation**: Automatic rollback support
- **State Management**: Saga state persistence

## Installation

Add the project reference to your application:

```xml
<ProjectReference Include="..\BuildingBlocks.Application\BuildingBlocks.Application.csproj" />
```

## Quick Start

### 1. Register Application Services

In your `Program.cs` or `Startup.cs`:

```csharp
using BuildingBlocks.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add all application layer services
builder.Services.AddApplicationLayer();

// Or add specific features
builder.Services.AddValidation();
builder.Services.AddLogging();
builder.Services.AddPerformanceMonitoring();
builder.Services.AddTransactionSupport();
builder.Services.AddSecurityContext();
builder.Services.AddCaching();

var app = builder.Build();
```

### 2. Add Caching Support

```csharp
// Memory caching
builder.Services.AddCaching();

// Distributed caching with Redis
builder.Services.AddDistributedCaching("localhost:6379");
```

### 3. Add Inbox/Outbox Pattern

```csharp
// Both inbox and outbox
builder.Services.AddInboxOutboxSupport();

// Or individually
builder.Services.AddInboxSupport();
builder.Services.AddOutboxSupport();
```

### 4. Add Saga Support

```csharp
// General saga support
builder.Services.AddSagas();

// Register specific saga
builder.Services.AddSaga<OrderSaga, OrderSagaData>();
```

## Usage Examples

### Creating Commands

```csharp
public record CreateOrderCommand(string CustomerId, List<OrderItem> Items) : ICommand<Guid>;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Implementation
        return Guid.NewGuid();
    }
}
```

### Creating Queries

```csharp
public record GetOrderQuery(Guid OrderId) : IQuery<OrderDto>;

public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        // Implementation
        return new OrderDto();
    }
}
```

### Using Pagination

```csharp
public record GetOrdersQuery : PagedQuery, IQuery<PagedResult<OrderDto>>
{
    public string? Status { get; init; }
}

public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        // Implementation with pagination
        return new PagedResult<OrderDto>
        {
            Items = orders,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
```

### Event Handling

```csharp
public record OrderCreatedEvent(Guid OrderId, string CustomerId) : IEvent;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Handle the event
    }
}
```

### Custom Validation

```csharp
public class CreateOrderValidator : ValidatorBase<CreateOrderCommand>
{
    public override async Task<ValidationResult> ValidateAsync(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var result = new ValidationResult();
        
        if (string.IsNullOrEmpty(request.CustomerId))
        {
            result.Errors.Add(new ValidationError("CustomerId", "Customer ID is required"));
        }
        
        return result;
    }
}
```

### Saga Implementation

```csharp
public class OrderSaga : SagaBase<OrderSagaData>
{
    public OrderSaga(Guid id, OrderSagaData data) : base(id, data) { }

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // Start saga logic
        Status = SagaStatus.Running;
        await ProcessPayment();
    }

    public override async Task CompensateAsync(CancellationToken cancellationToken = default)
    {
        // Compensation logic
        Status = SagaStatus.Compensating;
        await RefundPayment();
    }
}
```

## Configuration

### Cache Settings

```csharp
public class CacheSettings
{
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public bool EnableCaching { get; set; } = true;
}
```

### Inbox/Outbox Options

```csharp
public class InboxOptions
{
    public TimeSpan ProcessingInterval { get; set; } = TimeSpan.FromSeconds(30);
    public int BatchSize { get; set; } = 100;
}

public class OutboxOptions
{
    public TimeSpan ProcessingInterval { get; set; } = TimeSpan.FromSeconds(30);
    public int BatchSize { get; set; } = 100;
}
```

## Extension Methods

### Complete Registration Example

```csharp
var builder = WebApplication.CreateBuilder(args);

// Core application services
builder.Services.AddApplicationLayer();

// Security
builder.Services.AddSecurityContext();

// Caching
builder.Services.AddCaching();
builder.Services.AddDistributedCaching(builder.Configuration.GetConnectionString("Redis"));

// Messaging
builder.Services.AddInboxOutboxSupport();

// Sagas
builder.Services.AddSagas();
builder.Services.AddSaga<OrderSaga, OrderSagaData>();
builder.Services.AddSaga<PaymentSaga, PaymentSagaData>();

var app = builder.Build();

// Use application security
app.UseApplicationSecurity();
```

## Dependencies

This library depends on:
- **BuildingBlocks.Domain**: Domain layer abstractions
- **Microsoft.Extensions.Caching.Memory**: Memory caching support
- **Microsoft.Extensions.Caching.StackExchangeRedis**: Redis caching support
- **Microsoft.Extensions.Hosting.Abstractions**: Background service support
- **Microsoft.Extensions.Logging.Abstractions**: Logging support
- **Scrutor**: Assembly scanning for automatic registration

## Architecture Integration

This library is designed to work as part of a clean architecture solution:

```
📁 YourApplication/
├── 📁 Domain/ (BuildingBlocks.Domain)
├── 📁 Application/ (BuildingBlocks.Application) ← This library
├── 📁 Infrastructure/
├── 📁 API/
└── 📁 Tests/
```

## Best Practices

1. **Use CQRS**: Separate commands and queries for better separation of concerns
2. **Pipeline Behaviors**: Leverage behaviors for cross-cutting concerns
3. **Event-Driven**: Use events for loose coupling between bounded contexts
4. **Validation**: Implement validation at the application layer
5. **Caching**: Use caching behaviors for read-heavy operations
6. **Sagas**: Use sagas for complex, long-running business processes
7. **Inbox/Outbox**: Use for reliable message processing and eventual consistency