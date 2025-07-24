# BuildingBlocks.Domain - Library Structure

## Directory Structure

```
BuildingBlocks.Domain/
├── BusinessRules/                          # Business rule validation framework
│   ├── BusinessRuleBase.cs                 # Abstract base class with CheckRule method
│   ├── CompositeBusinessRule.cs            # Combines multiple business rules
│   └── IBusinessRule.cs                    # Business rule interface
├── Common/                                 # Common value objects and types
│   ├── Address.cs                          # Postal address value object
│   ├── DateRange.cs                        # Date range value object (placeholder)
│   ├── Email.cs                            # Email value object with validation
│   ├── Money.cs                            # Currency-aware monetary value object
│   └── PhoneNumber.cs                      # Phone number value object with validation
├── DomainEvents/                           # Domain event pattern implementation
│   ├── DomainEventBase.cs                  # Base implementation with auto-generated metadata
│   ├── DomainEventDispatcher.cs            # Service provider-based event dispatcher
│   ├── IDomainEvent.cs                     # Domain event interface
│   ├── IDomainEventDispatcher.cs           # Event dispatcher interface
│   └── IDomainEventHandler.cs              # Generic event handler interface
├── Entities/                               # Entity base classes and interfaces
│   ├── AggregateRoot.cs                    # Aggregate root with domain event management
│   ├── Entity.cs                           # Base entity class with strongly-typed IDs
│   ├── IAuditableEntity.cs                 # Interface for audit trail support
│   └── ISoftDeletable.cs                   # Interface for soft delete functionality
├── Exceptions/                             # Domain-specific exception types
│   ├── AggregateNotFoundException.cs       # Exception for missing aggregates
│   ├── BusinessRuleValidationException.cs  # Exception for business rule violations
│   ├── ConcurrencyException.cs             # Exception for concurrency conflicts
│   ├── DomainException.cs                  # Base domain exception class
│   └── InvalidOperationDomainException.cs  # Exception for invalid domain operations
├── Extensions/                             # Extension methods for domain objects
│   └── DomainExtensions.cs                 # Business rule validation and domain helpers
├── Guards/                                 # Input validation and precondition checking
│   └── Guard.cs                            # Guard clause implementations (placeholder)
├── Repository/                             # Repository pattern interfaces
│   ├── IReadOnlyRepository.cs              # Read-only repository interface
│   ├── IRepository.cs                      # Full repository interface
│   ├── IUnitOfWork.cs                      # Unit of work pattern interface
│   └── RepositoryBase.cs                   # Abstract repository base class
├── Specifications/                         # Specification pattern for complex queries
│   ├── AndSpecification.cs                 # Logical AND specification combiner
│   ├── ISpecification.cs                   # Specification pattern interface
│   ├── NotSpecification.cs                 # Logical NOT specification negator
│   ├── OrSpecification.cs                  # Logical OR specification combiner
│   ├── Specification.cs                    # Abstract specification base class
│   └── SpecificationEvaluator.cs           # Converts specifications to queryable
├── StronglyTypedIds/                       # Type-safe entity identifiers
│   ├── GuidId.cs                           # GUID-based strongly typed ID
│   ├── IntId.cs                            # Integer-based strongly typed ID
│   ├── IStronglyTypedId.cs                 # Base interface for strongly typed IDs
│   ├── Json/                               # JSON serialization support
│   │   ├── StronglyTypedIdJsonConverter.cs # Generic JSON converter
│   │   └── StronglyTypedIdJsonConverterFactory.cs # Converter factory
│   ├── LongId.cs                           # Long integer-based strongly typed ID
│   ├── StringId.cs                         # String-based strongly typed ID
│   └── StronglyTypedId.cs                  # Abstract base for strongly typed IDs
├── ValueObjects/                           # Value object base classes
│   ├── Enumeration.cs                      # Rich enum pattern implementation
│   ├── SingleValueObject.cs                # Base for single-value value objects
│   └── ValueObject.cs                      # Abstract base for complex value objects
├── BuildingBlocks.Domain.csproj            # Project file
├── BuildingBlocks.Domain.md                # This structure documentation file
└── README.md                               # Comprehensive usage documentation
```

## Component Categories

### 🏗️ Core Domain Building Blocks
- **Entities/**: Base classes for domain entities and aggregate roots
- **ValueObjects/**: Immutable value objects with structural equality
- **StronglyTypedIds/**: Type-safe entity identifiers with JSON support

### 📋 Business Logic Framework  
- **BusinessRules/**: Encapsulated business rule validation
- **Specifications/**: Query object pattern with logical operators
- **Extensions/**: Helper methods for domain operations

### 🗂️ Data Access Abstractions
- **Repository/**: Generic repository interfaces and base implementations
- **Specifications/**: Complex query building and evaluation

### 📦 Common Domain Types
- **Common/**: Reusable value objects (Money, Email, Address, etc.)
- **Exceptions/**: Specialized domain exception types
- **Guards/**: Input validation and precondition checking

### 🔧 Infrastructure Support
- **DomainEvents/**: Event-driven domain communication
- **StronglyTypedIds/Json/**: JSON serialization for typed IDs

## Key Features by Directory

### Entities/
- **Entity.cs**: Generic base entity with strongly-typed ID and proper equality
- **AggregateRoot.cs**: Domain event management for aggregate roots  
- **IAuditableEntity.cs**: Created/Modified audit trail interface
- **ISoftDeletable.cs**: Soft delete functionality with restore capability

### ValueObjects/
- **ValueObject.cs**: Structural equality through GetEqualityComponents()
- **SingleValueObject.cs**: Wrapper for single-value value objects with implicit conversion
- **Enumeration.cs**: Rich enum pattern with discovery methods

### StronglyTypedIds/
- **Type Safety**: Prevents primitive obsession with typed identifiers
- **JSON Support**: Automatic serialization/deserialization
- **Validation**: Built-in validation for each ID type (empty GUID, positive numbers, etc.)
- **ORM Support**: Compatible with Entity Framework Core value converters

### DomainEvents/
- **Event Publishing**: Automatic event dispatching through service provider
- **Handler Discovery**: Dynamic handler resolution and invocation
- **Metadata**: Auto-generated event IDs and timestamps

### BusinessRules/
- **Single Rules**: Simple business rule validation with IsBroken() method
- **Composite Rules**: Combine multiple rules with logical operations
- **Extension Support**: this.CheckRule() extension methods for any object

### Specifications/
- **Query Building**: Expression-based query criteria
- **Logical Operations**: AND, OR, NOT combinations
- **EF Core Integration**: Automatic include and ordering support
- **Pagination**: Built-in paging capabilities

### Repository/
- **Async Operations**: All methods support async/await patterns
- **Generic Interface**: Type-safe repository operations
- **Specification Support**: Query by specification pattern
- **Unit of Work**: Transaction coordination and management

### Common/
- **Money**: Currency-aware calculations with arithmetic operations
- **Email**: Regex validation with implicit conversion
- **Address**: Structured postal addresses with full formatting
- **PhoneNumber**: International phone number validation

This structure provides a comprehensive foundation for implementing Domain-Driven Design patterns while maintaining clean separation of concerns and strong typing throughout the domain layer.