# BuildingBlocks.Domain

A comprehensive domain layer library providing essential building blocks for implementing Domain-Driven Design (DDD) patterns in .NET applications.

## Features

### 🏗️ Core Domain Patterns
- **Entities**: Base entity classes with strongly-typed IDs and proper equality
- **Aggregate Roots**: Root entities managing domain events and business invariants
- **Value Objects**: Immutable objects representing domain concepts with structural equality
- **Domain Events**: Event-driven communication within domain boundaries
- **Strongly Typed IDs**: Type-safe entity identifiers with JSON serialization support

### 📋 Business Logic & Rules
- **Business Rules**: Encapsulated business rule validation with composite support
- **Specifications**: Query object pattern for complex queries with logical operators
- **Domain Exceptions**: Specialized exceptions for different domain error scenarios
- **Extensions**: Helper methods for rule validation and domain operations

### 🗂️ Repository & Data Access
- **Repository Pattern**: Generic repository interfaces with async operations
- **Unit of Work**: Transaction coordination with commit/rollback support
- **Read-Only Repository**: Query-only data access with specification support
- **Specification Evaluator**: Dynamic query building from specifications

### 📦 Common Value Objects
- **Money**: Currency-aware monetary values with arithmetic operations
- **Email**: Validated email addresses with regex verification
- **Phone Number**: Validated international phone numbers
- **Address**: Structured address information with full address formatting
- **Enumeration**: Rich enum pattern for domain enumerations

### 🔧 Infrastructure Support
- **JSON Converters**: Automatic serialization for strongly typed IDs
- **Audit Support**: Interfaces for auditable and soft-deletable entities
- **DI Extensions**: Dependency injection helpers for domain services

## Installation

Add the project reference to your domain layer:

```xml
<ProjectReference Include="..\BuildingBlocks.Domain\BuildingBlocks.Domain.csproj" />
```

## Complete Usage Guide

### 1. Strongly Typed IDs

Create type-safe identifiers for your entities:

```csharp
// GUID-based ID
public class CustomerId : GuidId
{
    public CustomerId(Guid value) : base(value) { }
    public static CustomerId New() => new(Guid.NewGuid());
    protected CustomerId() { } // For EF Core
}

// Integer-based ID
public class ProductId : IntId
{
    public ProductId(int value) : base(value) { }
    protected ProductId() { } // For EF Core
}

// String-based ID
public class CategoryCode : StringId
{
    public CategoryCode(string value) : base(value) { }
    protected CategoryCode() { } // For EF Core
}
```

**JSON Serialization Support:**
```csharp
// Configure JSON options
services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory());
});

// Or create JsonSerializerOptions with support
var options = JsonSerializerOptions.CreateWithStronglyTypedIdSupport();
```

### 2. Entities and Aggregate Roots

#### Basic Entity
```csharp
public class Product : Entity<ProductId>
{
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public CategoryId CategoryId { get; private set; }

    public Product(ProductId id, string name, Money price, CategoryId categoryId) 
        : base(id)
    {
        Name = Guard.Against.NullOrWhiteSpace(name);
        Price = price ?? throw new ArgumentNullException(nameof(price));
        CategoryId = categoryId ?? throw new ArgumentNullException(nameof(categoryId));
    }

    protected Product() { } // For EF Core
}
```

#### Aggregate Root with Domain Events
```csharp
public class Order : AggregateRoot<OrderId>, IAuditableEntity
{
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Audit properties
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }

    public Order(OrderId id, CustomerId customerId) : base(id)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        TotalAmount = Money.Zero("USD");
        AddDomainEvent(new OrderCreatedEvent(Id, CustomerId));
    }

    public void AddItem(ProductId productId, int quantity, Money unitPrice)
    {
        this.CheckRule(new MinimumQuantityRule(quantity));
        
        var item = new OrderItem(productId, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal();
        
        AddDomainEvent(new OrderItemAddedEvent(Id, productId, quantity));
    }

    public void Confirm()
    {
        this.CheckRule(new OrderMustHaveItemsRule(_items));
        
        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(Id, TotalAmount));
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Aggregate(Money.Zero("USD"), 
            (sum, item) => sum.Add(item.TotalPrice));
    }

    protected Order() { } // For EF Core
}
```

### 3. Value Objects

#### Complex Value Object
```csharp
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string postalCode, string country)
    {
        Street = Guard.Against.NullOrWhiteSpace(street?.Trim());
        City = Guard.Against.NullOrWhiteSpace(city?.Trim());
        PostalCode = Guard.Against.NullOrWhiteSpace(postalCode?.Trim());
        Country = Guard.Against.NullOrWhiteSpace(country?.Trim());
    }

    public string FullAddress => $"{Street}, {City}, {PostalCode}, {Country}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }
}
```

#### Single Value Object
```csharp
public class ProductCode : SingleValueObject<string>
{
    public ProductCode(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Product code cannot be empty");
        if (value.Length != 8)
            throw new ArgumentException("Product code must be 8 characters");
    }
}
```

#### Using Built-in Value Objects
```csharp
// Money with currency operations
var price = new Money(100.50m, "USD");
var discountedPrice = price.Multiply(0.9m);
var total = price.Add(new Money(50m, "USD"));

// Email validation
var customerEmail = Email.From("customer@example.com");

// Phone number validation
var phoneNumber = new PhoneNumber("+1-555-123-4567");

// Enumeration pattern
public class Priority : Enumeration
{
    public static Priority Low = new(1, "Low");
    public static Priority Medium = new(2, "Medium");
    public static Priority High = new(3, "High");
    public static Priority Critical = new(4, "Critical");

    public Priority(int id, string name) : base(id, name) { }

    public static Priority FromValue(int value) => FromValue<Priority>(value);
    public static Priority FromName(string name) => FromDisplayName<Priority>(name);
    public static IEnumerable<Priority> All() => GetAll<Priority>();
}
```

### 4. Business Rules

#### Simple Business Rule
```csharp
public class MinimumQuantityRule : IBusinessRule
{
    private readonly int _quantity;

    public MinimumQuantityRule(int quantity) => _quantity = quantity;

    public bool IsBroken() => _quantity <= 0;
    public string Message => "Quantity must be greater than zero";
}
```

#### Composite Business Rule
```csharp
public class OrderValidationRules : CompositeBusinessRule
{
    public OrderValidationRules(Order order) : base(
        new OrderMustHaveItemsRule(order.Items),
        new OrderCustomerMustExistRule(order.CustomerId),
        new OrderTotalMustBePositiveRule(order.TotalAmount))
    {
    }
}

// Usage
public void ValidateOrder(Order order)
{
    var rules = new OrderValidationRules(order);
    this.CheckRule(rules);
}
```

#### Base Business Rule Implementation
```csharp
public class OrderService : BusinessRuleBase
{
    public void ProcessOrder(Order order)
    {
        CheckRule(new OrderMustHaveItemsRule(order.Items));
        CheckRule(new CustomerCreditLimitRule(order.CustomerId, order.TotalAmount));
        
        // Process order logic here
    }
}
```

### 5. Domain Events

#### Domain Event Definition
```csharp
public class OrderCreatedEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }

    public OrderCreatedEvent(OrderId orderId, CustomerId customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }
}
```

#### Domain Event Handler
```csharp
public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ICustomerRepository _customerRepository;

    public OrderCreatedEventHandler(IEmailService emailService, ICustomerRepository customerRepository)
    {
        _emailService = emailService;
        _customerRepository = customerRepository;
    }

    public async Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(domainEvent.CustomerId, cancellationToken);
        if (customer != null)
        {
            await _emailService.SendOrderConfirmationAsync(customer.Email, domainEvent.OrderId);
        }
    }
}
```

### 6. Repository Pattern

#### Repository Interface Implementation
```csharp
public interface IOrderRepository : IRepository<Order, OrderId>
{
    Task<IEnumerable<Order>> GetOrdersByCustomerAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderWithItemsAsync(OrderId orderId, CancellationToken cancellationToken = default);
}
```

#### Using Specifications
```csharp
public class OrdersByDateRangeSpec : Specification<Order>
{
    public OrdersByDateRangeSpec(DateTime startDate, DateTime endDate)
    {
        Criteria = order => order.CreatedAt >= startDate && order.CreatedAt <= endDate;
        AddInclude(order => order.Items);
        ApplyOrderByDescending(order => order.CreatedAt);
    }
}

public class OrdersByStatusSpec : Specification<Order>
{
    public OrdersByStatusSpec(OrderStatus status)
    {
        Criteria = order => order.Status == status;
    }
}

// Combining specifications
var recentPendingOrders = new OrdersByDateRangeSpec(DateTime.Today.AddDays(-7), DateTime.Today)
    .And(new OrdersByStatusSpec(OrderStatus.Pending));

var orders = await orderRepository.FindAsync(recentPendingOrders);
```

### 7. Unit of Work Pattern

```csharp
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<OrderId> CreateOrderAsync(CustomerId customerId, List<OrderItemData> items)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new AggregateNotFoundException($"Customer {customerId} not found");

            var order = new Order(OrderId.New(), customerId);
            
            foreach (var item in items)
            {
                order.AddItem(item.ProductId, item.Quantity, item.UnitPrice);
            }

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Dispatch domain events
            await _eventDispatcher.DispatchEventsAsync(order.DomainEvents);
            order.ClearDomainEvents();

            await _unitOfWork.CommitTransactionAsync();
            return order.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

### 8. Soft Delete Support

```csharp
public class Customer : Entity<CustomerId>, ISoftDeletable
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    
    // Soft delete properties
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        AddDomainEvent(new CustomerDeletedEvent(Id));
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        AddDomainEvent(new CustomerRestoredEvent(Id));
    }
}
```

## Dependency Registration

### ASP.NET Core Registration

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Register domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        
        // Register all domain event handlers
        services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<OrderConfirmedEvent>, OrderConfirmedEventHandler>();
        
        // Configure JSON serialization for strongly typed IDs
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        });
        
        return services;
    }
}

// In Program.cs
builder.Services.AddDomainServices();
```

### Entity Framework Core Configuration

```csharp
public class ApplicationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure strongly typed IDs
        modelBuilder.Entity<Order>()
            .Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => new OrderId(value));
                
        modelBuilder.Entity<Customer>()
            .Property(e => e.Id)
            .HasConversion(
                id => id.Value, 
                value => new CustomerId(value));

        // Configure value objects
        modelBuilder.Entity<Customer>()
            .OwnsOne(c => c.Address, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
                address.Property(a => a.Country).HasMaxLength(100);
            });

        // Configure money value object
        modelBuilder.Entity<Order>()
            .OwnsOne(o => o.TotalAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("TotalAmount");
                money.Property(m => m.Currency).HasColumnName("Currency");
            });

        // Soft delete global filter
        modelBuilder.Entity<Customer>()
            .HasQueryFilter(c => !c.IsDeleted);
    }
}
```

## Exception Handling

```csharp
public class GlobalExceptionHandler
{
    public async Task HandleAsync(Exception exception, HttpContext context)
    {
        var response = exception switch
        {
            BusinessRuleValidationException brve => new ErrorResponse
            {
                Title = "Business Rule Violation",
                Detail = brve.Message,
                StatusCode = 400
            },
            AggregateNotFoundException anfe => new ErrorResponse
            {
                Title = "Resource Not Found",
                Detail = anfe.Message,
                StatusCode = 404
            },
            ConcurrencyException ce => new ErrorResponse
            {
                Title = "Concurrency Conflict",
                Detail = "The resource was modified by another user",
                StatusCode = 409
            },
            DomainException de => new ErrorResponse
            {
                Title = "Domain Error",
                Detail = de.Message,
                StatusCode = 400
            },
            _ => new ErrorResponse
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred",
                StatusCode = 500
            }
        };

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

## Testing Support

```csharp
public class OrderTests
{
    [Fact]
    public void Order_AddItem_ShouldRaiseOrderItemAddedEvent()
    {
        // Arrange
        var orderId = OrderId.New();
        var customerId = CustomerId.New();
        var order = new Order(orderId, customerId);
        var productId = ProductId.New();
        var unitPrice = new Money(10m, "USD");

        // Act
        order.AddItem(productId, 2, unitPrice);

        // Assert
        order.DomainEvents.Should().ContainSingle(e => e is OrderItemAddedEvent);
        var domainEvent = order.DomainEvents.OfType<OrderItemAddedEvent>().Single();
        domainEvent.OrderId.Should().Be(orderId);
        domainEvent.ProductId.Should().Be(productId);
        domainEvent.Quantity.Should().Be(2);
    }

    [Fact]
    public void Order_AddItemWithZeroQuantity_ShouldThrowBusinessRuleValidationException()
    {
        // Arrange
        var order = new Order(OrderId.New(), CustomerId.New());
        var productId = ProductId.New();
        var unitPrice = new Money(10m, "USD");

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleValidationException>(() => 
            order.AddItem(productId, 0, unitPrice));
        
        exception.Message.Should().Contain("Quantity must be greater than zero");
    }
}
```

## Key Components Reference

### Strongly Typed IDs
- **GuidId**: GUID-based identifiers with empty validation
- **IntId**: Integer-based identifiers with positive validation
- **LongId**: Long integer identifiers with positive validation
- **StringId**: String-based identifiers with null/whitespace validation
- **IStronglyTypedId**: Base interface for custom implementations

### Entity Framework Core Integration
- **StronglyTypedIdJsonConverter**: Generic JSON converter for strongly typed IDs
- **StronglyTypedIdJsonConverterFactory**: Factory for automatic converter registration
- Value converters for Entity Framework Core mapping

### Domain Events
- **IDomainEvent**: Base interface with Id and OccurredOn properties
- **DomainEventBase**: Base implementation with auto-generated metadata
- **IDomainEventHandler<T>**: Generic handler interface with async support
- **DomainEventDispatcher**: Service provider-based event dispatching

### Business Rules
- **IBusinessRule**: Simple rule interface with IsBroken() and Message
- **BusinessRuleBase**: Base class with CheckRule() helper method
- **CompositeBusinessRule**: Combines multiple rules with AND logic
- **Extensions**: this.CheckRule() and this.CheckRules() extension methods

### Specifications
- **ISpecification<T>**: Complete specification interface with criteria, includes, ordering, and paging
- **Specification<T>**: Abstract base with protected helper methods
- **AndSpecification/OrSpecification/NotSpecification**: Logical combination operators
- **SpecificationEvaluator**: Converts specifications to IQueryable

### Value Objects
- **ValueObject**: Abstract base with GetEqualityComponents() for structural equality
- **SingleValueObject<T>**: Base for single-value wrappers
- **Enumeration**: Rich enum pattern with Id, Name, and discovery methods
- **Money**: Currency-aware monetary calculations
- **Email/PhoneNumber**: Validated communication value objects
- **Address**: Structured postal address with full formatting

## Dependencies

This library has minimal external dependencies:
- **Microsoft.Extensions.DependencyInjection.Abstractions**: For dependency injection support in domain event dispatcher

## Architecture Integration

This library serves as the core foundation of clean architecture:

```
📁 YourApplication/
├── 📁 Domain/ (Uses BuildingBlocks.Domain) ← Pure business logic
├── 📁 Application/ (Uses Domain) ← Use cases and application services
├── 📁 Infrastructure/ (Uses Domain) ← Data access and external services
├── 📁 API/ (Uses Application) ← Controllers and web concerns
└── 📁 Tests/ ← Testing all layers
```

The domain layer should only depend on this building blocks library and have no other external dependencies, maintaining the clean architecture principle of dependency inversion.