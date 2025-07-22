# BuildingBlocks.Application

A comprehensive application layer building blocks library for .NET applications following Clean Architecture principles with CQRS, Event Sourcing, and Domain-Driven Design patterns.

## 🏗️ Architecture Overview

This library provides the essential application layer components for building robust, scalable applications using:

- **CQRS Pattern**: Command Query Responsibility Segregation
- **Event-Driven Architecture**: Domain and integration events
- **Pipeline Behaviors**: Cross-cutting concerns (logging, validation, caching, etc.)
- **Inbox/Outbox Pattern**: Reliable message processing
- **Clean Architecture**: Separation of concerns and dependency inversion

## 📁 Project Structure

```
BuildingBlocks.Application/
├── 📁 CQRS/                          # Command Query Responsibility Segregation
│   ├── 📁 Commands/                  # Command handling components
│   │   ├── ICommand.cs               # Command marker interface
│   │   ├── ICommandHandler.cs        # Command handler interfaces
│   │   └── CommandBase.cs            # Base command implementation
│   ├── 📁 Queries/                   # Query handling components
│   │   ├── IQuery.cs                 # Query interfaces
│   │   ├── IQueryHandler.cs          # Query handler interfaces
│   │   ├── QueryBase.cs              # Base query implementation
│   │   ├── PagedQuery.cs             # Pagination support
│   │   ├── PagedResult.cs            # Paged results
│   │   └── SortingQuery.cs           # Sorting support
│   ├── 📁 Events/                    # Event handling components
│   │   ├── IEvent.cs                 # Event marker interface
│   │   ├── IEventHandler.cs          # Event handler interfaces
│   │   ├── IIntegrationEvent.cs      # Integration event interface
│   │   ├── IntegrationEventBase.cs   # Base integration event
│   │   └── DomainEventNotification.cs # Domain event notifications
│   ├── 📁 Messages/                  # Message handling components
│   │   ├── IMessage.cs               # Message interfaces
│   │   ├── IStreamMessage.cs         # Stream message interface
│   │   ├── MessageBase.cs            # Base message implementation
│   │   └── IMessageContext.cs        # Message context interface
│   └── 📁 Mediator/                  # Mediator pattern implementation
│       ├── IMediator.cs              # Mediator interface
│       └── Mediator.cs               # Mediator implementation
├── 📁 Behaviors/                     # Pipeline behaviors for cross-cutting concerns
│   ├── IPipelineBehavior.cs          # Pipeline behavior interface
│   ├── LoggingBehavior.cs            # Request/response logging
│   ├── ValidationBehavior.cs         # Request validation
│   ├── CachingBehavior.cs            # Response caching
│   ├── TransactionBehavior.cs        # Database transactions
│   ├── PerformanceBehavior.cs        # Performance monitoring
│   └── RetryBehavior.cs              # Retry policies
├── 📁 Services/                      # Application services
│   ├── IApplicationService.cs        # Application service interface
│   ├── ApplicationServiceBase.cs     # Base application service
│   ├── IDomainEventService.cs        # Domain event service interface
│   ├── DomainEventService.cs         # Domain event service implementation
│   ├── OutboxBackgroundService.cs   # Outbox message processing
│   └── InboxBackgroundService.cs    # Inbox message processing
├── 📁 Validation/                    # Validation components
│   ├── IValidator.cs                 # Validator interface
│   ├── IValidationRule.cs            # Validation rule interface
│   ├── ValidationResult.cs           # Validation result
│   ├── ValidationError.cs            # Validation error
│   ├── CompositeValidator.cs         # Composite validation
│   └── ValidatorBase.cs              # Base validator
├── 📁 Caching/                       # Caching abstractions
│   ├── ICacheService.cs              # Cache service interface
│   ├── ICacheKey.cs                  # Cache key interface
│   ├── CacheKey.cs                   # Cache key implementation
│   ├── CacheSettings.cs              # Cache configuration
│   └── CachePolicy.cs                # Cache policies
├── 📁 Messaging/                     # Message bus and event bus
│   ├── IMessageBus.cs                # Message bus interface
│   ├── IEventBus.cs                  # Event bus interface
│   ├── IMessageHandler.cs            # Message handler interface
│   ├── IMessagePublisher.cs          # Message publisher interface
│   └── MessageMetadata.cs            # Message metadata
├── 📁 DTOs/                          # Data Transfer Objects
│   ├── BaseDto.cs                    # Base DTO
│   ├── AuditableDto.cs               # Auditable DTO
│   └── PagedDto.cs                   # Paged DTO
├── 📁 Mapping/                       # Object mapping abstractions
│   ├── IMapper.cs                    # Mapper interface
│   ├── IMappingProfile.cs            # Mapping profile interface
│   └── MapperBase.cs                 # Base mapper
├── 📁 Security/                      # Security and authorization
│   ├── ICurrentUserService.cs        # Current user service
│   ├── IPermissionService.cs         # Permission service
│   ├── UserContext.cs                # User context
│   └── SecurityContext.cs            # Security context
├── 📁 Inbox/                         # Inbox pattern implementation
│   ├── IInboxService.cs              # Inbox service interface
│   ├── InboxMessage.cs               # Inbox message entity
│   ├── InboxMessageStatus.cs         # Message status enum
│   ├── IInboxProcessor.cs            # Inbox processor interface
│   └── InboxProcessor.cs             # Inbox processor implementation
├── 📁 Outbox/                        # Outbox pattern implementation
│   ├── IOutboxService.cs             # Outbox service interface
│   ├── OutboxMessage.cs              # Outbox message entity
│   ├── OutboxMessageStatus.cs        # Message status enum
│   ├── IOutboxProcessor.cs           # Outbox processor interface
│   └── OutboxProcessor.cs            # Outbox processor implementation
├── 📁 Dispatchers/                   # Message dispatchers
│   ├── ICommandDispatcher.cs         # Command dispatcher interface
│   ├── CommandDispatcher.cs          # Command dispatcher implementation
│   ├── IQueryDispatcher.cs           # Query dispatcher interface
│   ├── QueryDispatcher.cs            # Query dispatcher implementation
│   ├── IEventDispatcher.cs           # Event dispatcher interface
│   ├── EventDispatcher.cs            # Event dispatcher implementation
│   ├── IMessageDispatcher.cs         # Message dispatcher interface
│   └── MessageDispatcher.cs          # Message dispatcher implementation
├── 📁 Sagas/                         # Saga pattern implementation
│   ├── ISaga.cs                      # Saga interface
│   ├── SagaBase.cs                   # Base saga implementation
│   └── ISagaManager.cs               # Saga manager interface
└── 📁 Extensions/                    # Dependency injection extensions
    ├── ServiceCollectionExtensions.cs # Service collection extensions
    ├── ApplicationExtensions.cs       # Application extensions
    └── MediatorExtensions.cs          # Mediator registration extensions
```

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