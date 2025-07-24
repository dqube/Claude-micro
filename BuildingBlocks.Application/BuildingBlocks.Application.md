# BuildingBlocks.Application - Library Structure

A comprehensive application layer building blocks library for .NET applications following Clean Architecture principles with CQRS, Event Sourcing, and Domain-Driven Design patterns.

## 🏗️ Architecture Overview

This library provides the essential application layer components for building robust, scalable applications using:

- **CQRS Pattern**: Command Query Responsibility Segregation with mediator
- **Event-Driven Architecture**: Domain and integration events with reliable processing
- **Pipeline Behaviors**: Cross-cutting concerns (logging, validation, caching, etc.)
- **Inbox/Outbox Pattern**: Reliable message processing with exactly-once delivery
- **Saga Pattern**: Long-running process orchestration with compensation
- **Clean Architecture**: Separation of concerns and dependency inversion

## Directory Structure

```
BuildingBlocks.Application/
├── Behaviors/                                  # Pipeline behaviors for cross-cutting concerns
│   ├── CachingBehavior.cs                     # Response caching with policy support
│   ├── IPipelineBehavior.cs                   # Base pipeline behavior interface
│   ├── LoggingBehavior.cs                     # Request/response logging with performance metrics
│   ├── PerformanceBehavior.cs                 # Execution time monitoring with thresholds
│   ├── RetryBehavior.cs                       # Configurable retry mechanisms with backoff
│   ├── TransactionBehavior.cs                 # Automatic transaction management
│   └── ValidationBehavior.cs                  # Request validation with error aggregation
├── Caching/                                   # Advanced caching infrastructure
│   ├── CacheKey.cs                            # Structured cache key implementation
│   ├── CachePolicy.cs                         # Cache expiration and priority policies
│   ├── CacheSettings.cs                       # Configurable cache settings
│   ├── ICacheKey.cs                           # Cache key abstraction interface
│   └── ICacheService.cs                       # Cache service abstraction
├── CQRS/                                      # Command Query Responsibility Segregation
│   ├── Commands/                              # Command handling components
│   │   ├── CommandBase.cs                     # Base command with common properties
│   │   ├── ICommand.cs                        # Command interfaces (void and with result)
│   │   └── ICommandHandler.cs                 # Command handler interfaces
│   ├── Events/                                # Event handling components
│   │   ├── DomainEventNotification.cs         # Domain event notification wrapper
│   │   ├── IEvent.cs                          # Event marker interface
│   │   ├── IEventHandler.cs                   # Event handler interface
│   │   ├── IIntegrationEvent.cs               # Integration event marker interface
│   │   └── IntegrationEventBase.cs            # Base integration event with metadata
│   ├── Mediator/                              # Mediator pattern implementation
│   │   ├── IMediator.cs                       # Mediator interface for request routing
│   │   └── Mediator.cs                        # Mediator implementation with pipeline support
│   ├── Messages/                              # Message handling components
│   │   ├── IMessage.cs                        # Base message interface
│   │   ├── IMessageContext.cs                 # Message context for metadata
│   │   ├── IStreamMessage.cs                  # Streaming message interface
│   │   └── MessageBase.cs                     # Base message implementation
│   └── Queries/                               # Query handling components
│       ├── IQuery.cs                          # Query interface with required response
│       ├── IQueryHandler.cs                   # Query handler interface
│       ├── PagedQuery.cs                      # Base paged query with sorting
│       ├── PagedResult.cs                     # Paged result container
│       ├── QueryBase.cs                       # Base query with common properties
│       └── SortingQuery.cs                    # Sorting query base with direction
├── Dispatchers/                               # Dedicated message dispatchers
│   ├── CommandDispatcher.cs                   # Command dispatching implementation
│   ├── EventDispatcher.cs                     # Event dispatching implementation
│   ├── ICommandDispatcher.cs                  # Command dispatcher interface
│   ├── IEventDispatcher.cs                    # Event dispatcher interface
│   ├── IMessageDispatcher.cs                  # Generic message dispatcher interface
│   ├── IQueryDispatcher.cs                    # Query dispatcher interface
│   ├── MessageDispatcher.cs                   # Generic message dispatching implementation
│   └── QueryDispatcher.cs                     # Query dispatching implementation
├── DTOs/                                      # Data Transfer Objects
│   ├── AuditableDto.cs                        # Base DTO with audit fields
│   ├── BaseDto.cs                             # Base DTO with common properties
│   └── PagedDto.cs                            # Base paged DTO
├── Extensions/                                # Dependency injection and configuration extensions
│   ├── ApplicationExtensions.cs               # Application-wide extension methods
│   ├── MediatorExtensions.cs                  # Mediator registration extensions
│   └── ServiceCollectionExtensions.cs         # Service collection registration extensions
├── Inbox/                                     # Inbox pattern for reliable message processing
│   ├── IInboxMessageHandler.cs                # Inbox message handler interface
│   ├── IInboxProcessor.cs                     # Inbox processor interface
│   ├── IInboxService.cs                       # Inbox service interface
│   ├── InboxMessage.cs                        # Inbox message entity
│   ├── InboxMessageStatus.cs                  # Message processing status enumeration
│   └── InboxProcessor.cs                      # Inbox processor implementation
├── Mapping/                                   # Object mapping abstractions
│   ├── IMapper.cs                             # Mapper interface with async support
│   ├── IMappingProfile.cs                     # Mapping profile interface
│   └── MapperBase.cs                          # Base mapper implementation
├── Messaging/                                 # Message bus and event bus infrastructure
│   ├── IEventBus.cs                           # Event bus interface for domain/integration events
│   ├── IMessageBus.cs                         # Generic message bus interface
│   ├── IMessageHandler.cs                     # Message handler interface
│   ├── IMessagePublisher.cs                   # Message publisher interface
│   └── MessageMetadata.cs                     # Message metadata container
├── Outbox/                                    # Outbox pattern for reliable message publishing
│   ├── IOutboxProcessor.cs                    # Outbox processor interface
│   ├── IOutboxService.cs                      # Outbox service interface
│   ├── OutboxMessage.cs                       # Outbox message entity
│   ├── OutboxMessageStatus.cs                 # Message publishing status enumeration
│   └── OutboxProcessor.cs                     # Outbox processor implementation
├── Sagas/                                     # Saga pattern for long-running processes
│   ├── ISaga.cs                               # Saga interface
│   ├── ISagaOrchestrator.cs                   # Saga orchestrator interface
│   ├── ISagaRepository.cs                     # Saga repository interface
│   ├── SagaBase.cs                            # Base saga implementation with compensation
│   ├── SagaExtensions.cs                      # Saga helper extensions
│   └── SagaStep.cs                            # Individual saga step definition
├── Security/                                  # Security and authorization components
│   ├── ICurrentUserService.cs                 # Current user service interface
│   ├── IPermissionService.cs                  # Permission service interface
│   ├── SecurityContext.cs                     # Security context with multi-tenant support
│   └── UserContext.cs                         # User context with roles and organization
├── Services/                                  # Application services and background processing
│   ├── ApplicationServiceBase.cs              # Base application service with common functionality
│   ├── DomainEventService.cs                  # Domain event publishing service
│   ├── IApplicationService.cs                 # Application service interface
│   ├── IDomainEventService.cs                 # Domain event service interface
│   ├── InboxBackgroundService.cs              # Hosted service for inbox processing
│   └── OutboxBackgroundService.cs             # Hosted service for outbox processing
├── Validation/                                # Validation framework
│   ├── CompositeValidator.cs                  # Composite validator for multiple rules
│   ├── IValidationRule.cs                     # Validation rule interface
│   ├── IValidator.cs                          # Validator interface
│   ├── ValidationResult.cs                    # Validation result with errors
│   └── ValidatorBase.cs                       # Base validator implementation
├── BuildingBlocks.Application.csproj          # Project file
├── BuildingBlocks.Application.md              # This structure documentation file
└── README.md                                  # Comprehensive usage documentation
```

## Component Categories

### 🏗️ CQRS Infrastructure
- **Commands/**: Write operations with handlers and base classes
- **Queries/**: Read operations with pagination, sorting, and filtering
- **Events/**: Domain and integration event handling
- **Messages/**: Generic message handling with streaming support
- **Mediator/**: Central request routing with pipeline orchestration

### 🔄 Cross-Cutting Concerns
- **Behaviors/**: Pipeline behaviors for logging, validation, caching, retry, performance, transactions
- **Validation/**: Rule-based validation framework with composite support
- **Caching/**: Advanced caching with policies, keys, and invalidation strategies
- **Security/**: User context, permissions, and multi-tenant security

### 📨 Messaging & Reliability
- **Inbox/**: Reliable message processing with exactly-once delivery
- **Outbox/**: Transactional message publishing with background processing
- **Messaging/**: Event bus and message bus abstractions
- **Dispatchers/**: Dedicated dispatchers for different message types

### 🔄 Process Orchestration
- **Sagas/**: Long-running process management with compensation
- **Services/**: Application services and background processing
- **Background Services**: Continuous processing for inbox/outbox

### 🔧 Infrastructure Support
- **DTOs/**: Data transfer objects with audit support
- **Mapping/**: Object mapping abstractions
- **Extensions/**: Dependency injection and configuration helpers

## Key Features by Directory

### Behaviors/
- **Pipeline Processing**: Cross-cutting concerns applied automatically to all requests
- **Configurable Behaviors**: Enable/disable behaviors based on configuration
- **Performance Monitoring**: Track execution times with configurable thresholds
- **Intelligent Caching**: Cache responses based on query types and policies
- **Automatic Validation**: Validate requests before processing
- **Retry Logic**: Configurable retry with exponential backoff
- **Transaction Management**: Automatic transaction handling for commands

### CQRS/
- **Command Handling**: Support for commands with and without results
- **Query Processing**: Read operations with pagination and sorting
- **Event Processing**: Domain and integration event handling
- **Mediator Pattern**: Central request routing with pipeline support
- **Message Abstraction**: Generic message handling framework

### Caching/
- **Policy-Based Caching**: Flexible caching policies with TTL and sliding expiration
- **Hierarchical Keys**: Structured cache keys with tagging support
- **Invalidation Strategies**: Tag-based and key-based cache invalidation
- **Configuration Support**: Environment-specific cache settings

### Inbox/Outbox/
- **Exactly-Once Processing**: Ensures messages are processed only once
- **Transactional Publishing**: Messages published as part of database transactions
- **Background Processing**: Continuous processing with configurable intervals
- **Error Handling**: Dead letter queues and retry mechanisms
- **Status Tracking**: Complete message lifecycle tracking

### Sagas/
- **Process Orchestration**: Coordinate multiple services in distributed transactions
- **Compensation Logic**: Automatic rollback of completed steps on failure
- **State Management**: Persistent saga state with recovery support
- **Step Definition**: Declarative step definition with execute/compensate actions

### Security/
- **User Context**: Current user information with roles and permissions
- **Multi-Tenant Support**: Organization and tenant-aware security
- **Permission System**: Role-based and attribute-based authorization
- **Security Context**: Centralized security information management

### Validation/
- **Rule-Based Validation**: Composable validation rules
- **Async Validation**: Support for async validation scenarios
- **Error Aggregation**: Collect multiple validation errors
- **Composite Validation**: Combine multiple validators

This structure provides a comprehensive foundation for building enterprise applications with modern architectural patterns, ensuring scalability, maintainability, and reliability through proven patterns like CQRS, Event Sourcing, Saga, and Inbox/Outbox.

## 🚀 Key Features

### 🎯 CQRS Implementation

#### Commands
Commands represent write operations and business actions:

```csharp
// Command without result
public record CreateUserCommand(string Name, string Email) : ICommand;

// Command with result
public record CreateUserCommand(string Name, string Email) : ICommand<Guid>;

// Command handler
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

#### Queries
Queries represent read operations:

```csharp
// Simple query
public record GetUserQuery(Guid Id) : IQuery<UserDto>;

// Paged query
public record GetUsersQuery : PagedQuery<UserDto>
{
    public string? SearchTerm { get; init; }
}

// Query handler
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

#### Events
Events represent something that has happened:

```csharp
// Domain event
public record UserCreatedEvent(Guid UserId, string Name, string Email) : IEvent;

// Integration event
public class UserCreatedIntegrationEvent : IntegrationEventBase
{
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}

// Event handler
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### 🔄 Mediator Pattern

The mediator coordinates communication between components:

```csharp
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.Name, request.Email);
        var userId = await _mediator.SendAsync(command);
        return Ok(new { UserId = userId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserQuery(id);
        var user = await _mediator.QueryAsync<GetUserQuery, UserDto>(query);
        return Ok(user);
    }
}
```

### 🔧 Pipeline Behaviors

Pipeline behaviors provide cross-cutting concerns:

#### Logging Behavior
Automatically logs all requests and responses:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

#### Validation Behavior
Validates requests using FluentValidation:

```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

#### Caching Behavior
Caches query results:

```csharp
public record GetUserQuery(Guid Id) : IQuery<UserDto>, ICacheableQuery
{
    public string CacheKey => $"user-{Id}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(30);
}
```

#### Transaction Behavior
Wraps commands in database transactions:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
```

### 📨 Inbox/Outbox Pattern

#### Outbox Pattern
Ensures reliable message publishing:

```csharp
public class UserService : IUserService
{
    private readonly IOutboxService _outboxService;

    public async Task CreateUserAsync(CreateUserCommand command)
    {
        // Create user in database
        var user = new User(command.Name, command.Email);
        await _userRepository.AddAsync(user);

        // Store event in outbox for reliable publishing
        var @event = new UserCreatedEvent(user.Id, user.Name, user.Email);
        await _outboxService.AddAsync(@event);
    }
}
```

#### Inbox Pattern
Ensures reliable message processing:

```csharp
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    private readonly IInboxService _inboxService;

    public async Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
    {
        // Check if message was already processed
        if (await _inboxService.IsProcessedAsync(@event.Id))
            return;

        // Process the event
        await ProcessUserCreatedEvent(@event);

        // Mark as processed
        await _inboxService.MarkAsProcessedAsync(@event.Id);
    }
}
```

### 🔒 Security Integration

```csharp
public class SecureCommandHandler : ICommandHandler<SecureCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;

    public async Task Handle(SecureCommand command, CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        
        if (!await _permissionService.HasPermissionAsync(currentUser, "CREATE_USER"))
            throw new UnauthorizedAccessException();

        // Handle command
    }
}
```

## 📦 Installation

### 1. Add Project Reference

```xml
<ProjectReference Include="..\BuildingBlocks.Application\BuildingBlocks.Application.csproj" />
```

### 2. Register Services

```csharp
using BuildingBlocks.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register all application services
builder.Services.AddApplicationLayer();

// Or register with assemblies containing handlers
builder.Services.AddMediatorWithAssemblies(
    typeof(CreateUserCommandHandler).Assembly,
    typeof(GetUserQueryHandler).Assembly
);

var app = builder.Build();
```

### 3. Configuration Options

```csharp
// Register with custom configuration
builder.Services.AddApplicationLayer(options =>
{
    options.EnableCaching = true;
    options.EnableValidation = true;
    options.EnableLogging = true;
    options.EnableRetry = true;
    options.EnableTransactions = true;
});
```

## 🔧 Advanced Configuration

### Custom Behaviors

```csharp
public class CustomBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // Pre-processing
        
        var response = await next();
        
        // Post-processing
        
        return response;
    }
}

// Register custom behavior
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CustomBehavior<,>));
```

### Custom Validation Rules

```csharp
public class UniqueEmailValidationRule : IValidationRule<string>
{
    private readonly IUserRepository _userRepository;

    public async Task<ValidationResult> ValidateAsync(string email, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.ExistsByEmailAsync(email);
        return exists 
            ? ValidationResult.Failure("Email already exists")
            : ValidationResult.Success();
    }
}
```

### Custom Cache Policies

```csharp
public class UserCachePolicy : ICachePolicy
{
    public TimeSpan Duration => TimeSpan.FromMinutes(30);
    public string KeyPrefix => "user";
    public bool SlidingExpiration => true;
}
```

## 📊 Background Services

### Outbox Processor

```csharp
public class OutboxBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _outboxProcessor.ProcessAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

### Inbox Processor

```csharp
public class InboxBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _inboxProcessor.ProcessAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

## 🧪 Testing Support

### Handler Testing

```csharp
[Test]
public async Task CreateUserCommandHandler_ShouldCreateUser()
{
    // Arrange
    var command = new CreateUserCommand("John Doe", "john@example.com");
    var handler = new CreateUserCommandHandler(_mockRepository.Object);

    // Act
    var userId = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(userId, Is.Not.EqualTo(Guid.Empty));
    _mockRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
}
```

### Behavior Testing

```csharp
[Test]
public async Task ValidationBehavior_ShouldThrowException_WhenValidationFails()
{
    // Arrange
    var invalidCommand = new CreateUserCommand("", "invalid-email");
    var behavior = new ValidationBehavior<CreateUserCommand, Guid>(_validators);

    // Act & Assert
    await Assert.ThrowsAsync<ValidationException>(
        () => behavior.Handle(invalidCommand, _mockNext.Object, CancellationToken.None)
    );
}
```

## 📈 Performance Considerations

### Caching Strategy

- **Query Caching**: Cache frequently accessed read operations
- **Sliding Expiration**: Extend cache lifetime for active data
- **Cache Invalidation**: Invalidate cache on related data changes

### Async Processing

- **Background Services**: Process outbox/inbox messages asynchronously
- **Event Publishing**: Use fire-and-forget for non-critical events
- **Bulk Operations**: Process multiple messages in batches

### Memory Management

- **Scoped Services**: Use appropriate service lifetimes
- **Disposable Resources**: Properly dispose of resources
- **Large Objects**: Consider streaming for large data sets

## 🔍 Monitoring and Diagnostics

### Logging Integration

```csharp
services.AddLogging(builder =>
{
    builder.AddSerilog();
    builder.AddApplicationInsights();
});
```

### Performance Monitoring

```csharp
services.Configure<PerformanceBehaviorOptions>(options =>
{
    options.WarningThreshold = TimeSpan.FromMilliseconds(500);
    options.ErrorThreshold = TimeSpan.FromSeconds(5);
});
```

### Health Checks

```csharp
services.AddHealthChecks()
    .AddCheck<OutboxHealthCheck>("outbox")
    .AddCheck<InboxHealthCheck>("inbox")
    .AddCheck<CacheHealthCheck>("cache");
```

## 📝 Best Practices

### Command Design

1. **Immutable**: Use records for immutable commands
2. **Validation**: Include all necessary validation attributes
3. **Single Responsibility**: One command per business operation
4. **Naming**: Use verb-noun naming convention

### Query Design

1. **Read-Only**: Never modify state in queries
2. **Projection**: Return only necessary data
3. **Pagination**: Support paging for large datasets
4. **Caching**: Cache frequently accessed queries

### Event Design

1. **Past Tense**: Name events in past tense
2. **Immutable**: Events should be immutable once created
3. **Versioning**: Support event schema evolution
4. **Idempotent**: Event handlers should be idempotent

### Handler Guidelines

1. **Single Purpose**: One handler per command/query/event
2. **Async**: Use async/await for I/O operations
3. **Error Handling**: Handle exceptions appropriately
4. **Testing**: Write comprehensive unit tests

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Add comprehensive unit tests
3. Update documentation
4. Follow semantic versioning
5. Ensure all builds pass

## 📄 License

This project is licensed under the MIT License.