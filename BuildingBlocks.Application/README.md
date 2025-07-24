# BuildingBlocks.Application

A comprehensive application layer library providing essential building blocks for implementing Clean Architecture and CQRS patterns in .NET applications.

## Features

### 🏗️ CQRS & Mediator Pattern
- **Commands**: Command handlers with response support and base classes
- **Queries**: Query handlers with pagination, sorting, and filtering support
- **Events**: Domain and integration event handling with notifications
- **Messages**: Generic message handling with streaming support
- **Mediator**: Central request routing and pipeline orchestration

### 🔄 Pipeline Behaviors (Cross-Cutting Concerns)
- **Logging**: Automatic request/response logging with performance metrics
- **Validation**: Comprehensive request validation with error aggregation
- **Performance Monitoring**: Execution time tracking with configurable thresholds
- **Transaction Management**: Automatic transaction handling with rollback support
- **Caching**: Intelligent request/response caching with policies
- **Retry**: Configurable automatic retry mechanisms with backoff strategies

### 📨 Messaging & Event Handling
- **Inbox/Outbox Pattern**: Reliable message processing with exactly-once delivery
- **Event Bus**: Domain and integration event publishing and handling
- **Message Bus**: Generic message handling with metadata support
- **Background Services**: Automated message processing with error handling
- **Dispatchers**: Dedicated dispatchers for commands, queries, events, and messages

### 🔐 Security & Context Management
- **User Context**: Current user information with roles and permissions
- **Security Context**: Multi-tenant and organization-aware security
- **Permission Service**: Role-based and attribute-based authorization
- **Current User Service**: Abstraction for accessing current user information

### ⚡ Advanced Caching
- **Memory Cache**: High-performance in-memory caching
- **Distributed Cache**: Redis and SQL Server caching support
- **Cache Policies**: TTL, sliding expiration, and dependency-based invalidation
- **Cache Keys**: Structured, hierarchical cache key management
- **Cache Settings**: Flexible configuration with environment-specific overrides

### 📝 Validation & Mapping
- **Validation Framework**: Rule-based validation with composite validators
- **Mapping Support**: Object mapping abstractions with profile support
- **DTOs**: Base classes for data transfer objects with audit support
- **Validation Rules**: Reusable validation components

### 🔄 Saga Pattern & Orchestration
- **Saga Orchestration**: Long-running process management with compensation
- **Saga Steps**: Individual process steps with rollback capabilities  
- **State Management**: Persistent saga state with recovery support
- **Saga Repository**: Storage abstraction for saga persistence

### 🏗️ Application Services
- **Application Service Base**: Foundation for application services
- **Domain Event Service**: Domain event publishing coordination
- **Background Services**: Hosted services for continuous processing
- **Service Abstractions**: Clean interfaces for application logic

## Installation

Add the project reference to your application layer:

```xml
<ProjectReference Include="..\BuildingBlocks.Application\BuildingBlocks.Application.csproj" />
```

## Complete Usage Guide

### 1. Service Registration

Register all application services in `Program.cs`:

```csharp
using BuildingBlocks.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add all application layer services with default configuration
builder.Services.AddApplicationLayer();

// Or with custom configuration
builder.Services.AddApplicationLayer(options =>
{
    options.EnableCaching = true;
    options.EnableValidation = true;
    options.EnablePerformanceLogging = true;
    options.DefaultTimeout = TimeSpan.FromSeconds(30);
});

// Add pipeline behaviors in specific order
builder.Services.AddPipelineBehaviors(behaviors =>
{
    behaviors.Add<LoggingBehavior<,>>();
    behaviors.Add<ValidationBehavior<,>>();
    behaviors.Add<CachingBehavior<,>>();
    behaviors.Add<PerformanceBehavior<,>>();
    behaviors.Add<RetryBehavior<,>>();
    behaviors.Add<TransactionBehavior<,>>();
});

var app = builder.Build();
```

### 2. CQRS Implementation

#### Commands with Responses
```csharp
// Command with result
public record CreateOrderCommand(CustomerId CustomerId, List<CreateOrderItemRequest> Items) 
    : CommandBase<OrderId>
{
    public override string OperationName => "CreateOrder";
}

// Command handler
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderId>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
    }

    public async Task<OrderId> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(OrderId.New(), request.CustomerId);
        
        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.Quantity, item.UnitPrice);
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Domain events are automatically published by TransactionBehavior
        await _domainEventService.PublishEventsAsync(order.DomainEvents, cancellationToken);
        
        return order.Id;
    }
}

// Command without result (void commands)
public record DeleteOrderCommand(OrderId OrderId) : ICommand
{
    public string OperationName => "DeleteOrder";
}

// Usage in controllers
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<OrderId>> CreateOrder(CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(OrderId id)
    {
        await _mediator.Send(new DeleteOrderCommand(id));
        return NoContent();
    }
}
```

#### Queries with Pagination and Sorting
```csharp
// Paged query with sorting
public record GetOrdersQuery : PagedQuery, IQuery<PagedResult<OrderDto>>
{
    public string? CustomerName { get; init; }
    public OrderStatus? Status { get; init; }
    public DateRange? DateRange { get; init; }
    
    public GetOrdersQuery()
    {
        // Default sorting
        SortBy = nameof(OrderDto.CreatedAt);
        SortDirection = SortDirection.Descending;
    }
}

// Query handler with specifications
public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var specification = new OrdersWithFilterSpec(request.CustomerName, request.Status, request.DateRange);
        
        var orders = await _orderRepository.GetPagedAsync(
            specification, 
            request.Page, 
            request.PageSize, 
            request.SortBy, 
            request.SortDirection, 
            cancellationToken);
            
        var orderDtos = await _mapper.MapAsync<List<OrderDto>>(orders.Items, cancellationToken);
        
        return new PagedResult<OrderDto>
        {
            Items = orderDtos,
            TotalCount = orders.TotalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = orders.TotalPages
        };
    }
}

// Single item query
public record GetOrderQuery(OrderId OrderId) : QueryBase<OrderDetailDto>
{
    public override string CacheKey => $"Order_{OrderId}";
    public override TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}
```

#### Domain Events and Integration Events
```csharp
// Domain event
public record OrderCreatedEvent(OrderId OrderId, CustomerId CustomerId, Money TotalAmount) 
    : IEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}

// Domain event handler
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly ICustomerRepository _customerRepository;

    public OrderCreatedEventHandler(
        ILogger<OrderCreatedEventHandler> logger,
        IEmailService emailService,
        ICustomerRepository customerRepository)
    {
        _logger = logger;
        _emailService = emailService;
        _customerRepository = customerRepository;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing order created event for order {OrderId}", notification.OrderId);
        
        var customer = await _customerRepository.GetByIdAsync(notification.CustomerId, cancellationToken);
        if (customer != null)
        {
            await _emailService.SendOrderConfirmationAsync(customer.Email, notification.OrderId, cancellationToken);
        }
    }
}

// Integration event for external systems
public record OrderCreatedIntegrationEvent : IntegrationEventBase
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    public decimal TotalAmount { get; }
    public string Currency { get; }

    public OrderCreatedIntegrationEvent(OrderId orderId, CustomerId customerId, Money totalAmount)
        : base($"order.created.{orderId}", "OrderService")
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount.Amount;
        Currency = totalAmount.Currency;
    }
}
```

### 3. Pipeline Behaviors Configuration

#### Custom Pipeline Behavior
```csharp
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;

    public AuditBehavior(
        ICurrentUserService currentUserService,
        IAuditService auditService,
        ILogger<AuditBehavior<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("User {UserId} executing {RequestName}", userId, requestName);
        
        var auditEntry = new AuditEntry
        {
            UserId = userId,
            Action = requestName,
            RequestData = JsonSerializer.Serialize(request),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            var response = await next();
            
            auditEntry.Success = true;
            auditEntry.ResponseData = JsonSerializer.Serialize(response);
            
            return response;
        }
        catch (Exception ex)
        {
            auditEntry.Success = false;
            auditEntry.Error = ex.Message;
            throw;
        }
        finally
        {
            await _auditService.SaveAuditEntryAsync(auditEntry, cancellationToken);
        }
    }
}
```

#### Configuring Behaviors
```csharp
// In service registration
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
```

### 4. Validation Framework

#### Request Validators
```csharp
public class CreateOrderCommandValidator : ValidatorBase<CreateOrderCommand>
{
    private readonly ICustomerRepository _customerRepository;

    public CreateOrderCommandValidator(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public override async Task<ValidationResult> ValidateAsync(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var result = new ValidationResult();

        // Basic validation
        if (request.CustomerId == null)
        {
            result.Errors.Add(new ValidationError(nameof(request.CustomerId), "Customer ID is required"));
        }

        if (request.Items == null || !request.Items.Any())
        {
            result.Errors.Add(new ValidationError(nameof(request.Items), "At least one order item is required"));
        }

        // Async validation
        if (request.CustomerId != null)
        {
            var customerExists = await _customerRepository.ExistsAsync(request.CustomerId, cancellationToken);
            if (!customerExists)
            {
                result.Errors.Add(new ValidationError(nameof(request.CustomerId), "Customer does not exist"));
            }
        }

        // Validate each item
        if (request.Items != null)
        {
            for (int i = 0; i < request.Items.Count; i++)
            {
                var item = request.Items[i];
                if (item.Quantity <= 0)
                {
                    result.Errors.Add(new ValidationError($"{nameof(request.Items)}[{i}].{nameof(item.Quantity)}", "Quantity must be greater than zero"));
                }
                if (item.UnitPrice.Amount <= 0)
                {
                    result.Errors.Add(new ValidationError($"{nameof(request.Items)}[{i}].{nameof(item.UnitPrice)}", "Unit price must be greater than zero"));
                }
            }
        }

        return result;
    }
}

// Composite validator for complex scenarios
public class OrderValidationRules : CompositeValidator<CreateOrderCommand>
{
    public OrderValidationRules(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IInventoryService inventoryService)
    {
        AddValidator(new CreateOrderCommandValidator(customerRepository));
        AddValidator(new ProductAvailabilityValidator(productRepository));
        AddValidator(new InventoryValidator(inventoryService));
    }
}
```

### 5. Caching Implementation

#### Cache Policies and Keys
```csharp
// Custom cache key
public class OrderCacheKey : CacheKey
{
    public OrderCacheKey(OrderId orderId) : base($"Order_{orderId}")
    {
        // 5 minute cache with 1 minute sliding expiration
        Policy = new CachePolicy
        {
            AbsoluteExpiration = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(1),
            Priority = CachePriority.High
        };
        
        // Add tags for cache invalidation
        Tags = new[] { "Orders", $"Customer_{orderId}" };
    }
}

// Cacheable query
public record GetOrderQuery(OrderId OrderId) : QueryBase<OrderDto>, ICacheableQuery
{
    public ICacheKey CacheKey => new OrderCacheKey(OrderId);
    public bool BypassCache { get; init; } = false;
}

// Using cache service directly
public class OrderService : ApplicationServiceBase
{
    private readonly ICacheService _cacheService;
    private readonly IOrderRepository _orderRepository;

    public async Task<OrderDto> GetOrderAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        var cacheKey = new OrderCacheKey(orderId);
        
        var cachedOrder = await _cacheService.GetAsync<OrderDto>(cacheKey, cancellationToken);
        if (cachedOrder != null)
        {
            return cachedOrder;
        }

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        var orderDto = Mapper.Map<OrderDto>(order);
        
        await _cacheService.SetAsync(cacheKey, orderDto, cancellationToken);
        
        return orderDto;
    }

    public async Task InvalidateOrderCacheAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        await _cacheService.RemoveByTagAsync("Orders", cancellationToken);
        await _cacheService.RemoveAsync(new OrderCacheKey(orderId), cancellationToken);
    }
}
```

### 6. Inbox/Outbox Pattern

#### Outbox Pattern Implementation
```csharp
// Publishing messages through outbox
public class OrderService : ApplicationServiceBase
{
    private readonly IOutboxService _outboxService;

    public async Task CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        using var transaction = await UnitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Create order in domain
            var order = new Order(OrderId.New(), command.CustomerId);
            await OrderRepository.AddAsync(order, cancellationToken);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            // Add integration event to outbox
            var integrationEvent = new OrderCreatedIntegrationEvent(order.Id, order.CustomerId, order.TotalAmount);
            await _outboxService.AddAsync(integrationEvent, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

// Background service processes outbox automatically
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationLayer();
        services.AddHostedService<OutboxBackgroundService>();
        services.AddHostedService<InboxBackgroundService>();
    }
}
```

#### Custom Message Handlers
```csharp
// Inbox message handler
public class OrderCreatedInboxHandler : IInboxMessageHandler<OrderCreatedIntegrationEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<OrderCreatedInboxHandler> _logger;

    public OrderCreatedInboxHandler(
        INotificationService notificationService,
        ILogger<OrderCreatedInboxHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedIntegrationEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing order created integration event for order {OrderId}", message.OrderId);
        
        await _notificationService.SendOrderNotificationAsync(
            message.CustomerId, 
            message.OrderId, 
            cancellationToken);
    }
}
```

### 7. Saga Pattern Implementation

#### Saga Definition
```csharp
public class OrderFulfillmentSaga : SagaBase<OrderFulfillmentSagaData>
{
    public OrderFulfillmentSaga(
        ISagaRepository<OrderFulfillmentSagaData> repository,
        ILogger<OrderFulfillmentSaga> logger) : base(repository, logger)
    {
    }

    protected override void ConfigureSteps()
    {
        Step("ReserveInventory")
            .Execute(async (data, ct) => await ReserveInventoryAsync(data, ct))
            .Compensate(async (data, ct) => await ReleaseInventoryAsync(data, ct));

        Step("ProcessPayment")
            .Execute(async (data, ct) => await ProcessPaymentAsync(data, ct))
            .Compensate(async (data, ct) => await RefundPaymentAsync(data, ct));

        Step("ShipOrder")
            .Execute(async (data, ct) => await ShipOrderAsync(data, ct))
            .Compensate(async (data, ct) => await CancelShipmentAsync(data, ct));

        Step("SendConfirmation")
            .Execute(async (data, ct) => await SendConfirmationAsync(data, ct));
    }

    private async Task ReserveInventoryAsync(OrderFulfillmentSagaData data, CancellationToken cancellationToken)
    {
        // Reserve inventory logic
        data.InventoryReserved = true;
        data.ReservationId = Guid.NewGuid();
    }

    private async Task ReleaseInventoryAsync(OrderFulfillmentSagaData data, CancellationToken cancellationToken)
    {
        // Release inventory compensation logic
        if (data.InventoryReserved && data.ReservationId.HasValue)
        {
            // Release reservation
            data.InventoryReserved = false;
        }
    }

    // Other step implementations...
}

// Saga data
public class OrderFulfillmentSagaData : ISagaData
{
    public Guid CorrelationId { get; set; }
    public OrderId OrderId { get; set; }
    public CustomerId CustomerId { get; set; }
    public bool InventoryReserved { get; set; }
    public Guid? ReservationId { get; set; }
    public bool PaymentProcessed { get; set; }
    public string? PaymentTransactionId { get; set; }
    public bool OrderShipped { get; set; }
    public string? TrackingNumber { get; set; }
    public SagaStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

// Starting a saga
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ISagaOrchestrator _sagaOrchestrator;

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        var sagaData = new OrderFulfillmentSagaData
        {
            CorrelationId = Guid.NewGuid(),
            OrderId = notification.OrderId,
            CustomerId = notification.CustomerId,
            Status = SagaStatus.Started,
            CreatedAt = DateTime.UtcNow
        };

        await _sagaOrchestrator.StartSagaAsync<OrderFulfillmentSaga>(sagaData, cancellationToken);
    }
}
```

### 8. Security and User Context

#### Security Context Usage
```csharp
public class OrderService : ApplicationServiceBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;

    public async Task<OrderDto> GetOrderAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.User;
        
        // Check permissions
        var hasPermission = await _permissionService.HasPermissionAsync(
            currentUser.UserId, 
            "Orders.Read", 
            cancellationToken);
            
        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("User does not have permission to read orders");
        }

        // Multi-tenant filtering
        var order = await OrderRepository.GetByIdAsync(orderId, cancellationToken);
        
        if (order.OrganizationId != currentUser.OrganizationId)
        {
            throw new UnauthorizedAccessException("User can only access orders from their organization");
        }

        return Mapper.Map<OrderDto>(order);
    }
}

// Custom security context
public class CustomUserContext : UserContext
{
    public string Department { get; set; }
    public List<string> AccessibleRegions { get; set; } = new();
    
    public bool CanAccessRegion(string region)
    {
        return AccessibleRegions.Contains(region) || Roles.Contains("GlobalAdmin");
    }
}
```

### 9. Application Services

#### Base Application Service
```csharp
public class OrderApplicationService : ApplicationServiceBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IDomainEventService _domainEventService;

    public OrderApplicationService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IDomainEventService domainEventService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<OrderApplicationService> logger) 
        : base(unitOfWork, mapper, logger)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _domainEventService = domainEventService;
    }

    public async Task<OrderId> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);

        using var transaction = await BeginTransactionAsync(cancellationToken);
        
        try
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new NotFoundException($"Customer {request.CustomerId} not found");
            }

            var order = new Order(OrderId.New(), request.CustomerId);
            
            foreach (var item in request.Items)
            {
                order.AddItem(item.ProductId, item.Quantity, item.UnitPrice);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            await _domainEventService.PublishEventsAsync(order.DomainEvents, cancellationToken);
            order.ClearDomainEvents();

            await CommitTransactionAsync(cancellationToken);
            
            Logger.LogInformation("Order {OrderId} created successfully", order.Id);
            return order.Id;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create order for customer {CustomerId}", request.CustomerId);
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
```

## Dependency Registration

### Complete Service Registration
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Core services
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<IDomainEventService, DomainEventService>();
        
        // Dispatchers
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<IMessageDispatcher, MessageDispatcher>();

        // Pipeline behaviors (order matters!)
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        // Caching
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.Configure<CacheSettings>(configuration.GetSection("Caching"));

        // Validation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Inbox/Outbox
        services.AddScoped<IInboxService, InboxService>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IInboxProcessor, InboxProcessor>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        
        // Background services
        services.AddHostedService<InboxBackgroundService>();
        services.AddHostedService<OutboxBackgroundService>();

        // Sagas
        services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();
        services.AddScoped(typeof(ISagaRepository<>), typeof(SagaRepository<>));

        // Security
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPermissionService, PermissionService>();

        // Mapping
        services.AddScoped<IMapper, AutoMapperAdapter>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Register all handlers automatically
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}

// Usage in Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationLayer(builder.Configuration);
```

### Configuration Settings
```json
{
  "Caching": {
    "DefaultPolicy": {
      "AbsoluteExpirationInMinutes": 30,
      "SlidingExpirationInMinutes": 5,
      "Priority": "Normal"
    },
    "Policies": {
      "ShortTerm": {
        "AbsoluteExpirationInMinutes": 5,
        "Priority": "High"
      },
      "LongTerm": {
        "AbsoluteExpirationInMinutes": 1440,
        "Priority": "Low"
      }
    }
  },
  "Performance": {
    "WarningThresholdMs": 1000,
    "ErrorThresholdMs": 5000
  },
  "Retry": {
    "MaxAttempts": 3,
    "BackoffMultiplier": 2,
    "BaseDelayMs": 1000
  },
  "InboxOutbox": {
    "ProcessingIntervalSeconds": 30,
    "BatchSize": 100,
    "MaxRetries": 5
  }
}
```

## Key Components Reference

### CQRS Components
- **ICommand/ICommand<T>**: Command interfaces with optional response
- **IQuery<T>**: Query interface with required response
- **IEvent**: Event interface for notifications
- **IMessage**: Base message interface
- **CommandBase/QueryBase**: Base implementations with common properties

### Pipeline Behaviors
- **IPipelineBehavior<TRequest, TResponse>**: Base behavior interface
- **LoggingBehavior**: Request/response logging with performance metrics
- **ValidationBehavior**: Automatic request validation
- **CachingBehavior**: Intelligent response caching
- **PerformanceBehavior**: Execution time monitoring
- **RetryBehavior**: Configurable retry mechanisms
- **TransactionBehavior**: Automatic transaction management

### Messaging Infrastructure
- **IEventBus**: Domain and integration event publishing
- **IMessageBus**: Generic message handling
- **IInboxService/IOutboxService**: Reliable message processing
- **InboxBackgroundService/OutboxBackgroundService**: Continuous message processing

### Validation Framework
- **IValidator<T>**: Validation interface
- **ValidatorBase<T>**: Base validator implementation
- **CompositeValidator<T>**: Combines multiple validators
- **ValidationResult**: Validation outcome with errors

### Caching System
- **ICacheService**: Cache abstraction
- **ICacheKey**: Structured cache keys
- **CachePolicy**: Expiration and priority policies
- **CacheSettings**: Configuration options

### Security & Context
- **ICurrentUserService**: Current user information
- **IPermissionService**: Authorization support
- **UserContext**: User context with roles and permissions
- **SecurityContext**: Security-related services

## Dependencies

This library depends on:
- **BuildingBlocks.Domain**: Domain layer abstractions
- **Microsoft.Extensions.Caching.Memory**: Memory caching support
- **Microsoft.Extensions.Hosting.Abstractions**: Background service support
- **Microsoft.Extensions.Logging.Abstractions**: Logging support
- **Microsoft.Extensions.DependencyInjection.Abstractions**: Dependency injection

## Architecture Integration

This library serves as the application layer in clean architecture:

```
📁 YourApplication/
├── 📁 Domain/ (BuildingBlocks.Domain) ← Pure business logic
├── 📁 Application/ (BuildingBlocks.Application) ← This library - Use cases & orchestration
├── 📁 Infrastructure/ (BuildingBlocks.Infrastructure) ← Data access & external services
├── 📁 API/ (BuildingBlocks.API) ← Controllers & web concerns
└── 📁 Tests/ ← Testing all layers
```

The application layer orchestrates domain operations, handles cross-cutting concerns through pipeline behaviors, and provides a clean interface for the presentation layer while maintaining independence from infrastructure concerns.