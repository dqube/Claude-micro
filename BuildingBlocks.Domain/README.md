# BuildingBlocks.Domain

A comprehensive domain layer library providing essential building blocks for implementing Domain-Driven Design (DDD) patterns in .NET applications.

## Features

### 🏗️ Core Domain Patterns
- **Entities**: Base entity classes with strongly-typed IDs
- **Aggregate Roots**: Root entities managing domain events
- **Value Objects**: Immutable objects representing domain concepts
- **Domain Events**: Event-driven communication within domain
- **Strongly Typed IDs**: Type-safe entity identifiers

### 📋 Business Logic & Rules
- **Business Rules**: Encapsulated business rule validation
- **Specifications**: Query object pattern for complex queries
- **Guards**: Input validation and precondition checking
- **Domain Exceptions**: Specialized exceptions for domain errors

### 🗂️ Repository & Data Access
- **Repository Pattern**: Generic repository interfaces
- **Unit of Work**: Transaction coordination
- **Read-Only Repository**: Query-only data access
- **Specification Evaluator**: Dynamic query building

### 📦 Common Value Objects
- **Money**: Currency-aware monetary values
- **Email**: Validated email addresses
- **Phone Number**: Validated phone numbers
- **Address**: Structured address information
- **Date Range**: Period representations

## Installation

Add the project reference to your domain layer:

```xml
<ProjectReference Include="..\BuildingBlocks.Domain\BuildingBlocks.Domain.csproj" />
```

## Quick Start

### 1. Creating Entities with Strongly Typed IDs

```csharp
// Define strongly typed ID
public class CustomerId : GuidId
{
    public CustomerId(Guid value) : base(value) { }
    public static CustomerId New() => new(Guid.NewGuid());
}

// Create entity
public class Customer : Entity<CustomerId>
{
    public string Name { get; private set; }
    public Email Email { get; private set; }

    public Customer(CustomerId id, string name, Email email) : base(id)
    {
        Name = name;
        Email = email;
    }

    // Business logic methods
    public void ChangeName(string newName)
    {
        Guard.Against.NullOrWhiteSpace(newName, nameof(newName));
        Name = newName;
    }
}
```

### 2. Creating Aggregate Roots with Domain Events

```csharp
public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }

    public Order(OrderId id, CustomerId customerId) : base(id)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        TotalAmount = Money.Zero("USD");
        
        AddDomainEvent(new OrderCreatedEvent(Id, CustomerId));
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(Id));
    }
}

// Domain events
public record OrderCreatedEvent(OrderId OrderId, CustomerId CustomerId) : DomainEventBase;
public record OrderConfirmedEvent(OrderId OrderId) : DomainEventBase;
```

### 3. Creating Value Objects

```csharp
public class CustomerName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    public CustomerName(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
    }

    public string FullName => $"{FirstName} {LastName}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}

// Using built-in value objects
var email = new Email("user@example.com");
var phone = new PhoneNumber("+1234567890");
var money = new Money(100.50m, "USD");
var address = new Address("123 Main St", "Anytown", "12345", "USA");
```

### 4. Implementing Business Rules

```csharp
public class CustomerAgeRule : IBusinessRule
{
    private readonly int _age;

    public CustomerAgeRule(int age)
    {
        _age = age;
    }

    public bool IsBroken() => _age < 18;
    
    public string Message => "Customer must be at least 18 years old";
}

// Usage in domain logic
public void RegisterCustomer(string name, int age)
{
    var ageRule = new CustomerAgeRule(age);
    ageRule.CheckRule(); // Throws BusinessRuleValidationException if broken
    
    // Continue with registration logic
}
```

### 5. Using Specifications

```csharp
public class ActiveCustomersSpecification : Specification<Customer>
{
    public override Expression<Func<Customer, bool>> Criteria => 
        customer => customer.IsActive && customer.RegistrationDate > DateTime.UtcNow.AddYears(-1);

    public ActiveCustomersSpecification()
    {
        AddInclude(c => c.Orders);
        ApplyOrderByDescending(c => c.RegistrationDate);
    }
}

public class CustomersByRegionSpecification : Specification<Customer>
{
    public override Expression<Func<Customer, bool>> Criteria { get; }

    public CustomersByRegionSpecification(string region)
    {
        Criteria = customer => customer.Address.Country == region;
    }
}

// Combining specifications
var activeInUSA = new ActiveCustomersSpecification()
    .And(new CustomersByRegionSpecification("USA"));
```

### 6. Repository Pattern Implementation

```csharp
public interface ICustomerRepository : IRepository<Customer, CustomerId>
{
    Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
}

// Usage with Unit of Work
public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerId> CreateCustomerAsync(string name, string email)
    {
        var customer = new Customer(
            CustomerId.New(),
            name,
            new Email(email)
        );

        await _customerRepository.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return customer.Id;
    }
}
```

## Advanced Usage

### Custom Strongly Typed IDs

```csharp
// GUID-based ID
public class ProductId : GuidId
{
    public ProductId(Guid value) : base(value) { }
    public static ProductId New() => new(Guid.NewGuid());
}

// Integer-based ID
public class CategoryId : IntId
{
    public CategoryId(int value) : base(value) { }
}

// String-based ID
public class SKU : StringId
{
    public SKU(string value) : base(value) { }
    
    protected override void Validate(string value)
    {
        if (value?.Length != 8)
            throw new ArgumentException("SKU must be exactly 8 characters");
    }
}
```

### Complex Business Rules

```csharp
public class CompositeOrderValidationRule : CompositeBusinessRule
{
    public CompositeOrderValidationRule(Order order)
    {
        AddRule(new OrderMinimumAmountRule(order.TotalAmount));
        AddRule(new OrderCustomerValidRule(order.CustomerId));
        AddRule(new OrderInventoryRule(order.Items));
    }
}

// Usage
var validationRule = new CompositeOrderValidationRule(order);
validationRule.CheckRule(); // Validates all rules
```

### Domain Event Handling

```csharp
public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly IEmailService _emailService;

    public OrderCreatedEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        // Send confirmation email
        await _emailService.SendOrderConfirmationAsync(domainEvent.OrderId);
    }
}
```

### Advanced Specifications

```csharp
public class OrdersByDateRangeSpecification : Specification<Order>
{
    public override Expression<Func<Order, bool>> Criteria { get; }

    public OrdersByDateRangeSpecification(DateRange dateRange)
    {
        Criteria = order => order.CreatedDate >= dateRange.Start && 
                           order.CreatedDate <= dateRange.End;
        
        AddInclude(o => o.Customer);
        AddInclude(o => o.OrderItems);
        ApplyOrderByDescending(o => o.CreatedDate);
        ApplyPaging(0, 50); // First 50 results
    }
}

// Complex specification combinations
var recentHighValueOrders = new OrdersByDateRangeSpecification(DateRange.LastMonth())
    .And(new OrdersByMinimumAmountSpecification(Money.FromAmount(1000, "USD")))
    .And(new OrdersByStatusSpecification(OrderStatus.Confirmed));
```

## Built-in Value Objects

### Money Operations

```csharp
var price = new Money(100.00m, "USD");
var discount = new Money(10.00m, "USD");

var finalPrice = price.Subtract(discount); // $90.00 USD
var doubled = price.Multiply(2); // $200.00 USD

// Currency validation
var euro = new Money(50.00m, "EUR");
// var invalid = price.Add(euro); // Throws InvalidOperationException
```

### Email Validation

```csharp
var email = new Email("user@example.com"); // Valid
// var invalid = new Email("invalid-email"); // Throws ArgumentException
```

### Date Range Operations

```csharp
var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(30));
var isInRange = range.Contains(DateTime.Today.AddDays(15)); // true
var duration = range.Duration; // TimeSpan of 30 days
```

## Exception Handling

```csharp
try
{
    var customer = await _customerRepository.GetByIdAsync(customerId);
    customer.ChangeEmail(newEmail);
}
catch (AggregateNotFoundException ex)
{
    // Handle entity not found
}
catch (BusinessRuleValidationException ex)
{
    // Handle business rule violation
    logger.LogWarning("Business rule violation: {Rule}", ex.BrokenRule.Message);
}
catch (DomainException ex)
{
    // Handle other domain exceptions
}
```

## Best Practices

### 1. Entity Design
- Always use strongly typed IDs
- Keep entities focused on their invariants
- Use aggregate roots to maintain consistency
- Emit domain events for important business moments

### 2. Value Object Usage
- Make value objects immutable
- Implement proper equality comparison
- Use for concepts that have no identity
- Validate input in constructors

### 3. Business Rules
- Make rules explicit and testable
- Use composite rules for complex validation
- Throw meaningful exceptions when rules are broken
- Keep rules close to the domain logic

### 4. Repository Implementation
- Use specifications for complex queries
- Keep repository interfaces in the domain layer
- Implement repositories in the infrastructure layer
- Use Unit of Work for transaction coordination

### 5. Domain Events
- Use for communication between aggregates
- Keep events immutable
- Include all relevant data in the event
- Handle events asynchronously when possible

## Dependencies

This library has minimal dependencies:
- **Microsoft.Extensions.DependencyInjection.Abstractions**: For dependency injection support

## Architecture Integration

This library serves as the foundation of clean architecture:

```
📁 YourApplication/
├── 📁 Domain/ (BuildingBlocks.Domain) ← This library
├── 📁 Application/
├── 📁 Infrastructure/
├── 📁 API/
└── 📁 Tests/
```

The domain layer is the core of your application, containing:
- **Business Logic**: Entities, value objects, and business rules
- **Domain Events**: Communication mechanism between aggregates
- **Abstractions**: Repository interfaces and domain services
- **Invariants**: Business rules and constraints