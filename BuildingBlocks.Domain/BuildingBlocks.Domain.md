# BuildingBlocks.Domain - Comprehensive Library Structure

## 🏗️ Architecture Overview

The BuildingBlocks.Domain library provides a comprehensive foundation for implementing Domain-Driven Design (DDD) patterns in .NET applications. It encapsulates core domain concepts like entities, value objects, aggregates, domain events, business rules, and specifications while maintaining clean separation of concerns and strong typing throughout the domain layer.

## 📋 Directory Structure

```
BuildingBlocks.Domain/
├── 📁 BusinessRules/                          # Business rule validation framework
│   ├── 📜 BusinessRuleBase.cs                 # Abstract base class with CheckRule method and validation logic
│   ├── 🔗 CompositeBusinessRule.cs            # Combines multiple business rules with logical operators
│   └── 🎯 IBusinessRule.cs                    # Business rule interface with IsBroken() method
├── 📁 Common/                                 # Common value objects and reusable types
│   ├── 🏠 Address.cs                          # Comprehensive postal address value object with validation
│   ├── 📅 DateRange.cs                        # Date range value object with overlap detection
│   ├── 📧 Email.cs                            # Email value object with regex validation and normalization
│   ├── 💰 Money.cs                            # Currency-aware monetary value object with arithmetic operations
│   └── 📱 PhoneNumber.cs                      # International phone number value object with format validation
├── 📁 DomainEvents/                           # Domain event pattern implementation for loose coupling
│   ├── 📡 DomainEventBase.cs                  # Base implementation with auto-generated ID and timestamp
│   ├── 🚀 DomainEventDispatcher.cs            # Service provider-based event dispatcher with error handling
│   ├── 📋 IDomainEvent.cs                     # Domain event interface with metadata properties
│   ├── 🎯 IDomainEventDispatcher.cs           # Event dispatcher interface with async support
│   └── 🔧 IDomainEventHandler.cs              # Generic event handler interface for type-safe handling
├── 📁 Entities/                               # Entity base classes and fundamental interfaces
│   ├── 🏛️ AggregateRoot.cs                    # Aggregate root with domain event management and consistency boundary
│   ├── 🧱 Entity.cs                           # Base entity class with strongly-typed IDs and proper equality
│   ├── 📊 IAuditableEntity.cs                 # Interface for automatic audit trail support
│   └── ❌ ISoftDeletable.cs                   # Interface for soft delete functionality with restore capability
├── 📁 Exceptions/                             # Domain-specific exception types for error handling
│   ├── 🔍 AggregateNotFoundException.cs       # Exception for missing aggregates with detailed context
│   ├── ⚠️ BusinessRuleValidationException.cs  # Exception for business rule violations with rule details
│   ├── 🔄 ConcurrencyException.cs             # Exception for optimistic concurrency conflicts
│   ├── 🚨 DomainException.cs                  # Base domain exception class with error categorization
│   └── 🚫 InvalidOperationDomainException.cs  # Exception for invalid domain operations with context
├── 📁 Extensions/                             # Extension methods for enhanced domain functionality
│   └── 🔧 DomainExtensions.cs                 # Business rule validation helpers and domain utilities
├── 📁 Guards/                                 # Input validation and precondition checking utilities
│   └── 🛡️ Guard.cs                            # Comprehensive guard clause implementations for parameter validation
├── 📁 Repository/                             # Repository pattern interfaces for data access abstraction
│   ├── 📖 IReadOnlyRepository.cs              # Read-only repository interface with query capabilities
│   ├── 📝 IRepository.cs                      # Full repository interface with CRUD operations
│   ├── 🔄 IUnitOfWork.cs                      # Unit of work pattern interface for transaction management
│   └── 📚 RepositoryBase.cs                   # Abstract repository base class with common functionality
├── 📁 Specifications/                         # Specification pattern for complex query logic
│   ├── ➕ AndSpecification.cs                 # Logical AND specification combiner for complex queries
│   ├── 📋 ISpecification.cs                   # Specification pattern interface with expression support
│   ├── ➖ NotSpecification.cs                 # Logical NOT specification negator for exclusion queries
│   ├── ➗ OrSpecification.cs                  # Logical OR specification combiner for union queries
│   ├── 📊 Specification.cs                    # Abstract specification base class with fluent API
│   └── ⚙️ SpecificationEvaluator.cs           # Converts specifications to IQueryable with optimization
├── 📁 StronglyTypedIds/                       # Type-safe entity identifiers to prevent primitive obsession
│   ├── 🆔 GuidId.cs                           # GUID-based strongly typed ID with validation
│   ├── 🔢 IntId.cs                            # Integer-based strongly typed ID with range validation
│   ├── 🎯 IStronglyTypedId.cs                 # Base interface for strongly typed IDs with value access
│   ├── 📁 Json/                               # JSON serialization support for APIs
│   │   ├── 🔄 StronglyTypedIdJsonConverter.cs # Generic JSON converter for all ID types
│   │   └── 🏭 StronglyTypedIdJsonConverterFactory.cs # Converter factory for automatic registration
│   ├── 📊 LongId.cs                           # Long integer-based strongly typed ID for large datasets
│   ├── 📝 StringId.cs                         # String-based strongly typed ID with format validation
│   └── 🏗️ StronglyTypedId.cs                 # Abstract base class for all strongly typed IDs
├── 📁 ValueObjects/                           # Value object base classes for immutable domain concepts
│   ├── 📋 Enumeration.cs                      # Rich enum pattern implementation with discovery methods
│   ├── 📦 SingleValueObject.cs                # Base for single-value value objects with implicit conversion
│   └── 💎 ValueObject.cs                      # Abstract base for complex value objects with structural equality
├── 📄 BuildingBlocks.Domain.csproj            # Project file with framework dependencies
├── 📚 BuildingBlocks.Domain.md                # This comprehensive structure documentation file
└── 📖 README.md                               # Complete usage documentation with examples
```

## 🎯 Component Categories & Architectural Patterns

### 🏗️ Core Domain Building Blocks (DDD Fundamentals)
- **Entities/**: Base classes for domain entities and aggregate roots with identity and lifecycle management
- **ValueObjects/**: Immutable value objects with structural equality and domain-specific validation
- **StronglyTypedIds/**: Type-safe entity identifiers preventing primitive obsession with ORM/JSON support
- **AggregateRoot**: Consistency boundary enforcement with domain event management

### 📋 Business Logic Framework (Domain Rules & Validation)
- **BusinessRules/**: Encapsulated business rule validation with composable rule patterns
- **Specifications/**: Query object pattern with logical operators for complex domain queries
- **Extensions/**: Fluent API helpers and domain operation utilities
- **Guards/**: Defensive programming utilities for input validation and precondition checking

### 🗂️ Data Access Abstractions (Repository Pattern)
- **Repository/**: Generic repository interfaces with specification support and base implementations
- **UnitOfWork/**: Transaction coordination and aggregate persistence management
- **Specifications/**: Complex query building with EF Core integration and performance optimization

### 📦 Common Domain Types (Reusable Components)
- **Common/**: Production-ready value objects (Money with currency, Email with validation, Address, PhoneNumber)
- **Exceptions/**: Specialized domain exception hierarchy with contextual error information
- **Enumeration/**: Rich enum pattern implementation with metadata and discovery capabilities

### 🔧 Infrastructure Support (Cross-Cutting Concerns)
- **DomainEvents/**: Event-driven architecture support with automatic dispatching and handler discovery
- **StronglyTypedIds/Json/**: Seamless JSON serialization for APIs and persistence layer integration
- **Auditing**: Built-in audit trail interfaces for creation/modification tracking

## 🔍 Detailed Implementation Features by Directory

### 📁 Entities/ - Domain Entity Foundation
- **Entity.cs**: 
  - Generic base entity class with strongly-typed ID support (`Entity<TId>`)
  - Proper equality implementation using ID comparison
  - Protected constructor for aggregate consistency
  - Built-in validation for entity invariants
- **AggregateRoot.cs**: 
  - Domain event collection and management (`AddDomainEvent`, `ClearDomainEvents`)
  - Aggregate boundary enforcement and consistency rules
  - Optimistic concurrency support with version tracking
  - Transactional integrity with UnitOfWork pattern
- **IAuditableEntity.cs**: 
  - Automatic audit trail with CreatedAt/CreatedBy tracking
  - Modification tracking with UpdatedAt/UpdatedBy fields
  - Integration with infrastructure interceptors
- **ISoftDeletable.cs**: 
  - Soft delete functionality with IsDeleted flag and DeletedAt timestamp
  - Restore capability for deleted entities
  - Automatic query filtering for soft-deleted entities

### 💎 ValueObjects/ - Immutable Domain Concepts
- **ValueObject.cs**: 
  - Structural equality through `GetEqualityComponents()` abstract method
  - Immutability enforcement through protected constructors
  - Value semantics with proper GetHashCode() and Equals() implementation
  - Copy and with-style methods for immutable updates
- **SingleValueObject.cs**: 
  - Wrapper for single-value value objects with type safety
  - Implicit conversion operators for seamless usage
  - Validation hooks for domain-specific constraints
- **Enumeration.cs**: 
  - Rich enum pattern with metadata and behavior
  - Discovery methods (`GetAll()`, `FromValue()`, `FromName()`)
  - Comparison operators and equality semantics
  - Extensible design for domain-specific enumerations

### 🆔 StronglyTypedIds/ - Type-Safe Identifiers
- **Type Safety Features**: 
  - Prevents primitive obsession with wrapper types
  - Compile-time type checking for ID parameters
  - Implicit/explicit conversion operators for convenience
- **JSON Integration**: 
  - Automatic serialization/deserialization with System.Text.Json
  - Custom converters for seamless API integration
  - Factory pattern for converter registration
- **Validation & Constraints**: 
  - Built-in validation for each ID type (non-empty GUID, positive integers)
  - Custom validation rules for domain-specific IDs
  - Error handling with descriptive exception messages
- **ORM Support**: 
  - Entity Framework Core value converter compatibility
  - Automatic conversion between ID types and database values
  - Query translation support for LINQ expressions

### 📡 DomainEvents/ - Event-Driven Architecture
- **Event Publishing System**: 
  - Automatic event dispatching through dependency injection
  - Weak event handling to prevent memory leaks
  - Exception isolation to prevent event handler failures
- **Handler Discovery & Management**: 
  - Dynamic handler resolution using service provider
  - Multiple handlers per event type support
  - Async/await pattern support with cancellation tokens
- **Event Metadata & Auditing**: 
  - Auto-generated event IDs for correlation and tracking
  - Timestamp generation for event ordering and debugging
  - Contextual information (user, tenant, correlation ID)

### 📜 BusinessRules/ - Domain Rule Validation
- **Single Rule Implementation**: 
  - `IBusinessRule` interface with `IsBroken()` method
  - Clear error messages for rule violations
  - Context-aware rule evaluation
- **Composite Rule Patterns**: 
  - `AndRule`, `OrRule` for logical combinations
  - `CompositeBusinessRule` for complex rule hierarchies
  - Short-circuit evaluation for performance optimization
- **Fluent Extension API**: 
  - `this.CheckRule()` extension methods for any object
  - Fluent chaining for multiple rule validation
  - Integration with entity invariant checking

### 🔍 Specifications/ - Query Object Pattern
- **Expression-Based Queries**: 
  - `Expression<Func<T, bool>>` foundation for type safety
  - Compile-time query validation
  - Reusable query logic across application layers
- **Logical Composition**: 
  - `And()`, `Or()`, `Not()` operators for complex queries
  - Fluent API for readable query construction
  - Performance-optimized expression tree manipulation
- **EF Core Deep Integration**: 
  - Automatic `Include()` generation for related entities
  - `OrderBy()` and `ThenBy()` support for sorting
  - Pagination with `Skip()` and `Take()` integration
  - Query caching and plan reuse

### 📚 Repository/ - Data Access Abstraction
- **Async-First Design**: 
  - All methods support async/await patterns with CancellationToken
  - Proper async enumeration with `IAsyncEnumerable<T>`
  - Task-based repository operations for scalability
- **Type-Safe Operations**: 
  - Generic interfaces with compile-time type checking
  - Strongly-typed ID support throughout the API
  - LINQ query integration with specifications
- **Advanced Query Support**: 
  - Specification pattern integration for complex queries
  - Projection support for DTOs and view models
  - Bulk operations for performance-critical scenarios
- **Transaction Management**: 
  - Unit of Work pattern for aggregate persistence
  - Transaction scope management with explicit boundaries
  - Rollback capabilities with exception handling

### 📦 Common/ - Production-Ready Value Objects
- **Money Value Object**: 
  - Multi-currency support with ISO 4217 currency codes
  - Arithmetic operations (+, -, *, /) with currency validation
  - Rounding strategies and precision handling
  - Exchange rate conversion support (interface)
- **Email Value Object**: 
  - RFC 5322 compliant email validation
  - Domain normalization and canonicalization
  - Implicit string conversion for convenience
  - Integration with email sending services
- **Address Value Object**: 
  - International address format support
  - Geocoding integration interfaces
  - Address validation and normalization
  - Formatting for different locales and postal systems
- **PhoneNumber Value Object**: 
  - International phone number validation (E.164 format)
  - Country code detection and validation
  - Format standardization and display formatting
  - SMS/calling service integration interfaces

## 🚀 Advanced Implementation Examples

### 📊 Entity & Aggregate Root Pattern
```csharp
// Strongly-typed entity with domain events
public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core constructor

    public static Order Create(CustomerId customerId)
    {
        var order = new Order
        {
            Id = OrderId.New(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            TotalAmount = Money.Zero(Currency.USD)
        };
        
        // Domain event automatically collected by AggregateRoot
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerId));
        return order;
    }
    
    public void AddItem(ProductId productId, int quantity, Money unitPrice)
    {
        this.CheckRule(new OrderMustBeInPendingStatusRule(Status));
        this.CheckRule(new QuantityMustBePositiveRule(quantity));
        
        var item = new OrderItem(productId, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal();
        
        AddDomainEvent(new OrderItemAddedEvent(Id, productId, quantity));
    }
}

// Strongly-typed ID with validation
public record OrderId : GuidId
{
    public OrderId(Guid value) : base(value) { }
    public static OrderId New() => new(Guid.NewGuid());
    public static OrderId From(Guid value) => new(value);
}
```

### 💎 Advanced Value Objects with Behavior
```csharp
// Money value object with currency operations
public class Money : ValueObject
{
    public decimal Amount { get; }
    public Currency CurrencyCode { get; }
    
    private Money(decimal amount, Currency currency)
    {
        Amount = Math.Round(amount, currency.DecimalPlaces);
        CurrencyCode = currency;
    }
    
    public static Money Create(decimal amount, Currency currency)
    {
        Guard.Against.Negative(amount, nameof(amount));
        Guard.Against.Null(currency, nameof(currency));
        return new Money(amount, currency);
    }
    
    public Money Add(Money other)
    {
        this.CheckRule(new SameCurrencyRule(CurrencyCode, other.CurrencyCode));
        return new Money(Amount + other.Amount, CurrencyCode);
    }
    
    public static Money operator +(Money left, Money right) => left.Add(right);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return CurrencyCode;
    }
}

// Rich enumeration with behavior
public class OrderStatus : Enumeration
{
    public static readonly OrderStatus Pending = new(1, nameof(Pending));
    public static readonly OrderStatus Confirmed = new(2, nameof(Confirmed));
    public static readonly OrderStatus Shipped = new(3, nameof(Shipped));
    public static readonly OrderStatus Delivered = new(4, nameof(Delivered));
    public static readonly OrderStatus Cancelled = new(5, nameof(Cancelled));
    
    public OrderStatus(int value, string name) : base(value, name) { }
    
    public bool CanTransitionTo(OrderStatus newStatus)
    {
        return (this, newStatus) switch
        {
            (var current, var target) when current == target => false,
            (var current, _) when current == Cancelled => false,
            (var current, _) when current == Delivered => false,
            (var current, var target) when current.Value < target.Value => true,
            _ => false
        };
    }
}
```

### 📜 Complex Business Rules & Specifications
```csharp
// Composite business rule with clear error messages
public class OrderCanBeConfirmedRule : CompositeBusinessRule
{
    public OrderCanBeConfirmedRule(Order order) : base()
    {
        AddRule(new OrderMustBeInPendingStatusRule(order.Status));
        AddRule(new OrderMustHaveItemsRule(order.Items));
        AddRule(new CustomerMustBeActiveRule(order.CustomerId));
        AddRule(new PaymentMustBeProcessedRule(order.Id));
    }
    
    public override string Message => "Order cannot be confirmed due to business rule violations";
}

// Specification for complex queries
public class ActiveCustomerOrdersSpec : Specification<Order>
{
    private readonly CustomerId _customerId;
    private readonly DateTime _fromDate;
    
    public ActiveCustomerOrdersSpec(CustomerId customerId, DateTime? fromDate = null)
    {
        _customerId = customerId;
        _fromDate = fromDate ?? DateTime.UtcNow.AddMonths(-6);
    }
    
    public override Expression<Func<Order, bool>> ToExpression()
    {
        return order => order.CustomerId == _customerId 
                    && order.CreatedAt >= _fromDate
                    && order.Status != OrderStatus.Cancelled
                    && !order.IsDeleted;
    }
    
    // Fluent specification composition
    public static ActiveCustomerOrdersSpec ForCustomer(CustomerId customerId) 
        => new(customerId);
        
    public ActiveCustomerOrdersSpec WithinLast(int months) 
        => new(_customerId, DateTime.UtcNow.AddMonths(-months));
}

// Usage in repository
var spec = ActiveCustomerOrdersSpec
    .ForCustomer(customerId)
    .WithinLast(3)
    .And(new OrdersAboveAmountSpec(Money.Create(100, Currency.USD)));

var orders = await _orderRepository.GetBySpecificationAsync(spec);
```

### 📡 Domain Event Handling
```csharp
// Domain event with rich metadata
public class OrderConfirmedEvent : DomainEventBase
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    public Money TotalAmount { get; }
    public DateTime ConfirmedAt { get; }
    public List<OrderItem> Items { get; }
    
    public OrderConfirmedEvent(OrderId orderId, CustomerId customerId, 
        Money totalAmount, List<OrderItem> items)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        ConfirmedAt = DateTime.UtcNow;
        Items = items.ToList(); // Defensive copy
    }
}

// Event handler with cross-cutting concerns
public class OrderConfirmedEventHandler : IDomainEventHandler<OrderConfirmedEvent>
{
    private readonly IInventoryService _inventoryService;
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderConfirmedEventHandler> _logger;
    
    public async Task HandleAsync(OrderConfirmedEvent domainEvent, 
        CancellationToken cancellationToken = default)
    {
        // Reserve inventory
        foreach (var item in domainEvent.Items)
        {
            await _inventoryService.ReserveAsync(item.ProductId, item.Quantity);
        }
        
        // Send confirmation email
        await _emailService.SendOrderConfirmationAsync(
            domainEvent.CustomerId, 
            domainEvent.OrderId);
            
        _logger.LogInformation("Order {OrderId} confirmed and processed", 
            domainEvent.OrderId);
    }
}
```

### 🔍 Advanced Repository with Specifications
```csharp
// Generic repository implementation with specifications
public class Repository<TEntity, TId> : IRepository<TEntity, TId> 
    where TEntity : Entity<TId>
    where TId : StronglyTypedId
{
    private readonly DbSet<TEntity> _dbSet;
    
    public async Task<TEntity?> GetBySpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_dbSet, specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<List<TEntity>> GetManyBySpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_dbSet, specification);
        return await query.ToListAsync(cancellationToken);
    }
    
    public async Task<PagedResult<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(_dbSet, specification);
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
            
        return new PagedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    }
}
```

## 🎯 Design Principles & Best Practices

### ✅ Domain-Driven Design Principles
- **Ubiquitous Language**: All classes and methods use domain terminology
- **Bounded Contexts**: Clear separation of domain concerns
- **Aggregate Boundaries**: Consistency boundaries with root entities
- **Domain Events**: Loose coupling between aggregates
- **Rich Domain Models**: Behavior-driven entities and value objects

### 🛡️ Defensive Programming
- **Guard Clauses**: Comprehensive input validation at boundaries
- **Immutability**: Immutable value objects and controlled entity mutations
- **Null Safety**: Explicit null handling and non-nullable design
- **Type Safety**: Strongly-typed IDs and compile-time error prevention

### ⚡ Performance Optimizations
- **Lazy Loading**: Controlled loading of related entities
- **Query Optimization**: Specification pattern with EF Core integration
- **Memory Efficiency**: Defensive copying and proper disposal patterns
- **Async Patterns**: Non-blocking operations with cancellation support

### 🔄 Testability Features
- **Pure Functions**: Side-effect-free domain logic
- **Dependency Injection**: Testable service abstractions
- **Factory Methods**: Controlled object creation for testing
- **Event Isolation**: Domain events can be tested independently

This comprehensive domain foundation provides a robust, maintainable, and testable base for implementing complex business logic while adhering to Domain-Driven Design principles and Clean Architecture patterns.