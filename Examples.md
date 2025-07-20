# BuildingBlocks Examples

This document contains comprehensive examples of how to use the BuildingBlocks architecture components.

## Table of Contents

1. [Saga Pattern Examples](#saga-pattern-examples)
2. [CQRS Examples](#cqrs-examples)
3. [Pipeline Behavior Examples](#pipeline-behavior-examples)
4. [Validation Examples](#validation-examples)
5. [Domain-Driven Design Examples](#domain-driven-design-examples)
6. [Inbox/Outbox Pattern Examples](#inboxoutbox-pattern-examples)

---

## Saga Pattern Examples

### 1. Order Processing Saga

This example demonstrates a complete order processing workflow with automatic compensation.

#### Saga Data Model

```csharp
using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.Sagas;

namespace YourApplication.Sagas;

// Example Saga Data
public class OrderProcessingData
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentId { get; set; } = string.Empty;
    public string ShippingId { get; set; } = string.Empty;
    public bool PaymentProcessed { get; set; }
    public bool InventoryReserved { get; set; }
    public bool ShippingArranged { get; set; }
}
```

#### Saga Implementation

```csharp
// Example Saga Implementation
public class OrderProcessingSaga : SagaBase<OrderProcessingData>
{
    private readonly IPaymentService _paymentService;
    private readonly IInventoryService _inventoryService;
    private readonly IShippingService _shippingService;

    public OrderProcessingSaga(
        OrderProcessingData data,
        ILogger<OrderProcessingSaga> logger,
        IPaymentService paymentService,
        IInventoryService inventoryService,
        IShippingService shippingService) : base(data, logger)
    {
        _paymentService = paymentService;
        _inventoryService = inventoryService;
        _shippingService = shippingService;
    }

    public override string Name => "OrderProcessing";

    protected override async Task ConfigureSteps()
    {
        // Step 1: Process Payment
        AddStep(
            name: "ProcessPayment",
            action: async (ct) =>
            {
                Data.PaymentId = await _paymentService.ProcessPaymentAsync(Data.OrderId, Data.Amount, ct);
                Data.PaymentProcessed = true;
            },
            compensation: async (ct) =>
            {
                if (Data.PaymentProcessed)
                {
                    await _paymentService.RefundPaymentAsync(Data.PaymentId, ct);
                    Data.PaymentProcessed = false;
                }
            }
        );

        // Step 2: Reserve Inventory
        AddStep(
            name: "ReserveInventory",
            action: async (ct) =>
            {
                await _inventoryService.ReserveItemsAsync(Data.OrderId, ct);
                Data.InventoryReserved = true;
            },
            compensation: async (ct) =>
            {
                if (Data.InventoryReserved)
                {
                    await _inventoryService.ReleaseReservationAsync(Data.OrderId, ct);
                    Data.InventoryReserved = false;
                }
            }
        );

        // Step 3: Arrange Shipping
        AddStep(
            name: "ArrangeShipping",
            action: async (ct) =>
            {
                Data.ShippingId = await _shippingService.ScheduleShippingAsync(Data.OrderId, ct);
                Data.ShippingArranged = true;
            },
            compensation: async (ct) =>
            {
                if (Data.ShippingArranged)
                {
                    await _shippingService.CancelShippingAsync(Data.ShippingId, ct);
                    Data.ShippingArranged = false;
                }
            }
        );
    }

    protected override async Task OnCompletedAsync(CancellationToken cancellationToken)
    {
        // Send order confirmation
        // Update order status
        // Notify customer
        await Task.CompletedTask;
    }

    protected override async Task OnCompensatedAsync(CancellationToken cancellationToken)
    {
        // Mark order as failed
        // Notify customer of failure
        // Log incident
        await Task.CompletedTask;
    }
}
```

#### Service Interfaces

```csharp
// Example service interfaces
public interface IPaymentService
{
    Task<string> ProcessPaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken);
    Task RefundPaymentAsync(string paymentId, CancellationToken cancellationToken);
}

public interface IInventoryService
{
    Task ReserveItemsAsync(Guid orderId, CancellationToken cancellationToken);
    Task ReleaseReservationAsync(Guid orderId, CancellationToken cancellationToken);
}

public interface IShippingService
{
    Task<string> ScheduleShippingAsync(Guid orderId, CancellationToken cancellationToken);
    Task CancelShippingAsync(string shippingId, CancellationToken cancellationToken);
}
```

#### Usage Example

```csharp
// Registering the saga
services.AddSaga<OrderProcessingSaga, OrderProcessingData>();

// Starting the saga
var sagaData = new OrderProcessingData 
{ 
    OrderId = orderId, 
    CustomerId = customerId, 
    Amount = 100.00m 
};

var saga = await sagaOrchestrator.StartSagaAsync<OrderProcessingSaga, OrderProcessingData>(
    sagaData, cancellationToken);
```

### 2. User Registration Saga

```csharp
public class UserRegistrationData
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool AccountCreated { get; set; }
    public bool EmailSent { get; set; }
    public bool ProfileCreated { get; set; }
}

public class UserRegistrationSaga : SagaBase<UserRegistrationData>
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly IProfileService _profileService;

    public UserRegistrationSaga(
        UserRegistrationData data,
        ILogger<UserRegistrationSaga> logger,
        IUserService userService,
        IEmailService emailService,
        IProfileService profileService) : base(data, logger)
    {
        _userService = userService;
        _emailService = emailService;
        _profileService = profileService;
    }

    public override string Name => "UserRegistration";

    protected override async Task ConfigureSteps()
    {
        AddStep("CreateAccount",
            action: async ct =>
            {
                Data.UserId = await _userService.CreateUserAsync(Data.Email, Data.Password, ct);
                Data.AccountCreated = true;
            },
            compensation: async ct =>
            {
                if (Data.AccountCreated)
                {
                    await _userService.DeleteUserAsync(Data.UserId, ct);
                    Data.AccountCreated = false;
                }
            });

        AddStep("SendWelcomeEmail",
            action: async ct =>
            {
                await _emailService.SendWelcomeEmailAsync(Data.Email, ct);
                Data.EmailSent = true;
            });

        AddStep("CreateProfile",
            action: async ct =>
            {
                await _profileService.CreateProfileAsync(Data.UserId, ct);
                Data.ProfileCreated = true;
            },
            compensation: async ct =>
            {
                if (Data.ProfileCreated)
                {
                    await _profileService.DeleteProfileAsync(Data.UserId, ct);
                    Data.ProfileCreated = false;
                }
            });
    }
}
```

---

## CQRS Examples

### 1. Command Examples

#### Simple Command (No Result)

```csharp
using BuildingBlocks.Application.CQRS.Commands;

public class DeleteUserCommand : CommandBase
{
    public Guid UserId { get; set; }
}

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        await _userRepository.DeleteByIdAsync(command.UserId, cancellationToken);
    }
}
```

#### Command with Result

```csharp
public class CreateUserCommand : CommandBase<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CreateUserCommandHandler : ICommandHandlerWithResult<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = new User(command.Name, command.Email);
        await _userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}
```

### 2. Query Examples

#### Simple Query

```csharp
using BuildingBlocks.Application.CQRS.Queries;

public class GetUserQuery : QueryBase<UserDto>
{
    public Guid UserId { get; set; }
}

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> HandleAsync(GetUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        return _mapper.Map<UserDto>(user);
    }
}
```

#### Paged Query

```csharp
public class GetUsersQuery : PagedQuery<UserDto>
{
    public string? SearchTerm { get; set; }
    public UserStatus? Status { get; set; }
}

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var specification = new UserSpecification(query.SearchTerm, query.Status);
        var users = await _userRepository.FindAsync(specification, cancellationToken);
        var totalCount = await _userRepository.CountAsync(specification, cancellationToken);
        
        var userDtos = _mapper.Map<List<UserDto>>(users);
        return new PagedResult<UserDto>(userDtos, totalCount, query.Page, query.PageSize);
    }
}
```

---

## Pipeline Behavior Examples

### 1. Custom Validation Behavior

```csharp
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Validation;

public class CustomValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IValidator<TRequest> _validator;

    public CustomValidationBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return await next();
    }
}
```

### 2. Caching Behavior

```csharp
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ICacheableQuery<TResponse>
    where TResponse : class
{
    private readonly ICacheService _cacheService;

    public CachingBehavior(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        var cacheKey = request.GetCacheKey();
        var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        
        if (cachedResponse != null)
        {
            return cachedResponse;
        }

        var response = await next();
        await _cacheService.SetAsync(cacheKey, response, request.GetCachePolicy(), cancellationToken);
        
        return response;
    }
}

public interface ICacheableQuery<T>
{
    string GetCacheKey();
    CachePolicy GetCachePolicy();
}
```

---

## Validation Examples

### 1. Custom Validator

```csharp
using BuildingBlocks.Application.Validation;

public class CreateUserCommandValidator : ValidatorBase<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        AddRule(new EmailValidationRule());
        AddRule(new NameValidationRule());
    }
}

public class EmailValidationRule : IValidationRule<CreateUserCommand>
{
    public bool CanValidate(CreateUserCommand instance) => true;

    public ValidationResult Validate(CreateUserCommand instance)
    {
        var result = new ValidationResult();
        
        if (string.IsNullOrWhiteSpace(instance.Email))
        {
            result.AddError(nameof(instance.Email), "Email is required");
        }
        else if (!IsValidEmail(instance.Email))
        {
            result.AddError(nameof(instance.Email), "Email format is invalid");
        }

        return result;
    }

    private bool IsValidEmail(string email)
    {
        // Email validation logic
        return email.Contains("@") && email.Contains(".");
    }
}
```

---

## Domain-Driven Design Examples

### 1. Entity with Strongly Typed ID

```csharp
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;

public class UserId : GuidId
{
    public UserId(Guid value) : base(value) { }
    public UserId() : base() { }
}

public class User : AggregateRoot<UserId>
{
    private User() { } // For EF Core

    public User(string name, string email) : base(new UserId())
    {
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        
        // Raise domain event
        AddDomainEvent(new UserCreatedEvent(Id, name, email));
    }

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public void UpdateEmail(string newEmail)
    {
        if (Email != newEmail)
        {
            var oldEmail = Email;
            Email = newEmail;
            AddDomainEvent(new UserEmailChangedEvent(Id, oldEmail, newEmail));
        }
    }
}
```

### 2. Value Object Example

```csharp
using BuildingBlocks.Domain.ValueObjects;

public class Address : ValueObject
{
    public Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}
```

### 3. Domain Event Example

```csharp
using BuildingBlocks.Domain.DomainEvents;

public class UserCreatedEvent : DomainEventBase
{
    public UserCreatedEvent(UserId userId, string name, string email)
    {
        UserId = userId;
        Name = name;
        Email = email;
    }

    public UserId UserId { get; }
    public string Name { get; }
    public string Email { get; }
}

public class UserCreatedEventHandler : IDomainEventHandler<UserCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(IEmailService emailService, ILogger<UserCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(UserCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User created: {UserId}", domainEvent.UserId);
        
        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(domainEvent.Email, cancellationToken);
    }
}
```

---

## Service Registration Examples

### 1. Complete Application Setup

```csharp
// In Program.cs or Startup.cs
services.AddApplicationLayer()
    .AddValidation()
    .AddLogging()
    .AddPerformanceMonitoring()
    .AddTransactionSupport()
    .AddSagas();

// Register specific components
services.AddSaga<OrderProcessingSaga, OrderProcessingData>();
services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();

// Register domain services
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IPaymentService, PaymentService>();
services.AddScoped<IEmailService, EmailService>();
```

### 2. Infrastructure Layer Setup

```csharp
// Infrastructure layer registration
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

// Caching
services.AddMemoryCache();
services.AddScoped<ICacheService, MemoryCacheService>();

// Messaging
services.AddScoped<IMessageBus, MessageBus>();
services.AddScoped<IEventBus, EventBus>();
```

---

## Inbox/Outbox Pattern Examples

### 1. Inbox Pattern - Message Deduplication

The Inbox pattern ensures that incoming messages are processed exactly once, preventing duplicate processing in distributed systems.

#### Basic Inbox Implementation

```csharp
using BuildingBlocks.Application.Inbox;
using Microsoft.Extensions.DependencyInjection;

// 1. Register Inbox services
services.AddInboxSupport();

// 2. Implement message handlers
public class OrderCreatedMessageHandler : IInboxMessageHandler<OrderCreatedMessage>
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderCreatedMessageHandler> _logger;

    public OrderCreatedMessageHandler(IOrderService orderService, ILogger<OrderCreatedMessageHandler> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing order created message for Order {OrderId}", message.OrderId);
        
        await _orderService.ProcessNewOrderAsync(message.OrderId, message.CustomerId, cancellationToken);
    }

    public bool CanHandle(string messageType) => messageType == nameof(OrderCreatedMessage);
}

// 3. Message model
public class OrderCreatedMessage
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
}
```

#### Processing Inbox Messages

```csharp
// Manual processing
public class OrderService
{
    private readonly IInboxService _inboxService;
    private readonly IInboxProcessor _inboxProcessor;

    public OrderService(IInboxService inboxService, IInboxProcessor inboxProcessor)
    {
        _inboxService = inboxService;
        _inboxProcessor = inboxProcessor;
    }

    public async Task ProcessIncomingOrderMessage(string messageJson, string messageType, string correlationId)
    {
        // Store message in inbox (idempotent operation)
        await _inboxService.AddMessageAsync(
            messageType: messageType,
            payload: messageJson,
            correlationId: correlationId);

        // Process immediately or let background service handle it
        await _inboxProcessor.ProcessPendingMessagesAsync();
    }
}
```

#### Background Service Processing

The background service automatically processes inbox messages:

```csharp
// Automatic background processing (already configured when using AddInboxSupport())
// The InboxBackgroundService will:
// 1. Process pending messages every 30 seconds (configurable)
// 2. Retry failed messages up to 3 times (configurable)
// 3. Clean up old processed messages after 30 days (configurable)

// Configure options
services.Configure<InboxOptions>(options =>
{
    options.BatchSize = 50;
    options.MaxRetries = 5;
    options.RetentionPeriod = TimeSpan.FromDays(7);
    options.ProcessingInterval = TimeSpan.FromSeconds(10);
});
```

### 2. Outbox Pattern - Transactional Message Publishing

The Outbox pattern ensures that messages are published reliably as part of a database transaction.

#### Basic Outbox Implementation

```csharp
using BuildingBlocks.Application.Outbox;

// 1. Register Outbox services
services.AddOutboxSupport();

// 2. Implement message publishers
public class ServiceBusMessagePublisher : IOutboxMessagePublisher
{
    private readonly IServiceBusClient _serviceBusClient;
    private readonly ILogger<ServiceBusMessagePublisher> _logger;

    public ServiceBusMessagePublisher(IServiceBusClient serviceBusClient, ILogger<ServiceBusMessagePublisher> logger)
    {
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    public async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing message {MessageId} to Service Bus", message.Id);
        
        await _serviceBusClient.SendMessageAsync(
            destination: message.Destination,
            messageBody: message.Payload,
            correlationId: message.CorrelationId,
            cancellationToken: cancellationToken);
    }

    public bool CanPublish(string destination) => destination.StartsWith("servicebus:");
}

public class RabbitMQMessagePublisher : IOutboxMessagePublisher
{
    private readonly IRabbitMQClient _rabbitMQClient;

    public RabbitMQMessagePublisher(IRabbitMQClient rabbitMQClient)
    {
        _rabbitMQClient = rabbitMQClient;
    }

    public async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _rabbitMQClient.PublishAsync(
            exchange: message.Destination,
            messageBody: message.Payload,
            correlationId: message.CorrelationId,
            cancellationToken: cancellationToken);
    }

    public bool CanPublish(string destination) => destination.StartsWith("rabbitmq:");
}
```

#### Using Outbox in Domain Services

```csharp
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOutboxService _outboxService;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IOrderRepository orderRepository, IOutboxService outboxService, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _outboxService = outboxService;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        // Start transaction
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. Create the order (business logic)
            var order = new Order(request.CustomerId, request.Items);
            await _orderRepository.AddAsync(order, cancellationToken);

            // 2. Store outbox message (same transaction)
            var orderCreatedMessage = new OrderCreatedMessage
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount
            };

            await _outboxService.AddMessageAsync(
                messageType: nameof(OrderCreatedMessage),
                payload: JsonSerializer.Serialize(orderCreatedMessage),
                destination: "servicebus:order-events",
                correlationId: order.Id.ToString(),
                cancellationToken: cancellationToken);

            // 3. Commit transaction (both order and outbox message saved atomically)
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
```

#### Scheduled Message Publishing

```csharp
public class OrderService
{
    public async Task ScheduleOrderReminderAsync(Guid orderId, DateTime reminderTime)
    {
        var reminderMessage = new OrderReminderMessage { OrderId = orderId };
        
        // Schedule message for future delivery
        await _outboxService.ScheduleMessageAsync(
            messageType: nameof(OrderReminderMessage),
            payload: JsonSerializer.Serialize(reminderMessage),
            destination: "servicebus:order-reminders",
            scheduledAt: reminderTime,
            correlationId: orderId.ToString());
    }
}
```

### 3. Combined Inbox/Outbox Setup

```csharp
// Complete setup with both patterns
services.AddInboxOutboxSupport();

// Register message handlers
services.AddScoped<IInboxMessageHandler<OrderCreatedMessage>, OrderCreatedMessageHandler>();
services.AddScoped<IInboxMessageHandler<PaymentProcessedMessage>, PaymentProcessedMessageHandler>();

// Register message publishers  
services.AddScoped<IOutboxMessagePublisher, ServiceBusMessagePublisher>();
services.AddScoped<IOutboxMessagePublisher, RabbitMQMessagePublisher>();

// Configure options
services.Configure<InboxOptions>(options =>
{
    options.ProcessingInterval = TimeSpan.FromSeconds(10);
    options.MaxRetries = 5;
});

services.Configure<OutboxOptions>(options =>
{
    options.ProcessingInterval = TimeSpan.FromSeconds(15);
    options.MaxRetries = 3;
});
```

### 4. Advanced Usage - Custom Message Processor

```csharp
public class CustomOrderProcessor
{
    private readonly IInboxProcessor _inboxProcessor;
    private readonly IOutboxProcessor _outboxProcessor;

    public CustomOrderProcessor(IInboxProcessor inboxProcessor, IOutboxProcessor outboxProcessor)
    {
        _inboxProcessor = inboxProcessor;
        _outboxProcessor = outboxProcessor;
    }

    public async Task ProcessOrderWorkflowAsync(CancellationToken cancellationToken = default)
    {
        // Process incoming order messages
        await _inboxProcessor.ProcessPendingMessagesAsync(cancellationToken);
        
        // Publish outgoing order events
        await _outboxProcessor.ProcessPendingMessagesAsync(cancellationToken);
        
        // Retry failed operations
        await _inboxProcessor.RetryFailedMessagesAsync(cancellationToken);
        await _outboxProcessor.RetryFailedMessagesAsync(cancellationToken);
        
        // Clean up old messages
        await _inboxProcessor.CleanupExpiredMessagesAsync(cancellationToken);
        await _outboxProcessor.CleanupExpiredMessagesAsync(cancellationToken);
    }
}
```

### 5. Monitoring and Observability

```csharp
public class MessageProcessingMetrics
{
    private readonly IInboxService _inboxService;
    private readonly IOutboxService _outboxService;

    public async Task<InboxMetrics> GetInboxMetricsAsync()
    {
        var pendingMessages = await _inboxService.GetPendingMessagesAsync(int.MaxValue);
        var failedMessages = await _inboxService.GetFailedMessagesAsync(int.MaxValue);
        
        return new InboxMetrics
        {
            PendingCount = pendingMessages.Count(),
            FailedCount = failedMessages.Count(),
            LastProcessedAt = DateTime.UtcNow
        };
    }

    public async Task<OutboxMetrics> GetOutboxMetricsAsync()
    {
        var pendingMessages = await _outboxService.GetPendingMessagesAsync(int.MaxValue);
        var failedMessages = await _outboxService.GetFailedMessagesAsync(int.MaxValue);
        
        return new OutboxMetrics
        {
            PendingCount = pendingMessages.Count(),
            FailedCount = failedMessages.Count(),
            LastPublishedAt = DateTime.UtcNow
        };
    }
}
```

The Inbox/Outbox patterns provide robust message handling capabilities ensuring reliability, exactly-once processing, and transactional consistency in distributed systems.

---

This comprehensive examples file shows how to implement and use all the major components of the BuildingBlocks architecture in real-world scenarios.