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

### Creating Entities with Strongly Typed IDs

```csharp
public class CustomerId : GuidId
{
    public CustomerId(Guid value) : base(value) { }
    public static CustomerId New() => new(Guid.NewGuid());
}

public class Customer : Entity<CustomerId>
{
    public string Name { get; private set; }
    public Email Email { get; private set; }

    public Customer(CustomerId id, string name, Email email) : base(id)
    {
        Name = name;
        Email = email;
    }
}
```

### Creating Aggregate Roots with Domain Events

```csharp
public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }

    public Order(OrderId id, CustomerId customerId) : base(id)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        AddDomainEvent(new OrderCreatedEvent(Id, CustomerId));
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(Id));
    }
}
```

### Using Value Objects

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

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
```

### Implementing Business Rules

```csharp
public class CustomerAgeRule : IBusinessRule
{
    private readonly int _age;

    public CustomerAgeRule(int age) => _age = age;

    public bool IsBroken() => _age < 18;
    public string Message => "Customer must be at least 18 years old";
}
```

## Key Components

### Strongly Typed IDs
- **GuidId**: GUID-based identifiers
- **IntId**: Integer-based identifiers  
- **LongId**: Long integer identifiers
- **StringId**: String-based identifiers
- **IStronglyTypedId**: Base interface for custom IDs

### Entity Framework Core Integration
- JSON converters for strongly typed IDs
- Value converters for Entity Framework
- Automatic configuration support

### Domain Events
- **IDomainEvent**: Base interface for domain events
- **DomainEventBase**: Base implementation
- **IDomainEventHandler**: Event handler interface
- **DomainEventDispatcher**: Event publishing

### Business Rules
- **IBusinessRule**: Business rule interface
- **BusinessRuleBase**: Base implementation
- **CompositeBusinessRule**: Multiple rule validation
- **BusinessRuleValidationException**: Rule violation exceptions

### Specifications
- **ISpecification**: Specification pattern interface
- **Specification<T>**: Base specification implementation
- **AndSpecification/OrSpecification**: Logical operators
- **SpecificationEvaluator**: Query building support

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