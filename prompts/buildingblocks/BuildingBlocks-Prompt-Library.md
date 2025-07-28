# BuildingBlocks Comprehensive Prompt Library
## Clean Architecture .NET Implementation Guide

> **A complete reference for building enterprise-grade .NET applications using the BuildingBlocks library suite**
> 
> **Based on real implementation patterns from microservices architecture**

---

## 📚 Table of Contents

1. [Overview & Architecture](#overview--architecture)
2. [Service Structure & Organization](#service-structure--organization)
3. [Domain Layer (BuildingBlocks.Domain)](#domain-layer-buildingblocksdomain)
4. [Application Layer (BuildingBlocks.Application)](#application-layer-buildingblocksapplication)
5. [Infrastructure Layer (BuildingBlocks.Infrastructure)](#infrastructure-layer-buildingblocksinfrastructure)
6. [API Layer (BuildingBlocks.API)](#api-layer-buildingblocksapi)
7. [Integration Patterns](#integration-patterns)
8. [Best Practices & Guidelines](#best-practices--guidelines)
9. [Troubleshooting & Common Issues](#troubleshooting--common-issues)

---

## 🏗️ Overview & Architecture

### Clean Architecture Principles

The BuildingBlocks library suite implements Clean Architecture with four distinct layers:

```
📁 Clean Architecture Structure
├── 🎨 Presentation (API Layer) ← BuildingBlocks.API
├── 📋 Application Layer ← BuildingBlocks.Application  
├── 🏭 Infrastructure Layer ← BuildingBlocks.Infrastructure
└── 🎯 Domain Layer ← BuildingBlocks.Domain (Core)
```

### Dependency Flow
```
API → Application → Domain
  ↓
Infrastructure → Application → Domain
```

### Key Architectural Benefits

- **Dependency Inversion**: Core business logic is independent of external concerns
- **Testability**: Each layer can be tested in isolation
- **Maintainability**: Clear separation of concerns and responsibilities  
- **Flexibility**: Easy to swap implementations without affecting business logic
- **Scalability**: Designed for microservices and distributed systems

---

## 🏢 Service Structure & Organization

### Microservice Architecture Pattern

Each service follows a consistent Clean Architecture structure with four layers:

```
📁 Services/{ServiceName}/
├── 🎯 Domain/                 # Pure business logic and rules
│   ├── Entities/              # Aggregate roots and entities
│   ├── ValueObjects/          # Value objects and strongly typed IDs
│   ├── Events/                # Domain events for business scenarios
│   ├── Exceptions/            # Domain-specific exceptions
│   ├── Repositories/          # Repository interfaces
│   └── DependencyInjection.cs
├── 📋 Application/            # Use cases and orchestration
│   ├── Commands/              # Write operations (Create, Update, Delete)
│   ├── Queries/               # Read operations with filtering and pagination
│   ├── DTOs/                  # Data transfer objects for API contracts
│   ├── EventHandlers/         # Domain event handlers
│   └── DependencyInjection.cs
├── 🏭 Infrastructure/         # External concerns and persistence
│   ├── Persistence/           # EF Core DbContext and configurations
│   ├── Repositories/          # Repository implementations
│   ├── Configurations/        # EF Core entity configurations
│   ├── Migrations/            # Database migrations
│   ├── Services/              # External service integrations
│   └── DependencyInjection.cs
├── 🌐 API/                    # HTTP concerns and endpoints
│   ├── Endpoints/             # Minimal API endpoint mappings
│   ├── Program.cs             # Service bootstrap and configuration
│   └── appsettings.json       # Service-specific configuration
└── 🧪 {ServiceName}.Tests/    # Service tests
    ├── Unit/                  # Unit tests for domain and application logic
    ├── Integration/           # Integration tests with database
    └── API/                   # API endpoint tests
```

### Service Naming Conventions

**Entities and Domain Objects:**
```csharp
// Entity naming: {Entity}.cs
public class Patient : AggregateRoot<PatientId> { }
public class User : AggregateRoot<UserId> { }
public class Contact : AggregateRoot<ContactId> { }

// Value Object naming: {Property}ValueObject.cs or descriptive names
public class PatientName : ValueObject { }
public class Email : SingleValueObject<string> { }
public class Address : ValueObject { }

// Strongly Typed ID naming: {Entity}Id.cs
public class PatientId : StronglyTypedId<Guid> { }
public class UserId : StronglyTypedId<Guid> { }
```

**Commands and Queries:**
```csharp
// Command naming: {Action}{Entity}Command.cs
public class CreatePatientCommand : CommandBase<PatientDto> { }
public class UpdatePatientContactCommand : CommandBase { }

// Query naming: Get{Entity}(s)Query.cs
public class GetPatientByIdQuery : IQuery<PatientDto> { }
public class GetPatientsQuery : IQuery<PagedResult<PatientDto>> { }

// Handler naming: {Command/Query}Handler.cs
public class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, PatientDto> { }
```

**Repository and Service Interfaces:**
```csharp
// Repository naming: I{Entity}Repository.cs
public interface IPatientRepository : IRepository<Patient, PatientId> { }

// Service naming: I{Entity}Service.cs (if needed)
public interface IPatientService { }
```

### Service Dependencies

Each service layer has specific dependency rules:

```csharp
// Domain Layer - No external dependencies
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.ValueObjects;

// Application Layer - Depends on Domain only
using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using {ServiceName}.Domain.Entities;

// Infrastructure Layer - Depends on Application and Domain
using BuildingBlocks.Infrastructure.Data.Context;
using {ServiceName}.Application.Commands;
using {ServiceName}.Domain.Entities;

// API Layer - Depends on Application only
using BuildingBlocks.API.Extensions;
using {ServiceName}.Application.Commands;
using {ServiceName}.Application.Queries;
```

### Configuration Structure

**Service Registration Pattern:**
```csharp
// Program.cs - Standard service setup
using BuildingBlocks.API.Extensions;
using {ServiceName}.Application;
using {ServiceName}.Domain;
using {ServiceName}.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add BuildingBlocks.API services
builder.Services.AddBuildingBlocksApi(builder.Configuration);

// Add service layers
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Use BuildingBlocks.API middleware
app.UseBuildingBlocksApi(builder.Configuration);

// Map service endpoints
app.Map{Entity}Endpoints();

app.Run();
```

**Layer Dependency Injection Pattern:**
```csharp
// Each layer provides its own DI configuration
public static class DependencyInjection
{
    public static IServiceCollection Add{LayerName}(
        this IServiceCollection services,
        IConfiguration configuration = null!)
    {
        // Register layer-specific services
        return services;
    }
}
```

---

## 🎯 Domain Layer (BuildingBlocks.Domain)

### Purpose & Responsibilities

The Domain layer contains the core business logic, entities, value objects, and domain rules. It has no dependencies on external frameworks or infrastructure.

### Core Components

#### 1. **Strongly Typed IDs**

**Implementation Pattern:**
```csharp
// Standard strongly typed GUID ID pattern
public class {Entity}Id : StronglyTypedId<Guid>
{
    public {Entity}Id(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("{Entity}Id cannot be empty", nameof(value));
    }
    
    public static {Entity}Id New() => new(Guid.NewGuid());
    public static {Entity}Id From(Guid value) => new(value);
}

// Alternative GuidId base class pattern
public class {Entity}Id : GuidId
{
    public {Entity}Id(Guid value) : base(value) { }
    public static {Entity}Id New() => new(Guid.NewGuid());
    public static {Entity}Id From(Guid value) => new(value);
}

// Usage in entities
public class {Entity} : AggregateRoot<{Entity}Id>
{
    public {Entity}({Entity}Id id, string name) : base(id)
    {
        Name = name;
    }
}
```

**Best Practices:**
- Always validate the ID value in the constructor
- Use static factory methods for ID generation
- Implement proper JSON serialization support (automatically handled by BuildingBlocks)
- Prefer Guid for distributed systems, int for performance-critical scenarios

#### 2. **Entities & Aggregate Roots**

**Aggregate Root Pattern:**
```csharp
public class {Entity} : AggregateRoot<{Entity}Id>
{
    // Value Objects for complex properties
    public {Entity}Name Name { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Address? Address { get; private set; }
    
    // Simple properties with business meaning
    public DateTime DateOfBirth { get; private set; }
    public {Entity}Type Type { get; private set; }
    
    // Entity state management
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core constructor - required for entity framework
    private {Entity}() : base({Entity}Id.New()) 
    { 
        // Initialize required non-nullable properties with default values
        Name = new {Entity}Name("Unknown", "Unknown");
        Email = new Email("unknown@temp.com");
        Type = {Entity}Type.Default;
    }

    // Domain constructor with required business invariants
    public {Entity}(
        {Entity}Id id,
        {Entity}Name name,
        Email email,
        DateTime dateOfBirth,
        {Entity}Type type) : base(id)
    {
        ValidateDateOfBirth(dateOfBirth);

        Name = name;
        Email = email;
        DateOfBirth = dateOfBirth;
        Type = type;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new {Entity}CreatedEvent(Id, Name, Email));
    }

    // Business operations that maintain invariants
    public void UpdateContactInformation(Email email, PhoneNumber? phoneNumber = null)
    {
        Email = email;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new {Entity}ContactUpdatedEvent(Id, Email, PhoneNumber));
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new {Entity}AddressUpdatedEvent(Id, Address));
    }

    // State transitions with business rules
    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new {Entity}DeactivatedEvent(Id));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new {Entity}ActivatedEvent(Id));
    }

    // Business logic calculations
    public int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        
        if (DateOfBirth.Date > today.AddYears(-age))
            age--;

        return age;
    }

    // Private business rule validations
    private static void ValidateDateOfBirth(DateTime dateOfBirth)
    {
        if (dateOfBirth > DateTime.Today)
            throw new ArgumentException("Date of birth cannot be in the future", nameof(dateOfBirth));

        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
            age--;

        if (age > 150)
            throw new ArgumentException("Age cannot exceed 150 years", nameof(dateOfBirth));
    }
}
```

**Entity Pattern (without collections):**
```csharp
public class {SimpleEntity} : AggregateRoot<{SimpleEntity}Id>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public {SimpleEntity}Type Type { get; private set; }
    public bool IsActive { get; private set; }

    private {SimpleEntity}() 
    { 
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = null!;
        Type = null!;
    }

    public {SimpleEntity}(
        {SimpleEntity}Id id,
        string firstName,
        string lastName,
        Email email,
        {SimpleEntity}Type type) : base(id)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        IsActive = true;

        AddDomainEvent(new {SimpleEntity}CreatedEvent(Id, Email));
    }

    public string FullName => $"{FirstName} {LastName}";

    // Information update operations with change detection
    public void UpdateContactInformation(Email email)
    {
        if (!Email.Equals(email))
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            AddDomainEvent(new {SimpleEntity}ContactUpdatedEvent(Id, Email));
        }
    }

    // Private validation helpers
    private static string ValidateName(string name, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{parameterName} cannot be null or empty", parameterName);
        
        return name.Trim();
    }
}
```

#### 3. **Value Objects**

**Complex Name Value Object:**
```csharp
public class {Entity}Name : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }

    public {Entity}Name(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim();
    }

    public string FullName => string.IsNullOrEmpty(MiddleName) 
        ? $"{FirstName} {LastName}" 
        : $"{FirstName} {MiddleName} {LastName}";

    public string DisplayName => $"{LastName}, {FirstName}" + 
        (string.IsNullOrEmpty(MiddleName) ? "" : $" {MiddleName[0]}.");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return MiddleName;
    }

    public override string ToString() => FullName;
}
```

**Complex Address Value Object:**
```csharp
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string postalCode, string country)
    {
        Street = ValidateAndTrim(street, nameof(street));
        City = ValidateAndTrim(city, nameof(city));
        PostalCode = ValidateAndTrim(postalCode, nameof(postalCode));
        Country = ValidateAndTrim(country, nameof(country));
    }

    public string FullAddress => $"{Street}, {City}, {PostalCode}, {Country}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString() => FullAddress;

    private static string ValidateAndTrim(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{parameterName} cannot be null or empty", parameterName);
        return value.Trim();
    }
}
```

**Single Value Objects:**
```csharp
// Email with validation
public class Email : SingleValueObject<string>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    public Email(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty");
        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format");
    }

    public static Email From(string email) => new(email);
}

// Phone number with validation
public class PhoneNumber : SingleValueObject<string>
{
    public PhoneNumber(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));
        
        var cleaned = CleanPhoneNumber(value);
        if (cleaned.Length < 10)
            throw new ArgumentException("Phone number must be at least 10 digits", nameof(value));
    }
    
    public static PhoneNumber From(string phoneNumber) => new(phoneNumber);
    
    private static string CleanPhoneNumber(string phoneNumber)
    {
        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }
}

// Validated string value object
public class {Property}Code : SingleValueObject<string>
{
    public {Property}Code(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("{Property} code cannot be null or empty", nameof(value));
        
        if (value.Length < 3)
            throw new ArgumentException("{Property} code must be at least 3 characters long", nameof(value));
        
        if (value.Length > 20)
            throw new ArgumentException("{Property} code cannot exceed 20 characters", nameof(value));
    }
    
    public static {Property}Code From(string value) => new(value);
}
```

**Enumeration Value Objects:**
```csharp
public class {Entity}Type : Enumeration
{
    public static readonly {Entity}Type Type1 = new(1, "Type1");
    public static readonly {Entity}Type Type2 = new(2, "Type2");
    public static readonly {Entity}Type Type3 = new(3, "Type3");
    public static readonly {Entity}Type Default = Type1;

    private {Entity}Type(int id, string name) : base(id, name) { }

    public static {Entity}Type FromName(string name)
    {
        var type = GetAll<{Entity}Type>().SingleOrDefault(t => 
            string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        
        return type ?? throw new ArgumentException($"Invalid {Entity.ToLower()} type: {name}", nameof(name));
    }

    public static {Entity}Type FromId(int id)
    {
        var type = GetAll<{Entity}Type>().SingleOrDefault(t => t.Id == id);
        return type ?? throw new ArgumentException($"Invalid {Entity.ToLower()} type ID: {id}", nameof(id));
    }
}

// Example: Gender enumeration
public class Gender : Enumeration
{
    public static readonly Gender Male = new(1, "Male");
    public static readonly Gender Female = new(2, "Female");
    public static readonly Gender Other = new(3, "Other");
    public static readonly Gender PreferNotToSay = new(4, "Prefer not to say");

    private Gender(int id, string name) : base(id, name) { }

    public static Gender FromName(string name)
    {
        var gender = GetAll<Gender>().SingleOrDefault(g => 
            string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase));
        
        return gender ?? throw new ArgumentException($"Invalid gender: {name}", nameof(name));
    }

    public static Gender FromId(int id)
    {
        var gender = GetAll<Gender>().SingleOrDefault(g => g.Id == id);
        return gender ?? throw new ArgumentException($"Invalid gender ID: {id}", nameof(id));
    }
}
```

#### 4. **Business Rules**

**Simple Business Rule:**
```csharp
public class MinimumQuantityRule : IBusinessRule
{
    private readonly int _quantity;

    public MinimumQuantityRule(int quantity) => _quantity = quantity;

    public bool IsBroken() => _quantity <= 0;
    public string Message => "Quantity must be greater than zero";
}
```

**Composite Business Rule:**
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

#### 5. **Domain Events**

**Domain Event Definition Pattern:**
```csharp
// Simple domain event with basic information
public record {Entity}CreatedEvent(
    {Entity}Id {Entity}Id,
    {Entity}Name Name,
    Email Email) : DomainEventBase;

// Domain event with additional properties
public record {Entity}ContactUpdatedEvent(
    {Entity}Id {Entity}Id,
    Email Email,
    PhoneNumber? PhoneNumber) : DomainEventBase;

// Domain event with optional properties
public record {Entity}AddressUpdatedEvent(
    {Entity}Id {Entity}Id,
    Address? Address) : DomainEventBase;

// Simple state change events
public record {Entity}ActivatedEvent({Entity}Id {Entity}Id) : DomainEventBase;
public record {Entity}DeactivatedEvent({Entity}Id {Entity}Id) : DomainEventBase;

// Complex domain event with multiple properties
public record {Entity}ProfileUpdatedEvent(
    {Entity}Id {Entity}Id,
    string FirstName,
    string LastName,
    string? Company,
    string? JobTitle) : DomainEventBase;
```

**Domain Event Examples:**
```csharp
// Healthcare domain events
public record PatientCreatedEvent(
    PatientId PatientId,
    MedicalRecordNumber MedicalRecordNumber,
    PatientName Name) : DomainEventBase;

public record PatientContactUpdatedEvent(
    PatientId PatientId,
    Email Email,
    PhoneNumber? PhoneNumber) : DomainEventBase;

public record PatientBloodTypeUpdatedEvent(
    PatientId PatientId,
    BloodType BloodType) : DomainEventBase;

// Authentication domain events
public record UserCreatedEvent(
    UserId UserId,
    Username Username,
    Email Email) : DomainEventBase;

public record UserPasswordUpdatedEvent(UserId UserId) : DomainEventBase;

public record UserRoleAssignedEvent(
    UserId UserId,
    RoleId RoleId,
    UserId AssignedBy) : DomainEventBase;

// Contact management domain events
public record ContactCreatedEvent(
    ContactId ContactId,
    Email Email,
    PhoneNumber? PhoneNumber) : DomainEventBase;

public record ContactNotesUpdatedEvent(
    ContactId ContactId,
    string? Notes) : DomainEventBase;
```

#### 6. **Repository Patterns**

**Basic Repository Interface:**
```csharp
public interface I{Entity}Repository : IRepository<{Entity}, {Entity}Id>
{
    // Basic entity-specific queries
    Task<{Entity}?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<{Entity}>> GetBy{Property}Async({PropertyType} {property}, CancellationToken cancellationToken = default);
    Task<IEnumerable<{Entity}>> GetActive{Entity}sAsync(CancellationToken cancellationToken = default);
    
    // Business-specific queries
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<{Entity}>> SearchBy{Property}Async(string searchTerm, CancellationToken cancellationToken = default);
}
```

**Domain-Specific Repository Examples:**
```csharp
// Healthcare repository
public interface IPatientRepository : IRepository<Patient, PatientId>
{
    Task<Patient?> GetByMedicalRecordNumberAsync(MedicalRecordNumber mrn, CancellationToken cancellationToken = default);
    Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetByGenderAsync(Gender gender, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetByBloodTypeAsync(BloodType bloodType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetActivePatientsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetPatientsByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithMedicalRecordNumberAsync(MedicalRecordNumber mrn, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
}

// Authentication repository
public interface IUserRepository : IRepository<User, UserId>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetLockedOutUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<int> GetFailedLoginAttemptsAsync(UserId userId, CancellationToken cancellationToken = default);
}

// Contact repository
public interface IContactRepository : IRepository<Contact, ContactId>
{
    Task<Contact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contact>> GetByContactTypeAsync(ContactType contactType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contact>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contact>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
}
```

**Using Specifications:**
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

// Combining specifications
var recentPendingOrders = new OrdersByDateRangeSpec(DateTime.Today.AddDays(-7), DateTime.Today)
    .And(new OrdersByStatusSpec(OrderStatus.Pending));

var orders = await orderRepository.FindAsync(recentPendingOrders);
```

### Domain Layer Implementation Checklist

- [ ] Define strongly typed IDs for all entities
- [ ] Implement proper entity encapsulation with private setters
- [ ] Create value objects for complex domain concepts
- [ ] Define business rules for domain invariants
- [ ] Implement domain events for cross-aggregate communication
- [ ] Use specifications for complex queries
- [ ] Implement repository interfaces for data access abstraction
- [ ] Add proper validation and guard clauses
- [ ] Ensure entities are persistence-ignorant
- [ ] Document domain concepts and business rules

---

## 📋 Application Layer (BuildingBlocks.Application)

### Purpose & Responsibilities

The Application layer orchestrates domain operations, handles use cases, and provides a clean interface between the API and Domain layers. It implements CQRS, pipeline behaviors, and application services.

### Core Components

#### 1. **CQRS Implementation**

**Command Pattern:**
```csharp
// Command with result
public class Create{Entity}Command : CommandBase<{Entity}Dto>
{
    public string Property1 { get; init; }
    public string Property2 { get; init; }
    public string? OptionalProperty { get; init; }
    public DateTime DateProperty { get; init; }
    public string EnumProperty { get; init; }
    public AddressDto? Address { get; init; }

    public Create{Entity}Command(
        string property1,
        string property2,
        string? optionalProperty,
        DateTime dateProperty,
        string enumProperty,
        AddressDto? address = null)
    {
        Property1 = property1;
        Property2 = property2;
        OptionalProperty = optionalProperty;
        DateProperty = dateProperty;
        EnumProperty = enumProperty;
        Address = address;
    }
}

// Command without result
public class Update{Entity}ContactCommand : CommandBase
{
    public {Entity}Id {Entity}Id { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }

    public Update{Entity}ContactCommand({Entity}Id {entity.ToLower()}Id, string email, string? phoneNumber = null)
    {
        {Entity}Id = {entity.ToLower()}Id;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}

// Command handler with result
public class Create{Entity}CommandHandler : ICommandHandler<Create{Entity}Command, {Entity}Dto>
{
    private readonly IRepository<{Entity}, {Entity}Id> _{entity.ToLower()}Repository;
    private readonly IUnitOfWork _unitOfWork;

    public Create{Entity}CommandHandler(
        IRepository<{Entity}, {Entity}Id> {entity.ToLower()}Repository,
        IUnitOfWork unitOfWork)
    {
        _{entity.ToLower()}Repository = {entity.ToLower()}Repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<{Entity}Dto> HandleAsync(Create{Entity}Command request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var {entity.ToLower()}Id = {Entity}Id.New();
        var name = new {Entity}Name(request.FirstName, request.LastName, request.MiddleName);
        var email = new Email(request.Email);
        var enumValue = {Entity}Type.FromName(request.EnumProperty);

        var {entity.ToLower()} = new {Entity}({entity.ToLower()}Id, name, email, request.DateProperty, enumValue);

        // Add optional fields safely
        if (!string.IsNullOrWhiteSpace(request.OptionalProperty))
        {
            var optionalValue = new OptionalValueObject(request.OptionalProperty);
            {entity.ToLower()}.UpdateOptionalProperty(optionalValue);
        }

        if (request.Address is not null)
        {
            var address = new Address(
                request.Address.Street ?? string.Empty,
                request.Address.City ?? string.Empty,
                request.Address.PostalCode ?? string.Empty,
                request.Address.Country ?? string.Empty);
            {entity.ToLower()}.UpdateAddress(address);
        }

        await _{entity.ToLower()}Repository.AddAsync({entity.ToLower()}, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto({entity.ToLower()});
    }

    private static {Entity}Dto MapToDto({Entity} {entity.ToLower()})
    {
        return new {Entity}Dto
        {
            Id = {entity.ToLower()}.Id.Value,
            Property1 = {entity.ToLower()}.Property1.Value,
            Property2 = {entity.ToLower()}.Property2.Value,
            FullName = {entity.ToLower()}.Name.FullName,
            DisplayName = {entity.ToLower()}.Name.DisplayName,
            Email = {entity.ToLower()}.Email.Value,
            OptionalProperty = {entity.ToLower()}.OptionalProperty?.Value,
            DateProperty = {entity.ToLower()}.DateProperty,
            EnumProperty = {entity.ToLower()}.EnumProperty.Name,
            IsActive = {entity.ToLower()}.IsActive,
            CreatedAt = {entity.ToLower()}.CreatedAt,
            UpdatedAt = {entity.ToLower()}.UpdatedAt
        };
    }
}

// Command handler without result
public class Update{Entity}ContactCommandHandler : ICommandHandler<Update{Entity}ContactCommand>
{
    private readonly IRepository<{Entity}, {Entity}Id> _{entity.ToLower()}Repository;
    private readonly IUnitOfWork _unitOfWork;

    public Update{Entity}ContactCommandHandler(
        IRepository<{Entity}, {Entity}Id> {entity.ToLower()}Repository,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull({entity.ToLower()}Repository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _{entity.ToLower()}Repository = {entity.ToLower()}Repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(Update{Entity}ContactCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var {entity.ToLower()} = await _{entity.ToLower()}Repository.GetByIdAsync(request.{Entity}Id, cancellationToken);
        if ({entity.ToLower()} is null)
            throw new {Entity}NotFoundException(request.{Entity}Id);

        var email = new Email(request.Email);
        var phoneNumber = string.IsNullOrEmpty(request.PhoneNumber)
            ? null
            : new PhoneNumber(request.PhoneNumber);

        {entity.ToLower()}.UpdateContactInformation(email, phoneNumber);
        _{entity.ToLower()}Repository.Update({entity.ToLower()});
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

**Query Pattern:**
```csharp
// Single entity query
public class Get{Entity}ByIdQuery : IRequest<{Entity}Dto?>
{
    public {Entity}Id Id { get; init; }

    public Get{Entity}ByIdQuery({Entity}Id id)
    {
        Id = id;
    }
}

// Paged query with filtering and sorting
public class Get{Entity}sQuery : IRequest<PagedResult<{Entity}Dto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? EntityType { get; init; }
    public bool? IsActive { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;

    public Get{Entity}sQuery(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        string? entityType,
        bool? isActive,
        string sortBy,
        bool sortDescending)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        EntityType = entityType;
        IsActive = isActive;
        SortBy = sortBy;
        SortDescending = sortDescending;
    }
}

// Single entity query handler
public class Get{Entity}ByIdQueryHandler : IRequestHandler<Get{Entity}ByIdQuery, {Entity}Dto?>
{
    private readonly IRepository<{Entity}, {Entity}Id> _{entity.ToLower()}Repository;

    public Get{Entity}ByIdQueryHandler(IRepository<{Entity}, {Entity}Id> {entity.ToLower()}Repository)
    {
        _{entity.ToLower()}Repository = {entity.ToLower()}Repository;
    }

    public async Task<{Entity}Dto?> Handle(Get{Entity}ByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var {entity.ToLower()} = await _{entity.ToLower()}Repository.GetByIdAsync(request.Id, cancellationToken);
        if ({entity.ToLower()} is null)
            return null;

        return MapToDto({entity.ToLower()});
    }

    private static {Entity}Dto MapToDto({Entity} {entity.ToLower()})
    {
        return new {Entity}Dto
        {
            Id = {entity.ToLower()}.Id.Value,
            Property1 = {entity.ToLower()}.Property1.Value,
            Property2 = {entity.ToLower()}.Property2.Value,
            FullName = {entity.ToLower()}.Name.FullName,
            DisplayName = {entity.ToLower()}.Name.DisplayName,
            Email = {entity.ToLower()}.Email.Value,
            DateProperty = {entity.ToLower()}.DateProperty,
            EnumProperty = {entity.ToLower()}.EnumProperty.Name,
            IsActive = {entity.ToLower()}.IsActive,
            CreatedAt = {entity.ToLower()}.CreatedAt,
            UpdatedAt = {entity.ToLower()}.UpdatedAt
        };
    }
}

// Paged query handler
public class Get{Entity}sQueryHandler : IRequestHandler<Get{Entity}sQuery, PagedResult<{Entity}Dto>>
{
    private readonly IRepository<{Entity}, {Entity}Id> _{entity.ToLower()}Repository;

    public Get{Entity}sQueryHandler(IRepository<{Entity}, {Entity}Id> {entity.ToLower()}Repository)
    {
        _{entity.ToLower()}Repository = {entity.ToLower()}Repository;
    }

    public async Task<PagedResult<{Entity}Dto>> Handle(Get{Entity}sQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var {entity.ToLower()}s = await _{entity.ToLower()}Repository.GetAllAsync(cancellationToken);

        // Apply filtering
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            {entity.ToLower()}s = {entity.ToLower()}s.Where({entity.ToLower()} =>
                {entity.ToLower()}.Name.FullName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                {entity.ToLower()}.Email.Value.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.EntityType))
        {
            {entity.ToLower()}s = {entity.ToLower()}s.Where({entity.ToLower()} => {entity.ToLower()}.Type.Name.Equals(request.EntityType, StringComparison.OrdinalIgnoreCase));
        }

        if (request.IsActive.HasValue)
        {
            {entity.ToLower()}s = {entity.ToLower()}s.Where({entity.ToLower()} => {entity.ToLower()}.IsActive == request.IsActive.Value);
        }

        // Apply sorting
        {entity.ToLower()}s = request.SortBy.ToLowerInvariant() switch
        {
            "name" => request.SortDescending 
                ? {entity.ToLower()}s.OrderByDescending({entity.ToLower()} => {entity.ToLower()}.Name.FullName)
                : {entity.ToLower()}s.OrderBy({entity.ToLower()} => {entity.ToLower()}.Name.FullName),
            "email" => request.SortDescending 
                ? {entity.ToLower()}s.OrderByDescending({entity.ToLower()} => {entity.ToLower()}.Email.Value)
                : {entity.ToLower()}s.OrderBy({entity.ToLower()} => {entity.ToLower()}.Email.Value),
            "createdat" or _ => request.SortDescending 
                ? {entity.ToLower()}s.OrderByDescending({entity.ToLower()} => {entity.ToLower()}.CreatedAt)
                : {entity.ToLower()}s.OrderBy({entity.ToLower()} => {entity.ToLower()}.CreatedAt)
        };

        // Apply pagination
        var totalCount = {entity.ToLower()}s.Count();
        var pagedItems = {entity.ToLower()}s
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = pagedItems.Select(MapToDto).ToList();

        return new PagedResult<{Entity}Dto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    private static {Entity}Dto MapToDto({Entity} {entity.ToLower()})
    {
        return new {Entity}Dto
        {
            Id = {entity.ToLower()}.Id.Value,
            Property1 = {entity.ToLower()}.Property1.Value,
            Property2 = {entity.ToLower()}.Property2.Value,
            FullName = {entity.ToLower()}.Name.FullName,
            DisplayName = {entity.ToLower()}.Name.DisplayName,
            Email = {entity.ToLower()}.Email.Value,
            DateProperty = {entity.ToLower()}.DateProperty,
            EnumProperty = {entity.ToLower()}.EnumProperty.Name,
            IsActive = {entity.ToLower()}.IsActive,
            CreatedAt = {entity.ToLower()}.CreatedAt,
            UpdatedAt = {entity.ToLower()}.UpdatedAt
        };
    }
}
```

**DTO Pattern:**
```csharp
public record {Entity}Dto
{
    public Guid Id { get; init; }
    public string Property1 { get; init; } = string.Empty;
    public string Property2 { get; init; } = string.Empty;
    public string? OptionalProperty { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public AddressDto? Address { get; init; }
    public DateTime DateProperty { get; init; }
    public int CalculatedProperty { get; init; }
    public string EnumProperty { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country);
```

#### 2. **Pipeline Behaviors**

**Custom Pipeline Behavior:**
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

**Pipeline Behavior Registration:**
```csharp
// Order matters for pipeline behaviors
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
```

#### 3. **Validation Framework**

**Request Validator:**
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

        return result;
    }
}
```

#### 4. **Caching Implementation**

**Cacheable Query:**
```csharp
public record GetOrderQuery(OrderId OrderId) : QueryBase<OrderDto>, ICacheableQuery
{
    public ICacheKey CacheKey => new OrderCacheKey(OrderId);
    public bool BypassCache { get; init; } = false;
}

// Cache key implementation
public class OrderCacheKey : CacheKey
{
    public OrderCacheKey(OrderId orderId) : base($"Order_{orderId}")
    {
        Policy = new CachePolicy
        {
            AbsoluteExpiration = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(1),
            Priority = CachePriority.High
        };
        
        Tags = new[] { "Orders", $"Customer_{orderId}" };
    }
}
```

#### 5. **Inbox/Outbox Pattern**

**Outbox Pattern Implementation:**
```csharp
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
```

#### 6. **Saga Pattern**

**Saga Definition:**
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
}
```

### Application Layer Service Registration

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
        services.AddHostedService<InboxBackgroundService>();
        services.AddHostedService<OutboxBackgroundService>();

        // Security
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPermissionService, PermissionService>();

        // Register all handlers automatically
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
```

---

## 🏭 Infrastructure Layer (BuildingBlocks.Infrastructure)

### Purpose & Responsibilities

The Infrastructure layer provides concrete implementations for all application layer abstractions, including data access, caching, messaging, authentication, and external service integrations.

### Core Components

#### 1. **Data Access & Entity Framework**

**DbContext with Interceptors:**
```csharp
public class ApplicationDbContext : DbContextBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDomainEventDispatcher eventDispatcher) 
        : base(options)
    {
        _currentUserService = currentUserService;
        _eventDispatcher = eventDispatcher;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Apply strongly typed ID converters
        modelBuilder.ApplyStronglyTypedIdConverters();
        
        // Configure audit fields globally
        modelBuilder.ConfigureAuditFields();
        
        // Configure soft delete globally
        modelBuilder.ConfigureSoftDelete();
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add interceptors for cross-cutting concerns
        optionsBuilder.AddInterceptors(
            new AuditInterceptor(_currentUserService),
            new DomainEventInterceptor(_eventDispatcher),
            new SoftDeleteInterceptor());
            
        base.OnConfiguring(optionsBuilder);
    }
}
```

**Repository Implementation:**
```csharp
public class ProductRepository : Repository<Product, ProductId>, IProductRepository
{
    private readonly ICacheService _cache;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(
        ApplicationDbContext context,
        ICacheService cache,
        ILogger<ProductRepository> logger) 
        : base(context)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Product?> GetByCodeAsync(ProductCode code, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyGenerator.Generate<Product>("code", code.Value);
        var cachedProduct = await _cache.GetAsync<Product>(cacheKey, cancellationToken);
        
        if (cachedProduct != null)
        {
            return cachedProduct;
        }

        var product = await Context.Products
            .Where(p => p.Code.Value == code.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (product != null)
        {
            var cachePolicy = new CachePolicy
            {
                AbsoluteExpiration = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            };
            await _cache.SetAsync(cacheKey, product, cachePolicy, cancellationToken);
        }

        return product;
    }
}
```

#### 2. **Caching Infrastructure**

**Multi-Tier Cache Service:**
```csharp
public class MultiTierCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ICacheKeyGenerator _keyGenerator;
    private readonly ILogger<MultiTierCacheService> _logger;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var fullKey = _keyGenerator.GenerateKey<T>(key);
        
        // Try memory cache first (L1)
        if (_memoryCache.TryGetValue(fullKey, out T? cachedValue))
        {
            _logger.LogDebug("Cache HIT (Memory): {Key}", fullKey);
            return cachedValue;
        }

        // Try distributed cache (L2)
        var distributedValue = await _distributedCache.GetStringAsync(fullKey, cancellationToken);
        if (!string.IsNullOrEmpty(distributedValue))
        {
            var deserializedValue = JsonSerializer.Deserialize<T>(distributedValue);
            
            // Populate memory cache
            var memoryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2),
                Priority = CacheItemPriority.Normal
            };
            _memoryCache.Set(fullKey, deserializedValue, memoryOptions);
            
            _logger.LogDebug("Cache HIT (Distributed): {Key}", fullKey);
            return deserializedValue;
        }

        return null;
    }

    public async Task SetAsync<T>(string key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        var fullKey = _keyGenerator.GenerateKey<T>(key);
        var effectivePolicy = policy ?? _config.DefaultPolicy;
        
        // Set in both memory and distributed cache
        // Implementation details...
    }
}
```

#### 3. **Authentication & Security**

**JWT Token Service:**
```csharp
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfiguration _config;
    private readonly ILogger<JwtTokenService> _logger;

    public string GenerateAccessToken(
        string userId,
        string username,
        IEnumerable<string> roles,
        IDictionary<string, string>? additionalClaims = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config.SecretKey);
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add roles and additional claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddMinutes(_config.AccessTokenExpiryMinutes),
            Issuer = _config.Issuer,
            Audience = _config.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
```

#### 4. **Messaging Infrastructure**

**Event Bus Implementation:**
```csharp
public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventPublisher _publisher;
    private readonly ILogger<EventBus> _logger;

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent
    {
        try
        {
            // Publish to external message bus
            await _publisher.PublishAsync(@event, cancellationToken);
            
            // Handle locally if configured
            if (_config.HandleLocalEvents)
            {
                await HandleLocalEventAsync(@event, cancellationToken);
            }
            
            _logger.LogInformation("Event published: {EventType} - {EventId}", 
                typeof(T).Name, @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event: {EventType} - {EventId}", 
                typeof(T).Name, @event.Id);
            throw;
        }
    }

    private async Task HandleLocalEventAsync<T>(T @event, CancellationToken cancellationToken) where T : class, IEvent
    {
        var handlers = _serviceProvider.GetServices<IEventHandler<T>>();
        var tasks = handlers.Select(handler => HandleEventSafelyAsync(handler, @event, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
```

#### 5. **Background Services**

**Outbox Processor:**
```csharp
public class OutboxBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxBackgroundService> _logger;
    private readonly OutboxSettings _settings;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxProcessor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                
                await outboxProcessor.ProcessPendingMessagesAsync(stoppingToken);
                
                await Task.Delay(_settings.ProcessingInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
```

### Infrastructure Service Registration

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Data Layer
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        
        // Caching
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));
        services.AddScoped<ICacheService, MultiTierCacheService>();
        
        // Authentication
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        
        // Messaging
        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IMessageBus, MessageBus>();
        
        // Background Services
        services.AddHostedService<OutboxBackgroundService>();
        services.AddHostedService<InboxBackgroundService>();
        
        // Security
        services.AddScoped<IEncryptionService, AesEncryptionService>();
        services.AddScoped<IHashingService, Pbkdf2HashingService>();
        
        return services;
    }
}
```

---

## 🎨 API Layer (BuildingBlocks.API)

### Purpose & Responsibilities

The API layer provides the presentation interface using ASP.NET Core Minimal APIs, handling HTTP concerns, authentication, validation, documentation, and response formatting.

### Core Components

#### 1. **Minimal API Endpoints**

**Basic CRUD Endpoints:**
```csharp
public class UserEndpoints : EndpointBase
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/users")
            .WithTags("Users")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .Produces<ApiResponse<UserResponse>>(200)
            .Produces<ApiResponse>(404)
            .RequireAuthorization("UserOrAdmin");

        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create new user")
            .Produces<ApiResponse<UserResponse>>(201)
            .Produces<ValidationErrorResponse>(400)
            .RequireAuthorization("AdminOnly");
    }

    private static async Task<IResult> GetUserById(
        Guid id,
        IMediator mediator,
        HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        try
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await mediator.Send(query);
            
            if (result == null)
            {
                return NotFound($"User with ID {id} not found", correlationId);
            }
            
            return Success(result, "User retrieved successfully", correlationId);
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving user: {ex.Message}", correlationId);
        }
    }
}
```

**Advanced CRUD with Base Class:**
```csharp
public class ProductEndpoints : CrudEndpoints<Product, ProductResponse, ProductId>
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var productEndpoints = new ProductEndpoints();
        
        var group = endpoints.MapGroup("/api/v1/products")
            .WithTags("Products")
            .WithOpenApi();
        
        // Register CRUD endpoints with custom behavior
        productEndpoints.RegisterEndpoints(group, options =>
        {
            options.EnableGet = true;
            options.EnableGetAll = true;
            options.EnableCreate = true;
            options.EnableUpdate = true;
            options.EnableDelete = true;
            options.RequireAuthentication = true;
            options.CreateRequiresRole = "Admin";
            options.UpdateRequiresRole = "Admin";
            options.DeleteRequiresRole = "Admin";
        });
        
        // Add custom business endpoints
        group.MapPost("/{id:guid}/activate", ActivateProduct)
            .RequireAuthorization("AdminOnly");
    }
    
    // Override methods for custom behavior
    protected override async Task<ProductResponse?> HandleGetAsync(
        ProductId id, 
        IMediator mediator, 
        HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        using var activity = ActivitySource.StartActivity("GetProduct");
        activity?.SetTag("product.id", id.Value.ToString());
        
        var query = new GetProductByIdQuery { Id = id };
        return await mediator.Send(query);
    }
}
```

#### 2. **Authentication & Authorization**

**JWT Setup:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Authentication:Jwt");
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            ClockSkew = TimeSpan.FromMinutes(5)
        };
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT authentication failed: {Error}", 
                    context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

// Authorization policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin")
              .RequireAuthenticatedUser())
    .AddPolicy("UserOrAdmin", policy =>
        policy.RequireRole("User", "Admin")
              .RequireAuthenticatedUser());
```

#### 3. **Validation & Error Handling**

**FluentValidation Integration:**
```csharp
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");
    }
}

// Global exception handling
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.GetCorrelationId();
        
        var (statusCode, message) = exception switch
        {
            ValidationException => (400, "Validation failed"),
            UnauthorizedAccessException => (401, "Unauthorized access"),
            KeyNotFoundException => (404, "Resource not found"),
            _ => (500, "An error occurred")
        };
        
        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}
```

#### 4. **Response Framework**

**API Response Builder:**
```csharp
public static class ResponseExamples
{
    public static IResult BuildSuccessResponse<T>(T data, string message = "Success")
    {
        var response = ApiResponseBuilder<T>
            .Success()
            .WithData(data)
            .WithMessage(message)
            .WithCorrelationId(Guid.NewGuid().ToString())
            .WithTimestamp(DateTime.UtcNow)
            .WithMetadata("version", "1.0")
            .Build();
            
        return Results.Ok(response);
    }
    
    public static IResult BuildPagedResponse<T>(
        IEnumerable<T> items, 
        int page, 
        int pageSize, 
        int totalCount)
    {
        var pagedResponse = PagedResponse<T>.Create(
            items, page, pageSize, totalCount);
            
        var response = ApiResponseBuilder<PagedResponse<T>>
            .Success()
            .WithData(pagedResponse)
            .WithMessage($"Retrieved {items.Count()} items")
            .Build();
            
        return Results.Ok(response);
    }
}
```

#### 5. **Rate Limiting**

**Rate Limiting Configuration:**
```csharp
builder.Services.AddRateLimiter(options =>
{
    // Global rate limit
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: "global",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10000,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // Per-user rate limit
    options.AddPolicy("UserPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User?.Identity?.Name ?? "anonymous",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 1000,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// Apply to endpoints
group.MapPost("/", CreateUser)
    .RequireRateLimiting("UserPolicy");
```

#### 6. **OpenAPI Documentation**

**Enhanced Documentation:**
```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<AuthenticationDocumentTransformer>();
});

builder.Services.AddScalar(options =>
{
    options.Title = "User Management API";
    options.Theme = ScalarTheme.Purple;
    options.ShowSidebar = true;
});

// Endpoint documentation with examples
group.MapPost("/", CreateUser)
    .WithName("CreateUser")
    .WithSummary("Create a new user account")
    .WithDescription("Creates a new user with the provided information")
    .WithOpenApi(operation =>
    {
        operation.RequestBody.Content["application/json"].Examples = new Dictionary<string, OpenApiExample>
        {
            ["basic"] = new()
            {
                Summary = "Basic user creation",
                Value = new CreateUserRequest("John", "Doe", "john@example.com")
            }
        };
        
        return operation;
    });
```

### API Layer Service Registration

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Core API services
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services.AddScalar();
        
        // Authentication
        services.AddAuthentication().AddJwtBearer(configuration);
        services.AddAuthorizationPolicies();
        
        // Validation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Rate limiting
        services.AddRateLimiter(configuration);
        
        // CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });
        
        // Health checks
        services.AddHealthChecks();
        
        return services;
    }

    public static WebApplication UseApiLayer(this WebApplication app)
    {
        // Security headers
        app.UseSecurityHeaders();
        
        // CORS
        app.UseCors();
        
        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Rate limiting
        app.UseRateLimiter();
        
        // Error handling
        app.UseMiddleware<GlobalExceptionMiddleware>();
        
        // Request logging
        app.UseRequestLogging();
        
        // Documentation
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
        
        // Health checks
        app.MapHealthChecks("/health");
        
        return app;
    }
}
```

---

## 🔄 Integration Patterns

### 1. **Complete Service Setup**

```csharp
// Program.cs - Full integration
using BuildingBlocks.Domain.Extensions;
using BuildingBlocks.Application.Extensions;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register all layers
builder.Services.AddDomainServices();
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiLayer(builder.Configuration);

// Add service-specific services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Configure middleware pipeline
app.UseApiLayer();

// Map endpoints
app.MapUserEndpoints();
app.MapOrderEndpoints();
app.MapProductEndpoints();

app.Run();
```

### 2. **Configuration Management**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourApp;Trusted_Connection=true;",
    "Redis": "localhost:6379"
  },
  "Authentication": {
    "Jwt": {
      "SecretKey": "your-secret-key-must-be-at-least-32-characters-long",
      "Issuer": "YourApp",
      "Audience": "YourApp-API",
      "ExpiryMinutes": "60"
    }
  },
  "Caching": {
    "DefaultPolicy": {
      "AbsoluteExpirationInMinutes": 30,
      "SlidingExpirationInMinutes": 5
    }
  },
  "RateLimiting": {
    "GlobalPolicy": {
      "PermitLimit": 10000,
      "Window": "00:01:00"
    }
  },
  "Api": {
    "Title": "Your API",
    "Version": "v1.0",
    "Description": "Your API description"
  }
}
```

### 3. **Testing Integration**

```csharp
// Integration test setup
public class WebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> 
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace database with in-memory
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
            
            // Replace external services with mocks
            services.AddScoped<IEmailService, MockEmailService>();
        });
    }
}

// Test example
public class UserEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateUserRequest("John", "Doe", "john@example.com");
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();
        content!.Success.Should().BeTrue();
    }
}
```

---

## 📋 Best Practices & Guidelines

### 1. **Domain Layer Best Practices**

- **Keep the domain pure**: No dependencies on external frameworks
- **Use strongly typed IDs**: Prevent ID confusion and type safety
- **Implement proper encapsulation**: Private setters, factory methods
- **Define clear business rules**: Explicit validation and invariants
- **Use domain events**: For cross-aggregate communication
- **Apply DDD patterns**: Entities, Value Objects, Aggregates, Repositories

### 2. **Application Layer Best Practices**

- **Implement CQRS**: Separate commands and queries
- **Use pipeline behaviors**: For cross-cutting concerns
- **Validate inputs**: Use FluentValidation for complex rules
- **Handle transactions**: Use Unit of Work pattern
- **Cache appropriately**: Cache queries, not commands
- **Implement idempotency**: For critical operations

### 3. **Infrastructure Layer Best Practices**

- **Use configuration**: Externalize all settings
- **Implement health checks**: Monitor dependencies
- **Use connection pooling**: For databases and external services
- **Implement retry policies**: For transient failures
- **Log appropriately**: Structured logging with correlation IDs
- **Monitor performance**: Track metrics and slow operations

### 4. **API Layer Best Practices**

- **Use appropriate HTTP status codes**: Follow REST conventions
- **Implement proper error handling**: Consistent error responses
- **Add comprehensive documentation**: OpenAPI with examples
- **Implement rate limiting**: Protect against abuse
- **Use correlation IDs**: For request tracing
- **Validate all inputs**: Security and data integrity

### 5. **Security Best Practices**

- **Implement authentication**: Use JWT or API keys
- **Apply authorization**: Role-based and policy-based
- **Validate all inputs**: Prevent injection attacks
- **Use HTTPS**: Encrypt data in transit
- **Implement security headers**: HSTS, CSP, etc.
- **Audit sensitive operations**: Track user actions

### 6. **Performance Best Practices**

- **Use async/await**: For I/O operations
- **Implement caching**: Multiple tiers and strategies
- **Use pagination**: For large datasets
- **Optimize database queries**: Use specifications and projections
- **Monitor performance**: Track response times and bottlenecks
- **Use background processing**: For long-running operations

---

## 🔧 Troubleshooting & Common Issues

### 1. **Entity Framework Issues**

**Problem**: Domain events not firing
```csharp
// Solution: Ensure DomainEventInterceptor is registered
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.AddInterceptors(new DomainEventInterceptor(_eventDispatcher));
}
```

**Problem**: Strongly typed IDs not working with EF Core
```csharp
// Solution: Configure value converters
modelBuilder.Entity<Product>()
    .Property(e => e.Id)
    .HasConversion(
        id => id.Value,
        value => new ProductId(value));
```

### 2. **Caching Issues**

**Problem**: Cache not invalidating
```csharp
// Solution: Use proper cache tags and invalidation
public async Task InvalidateProductCacheAsync(ProductId productId)
{
    await _cache.RemoveByTagAsync($"product-{productId}", CancellationToken.None);
}
```

### 3. **Authentication Issues**

**Problem**: JWT tokens not validating
```csharp
// Solution: Check token validation parameters
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.FromMinutes(5) // Allow for clock drift
};
```

### 4. **Performance Issues**

**Problem**: Slow API responses
```csharp
// Solution: Add performance monitoring
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > 1000)
        {
            _logger.LogWarning("Slow request: {RequestType} took {Duration}ms", 
                typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);
        }
        
        return response;
    }
}
```

### 5. **Validation Issues**

**Problem**: Validation not running
```csharp
// Solution: Ensure ValidationBehavior is registered in correct order
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // Before other behaviors
```

### 6. **Configuration Issues**

**Problem**: Settings not loading
```csharp
// Solution: Check configuration binding
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Authentication:Jwt"));

// Validate configuration
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Authentication:Jwt"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

---

## 📚 Additional Resources

### Documentation
- [Clean Architecture Principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design Reference](https://www.domainlanguage.com/ddd/reference/)
- [CQRS Pattern Guide](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [ASP.NET Core Minimal APIs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)

### Examples & Templates
- [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers)
- [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [Microservices Template](https://github.com/dotnet-architecture/eShopOnContainers)

---

*This prompt library serves as a comprehensive guide for implementing enterprise-grade .NET applications using the BuildingBlocks library suite. Each section provides practical examples, best practices, and troubleshooting guidance to help developers build maintainable, scalable, and robust applications following Clean Architecture principles.* 