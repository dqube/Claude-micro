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

Register application services in `Program.cs`:

```csharp
using BuildingBlocks.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add all application layer services
builder.Services.AddApplicationLayer();

var app = builder.Build();
```

## Core Components

### CQRS Implementation

```csharp
// Commands
public record CreateOrderCommand(string CustomerId, List<OrderItem> Items) : ICommand<Guid>;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Implementation
        return Guid.NewGuid();
    }
}

// Queries
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

### Pagination Support

```csharp
public record GetOrdersQuery : PagedQuery, IQuery<PagedResult<OrderDto>>
{
    public string? Status { get; init; }
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

### Validation

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

## Key Services

### Pipeline Behaviors
- **IPipelineBehavior**: Base behavior interface
- **ValidationBehavior**: Request validation
- **LoggingBehavior**: Request/response logging
- **PerformanceBehavior**: Performance monitoring
- **TransactionBehavior**: Transaction management
- **CachingBehavior**: Response caching
- **RetryBehavior**: Automatic retry logic

### Caching Services
- **ICacheService**: Cache abstraction
- **CacheKey**: Structured cache keys
- **CachePolicy**: Cache expiration policies
- **CacheSettings**: Configuration options

### Messaging Services
- **IMessageBus**: Message publishing
- **IEventBus**: Event publishing
- **IMessageHandler**: Message processing
- **IEventHandler**: Event processing

### Inbox/Outbox Pattern
- **InboxMessage/OutboxMessage**: Message entities
- **IInboxService/IOutboxService**: Message processing
- **InboxProcessor/OutboxProcessor**: Background processors
- **InboxBackgroundService/OutboxBackgroundService**: Hosted services

### Saga Pattern
- **ISaga**: Saga interface
- **SagaBase<T>**: Base saga implementation
- **ISagaOrchestrator**: Saga orchestration
- **SagaStep**: Individual saga steps

## Dependencies

This library depends on:
- **BuildingBlocks.Domain**: Domain layer abstractions
- **Microsoft.Extensions.Caching.Memory**: Memory caching support
- **Microsoft.Extensions.Hosting.Abstractions**: Background service support
- **Microsoft.Extensions.Logging.Abstractions**: Logging support

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