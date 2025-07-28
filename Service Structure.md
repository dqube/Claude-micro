# Service Structure Template

This document defines the comprehensive generic structure for microservices in the clean architecture solution. Each service follows the same organizational pattern based on Clean Architecture principles with detailed folder organization.

## Generic Service Structure

```
{ServiceName}Service/
├── 📁 API/                             # API Layer (Presentation)
│   ├── {ServiceName}.API.csproj        # API project file
│   ├── Program.cs                      # Application entry point and service configuration
│   ├── 📁 Endpoints/                   # API endpoints using Minimal APIs
│   │   ├── {Entity}Endpoints.cs        # Primary entity endpoints (CRUD operations)
│   │   ├── {SecondaryEntity}Endpoints.cs # Secondary entity endpoints
│   │   └── {RelatedEntity}Endpoints.cs # Related entity endpoints
│   ├── 📁 Validators/                  # FluentValidation request validators
│   │   ├── Create{Entity}RequestValidator.cs # Create operation validator
│   │   ├── Update{Entity}RequestValidator.cs # Update operation validator
│   │   ├── Delete{Entity}RequestValidator.cs # Delete operation validator
│   │   ├── Get{Entity}sRequestValidator.cs   # List/search query validator
│   │   ├── Get{Entity}ByIdRequestValidator.cs # Get by ID validator
│   │   └── {BusinessOperation}RequestValidator.cs # Business operation validators
│   ├── 📁 Middleware/                  # Custom middleware (if needed)
│   │   ├── {Service}AuthorizationMiddleware.cs # Service-specific authorization
│   │   └── {Service}ValidationMiddleware.cs    # Service-specific validation
│   ├── 📁 Filters/                     # Action filters and attributes
│   │   ├── {Service}AuthorizeAttribute.cs      # Custom authorization
│   │   └── {Service}ValidationFilter.cs        # Custom validation filter
│   ├── 📁 Extensions/                  # Service registration extensions
│   │   ├── ServiceCollectionExtensions.cs      # DI container setup
│   │   └── WebApplicationExtensions.cs         # Pipeline configuration
│   ├── 📁 Properties/                  # Project properties and launch settings
│   │   └── launchSettings.json         # Development launch configuration
│   ├── 📁 Configuration/               # Configuration classes
│   │   ├── ApiSettings.cs              # API-specific settings
│   │   └── {Service}ApiOptions.cs      # Service configuration options
│   ├── appsettings.json                # Production application configuration
│   ├── appsettings.Development.json    # Development environment settings
│   ├── appsettings.Staging.json        # Staging environment settings (optional)
│   └── appsettings.Production.json     # Production environment settings (optional)
├── 📁 Application/                     # Application Layer (Use Cases)
│   ├── {ServiceName}.Application.csproj # Application layer project file
│   ├── DependencyInjection.cs         # Application services registration
│   ├── 📁 Commands/                    # Command handlers (Write operations)
│   │   ├── 📁 Create{Entity}/          # Create entity command group
│   │   │   ├── Create{Entity}Command.cs    # Create command definition
│   │   │   ├── Create{Entity}CommandHandler.cs # Create command handler
│   │   │   └── Create{Entity}CommandValidator.cs # Command validation
│   │   ├── 📁 Update{Entity}/          # Update entity command group
│   │   │   ├── Update{Entity}Command.cs    # Update command definition
│   │   │   ├── Update{Entity}CommandHandler.cs # Update command handler
│   │   │   └── Update{Entity}CommandValidator.cs # Command validation
│   │   ├── 📁 Delete{Entity}/          # Delete entity command group
│   │   │   ├── Delete{Entity}Command.cs    # Delete command definition
│   │   │   ├── Delete{Entity}CommandHandler.cs # Delete command handler
│   │   │   └── Delete{Entity}CommandValidator.cs # Command validation
│   │   └── 📁 {BusinessOperation}/     # Business operation commands
│   │       ├── {BusinessOperation}Command.cs       # Business command
│   │       ├── {BusinessOperation}CommandHandler.cs # Business handler
│   │       └── {BusinessOperation}CommandValidator.cs # Business validation
│   ├── 📁 Queries/                     # Query handlers (Read operations)
│   │   ├── 📁 Get{Entity}ById/         # Get entity by ID query group
│   │   │   ├── Get{Entity}ByIdQuery.cs     # Query definition
│   │   │   ├── Get{Entity}ByIdQueryHandler.cs # Query handler
│   │   │   └── Get{Entity}ByIdQueryValidator.cs # Query validation
│   │   ├── 📁 Get{Entity}s/            # Get entities collection query group
│   │   │   ├── Get{Entity}sQuery.cs        # Collection query definition
│   │   │   ├── Get{Entity}sQueryHandler.cs # Collection query handler
│   │   │   └── Get{Entity}sQueryValidator.cs # Collection query validation
│   │   ├── 📁 Search{Entity}s/         # Search entities query group
│   │   │   ├── Search{Entity}sQuery.cs     # Search query definition
│   │   │   ├── Search{Entity}sQueryHandler.cs # Search query handler
│   │   │   └── Search{Entity}sQueryValidator.cs # Search query validation
│   │   └── 📁 {BusinessQuery}/         # Business-specific queries
│   │       ├── {BusinessQuery}Query.cs     # Business query definition
│   │       ├── {BusinessQuery}QueryHandler.cs # Business query handler
│   │       └── {BusinessQuery}QueryValidator.cs # Business query validation
│   ├── 📁 DTOs/                        # Data Transfer Objects
│   │   ├── 📁 Requests/                # Request DTOs
│   │   │   ├── Create{Entity}Request.cs    # Create request DTO
│   │   │   ├── Update{Entity}Request.cs    # Update request DTO
│   │   │   ├── Get{Entity}sRequest.cs      # List request DTO
│   │   │   └── {BusinessOperation}Request.cs # Business operation request
│   │   ├── 📁 Responses/               # Response DTOs
│   │   │   ├── {Entity}Response.cs         # Entity response DTO
│   │   │   ├── {Entity}ListResponse.cs     # Entity list response DTO
│   │   │   ├── {Entity}DetailResponse.cs   # Entity detail response DTO
│   │   │   └── {BusinessOperation}Response.cs # Business operation response
│   │   └── 📁 Common/                  # Common DTOs
│   │       ├── PagedResponse.cs            # Paged response wrapper
│   │       ├── ApiResponse.cs              # Standard API response
│   │       └── ErrorResponse.cs            # Error response DTO
│   ├── 📁 EventHandlers/               # Domain event handlers
│   │   ├── DomainEventWrapper.cs       # MediatR domain event wrapper
│   │   ├── 📁 {Entity}Events/          # Entity-specific event handlers
│   │   │   ├── {Entity}CreatedEventHandler.cs     # Created event handler
│   │   │   ├── {Entity}UpdatedEventHandler.cs     # Updated event handler
│   │   │   ├── {Entity}DeletedEventHandler.cs     # Deleted event handler
│   │   │   ├── {Entity}ActivatedEventHandler.cs   # Activated event handler
│   │   │   └── {Entity}DeactivatedEventHandler.cs # Deactivated event handler
│   │   └── 📁 IntegrationEvents/       # Integration event handlers
│   │       ├── {External}EventHandler.cs          # External system events
│   │       └── {CrossService}EventHandler.cs      # Cross-service events
│   ├── 📁 Services/                    # Application services
│   │   ├── I{Entity}ApplicationService.cs # Application service interface
│   │   ├── {Entity}ApplicationService.cs  # Application service implementation
│   │   └── I{Business}Service.cs           # Business service interface
│   ├── 📁 Behaviors/                   # MediatR pipeline behaviors
│   │   ├── ValidationBehavior.cs       # Request validation behavior
│   │   ├── LoggingBehavior.cs          # Request/response logging
│   │   ├── PerformanceBehavior.cs      # Performance monitoring
│   │   └── TransactionBehavior.cs      # Database transaction handling
│   ├── 📁 Mappings/                    # Object mapping profiles
│   │   ├── {Entity}MappingProfile.cs   # Entity mapping configuration
│   │   └── ApplicationMappingProfile.cs # General application mappings
│   ├── 📁 Specifications/              # Business rule specifications
│   │   ├── {Entity}Specifications.cs   # Entity-specific specifications
│   │   └── {Business}Specifications.cs # Business rule specifications
│   └── 📁 Exceptions/                  # Application-specific exceptions
│       ├── {Entity}ApplicationException.cs # Entity application exceptions
│       └── {Business}Exception.cs           # Business logic exceptions
├── 📁 Domain/                          # Domain Layer (Business Logic)
│   ├── {ServiceName}.Domain.csproj     # Domain layer project file
│   ├── DependencyInjection.cs         # Domain services registration
│   ├── 📁 Aggregates/                  # Aggregate roots and entities
│   │   ├── 📁 {Entity}Aggregate/       # Main entity aggregate
│   │   │   ├── {Entity}.cs             # Aggregate root entity
│   │   │   ├── {SubEntity}.cs          # Child entities within aggregate
│   │   │   └── {Entity}Factory.cs      # Entity factory for complex creation
│   │   └── 📁 {SecondaryEntity}Aggregate/ # Secondary aggregate
│   │       ├── {SecondaryEntity}.cs    # Secondary aggregate root
│   │       └── {RelatedEntity}.cs      # Related entities
│   ├── 📁 ValueObjects/                # Domain value objects
│   │   ├── 📁 Identifiers/             # Strongly typed identifiers
│   │   │   ├── {Entity}Id.cs           # Primary entity identifier
│   │   │   ├── {SecondaryEntity}Id.cs  # Secondary entity identifier
│   │   │   └── {ExternalId}.cs         # External system identifiers
│   │   ├── 📁 Common/                  # Common value objects
│   │   │   ├── Address.cs              # Address value object
│   │   │   ├── Email.cs                # Email value object
│   │   │   ├── PhoneNumber.cs          # Phone number value object
│   │   │   └── Money.cs                # Money value object
│   │   └── 📁 {Business}/              # Business-specific value objects
│   │       ├── {Property}.cs           # Business property value object
│   │       └── {Measurement}.cs        # Measurement value objects
│   ├── 📁 Events/                      # Domain events
│   │   ├── 📁 {Entity}Events/          # Entity-specific events
│   │   │   ├── {Entity}CreatedEvent.cs         # Entity created
│   │   │   ├── {Entity}UpdatedEvent.cs         # Entity updated
│   │   │   ├── {Entity}DeletedEvent.cs         # Entity deleted
│   │   │   ├── {Entity}ActivatedEvent.cs       # Entity activated
│   │   │   ├── {Entity}DeactivatedEvent.cs     # Entity deactivated
│   │   │   └── {Entity}{BusinessAction}Event.cs # Business action events
│   │   └── 📁 IntegrationEvents/       # Cross-boundary events
│   │       ├── {Entity}IntegrationEvent.cs     # Integration events
│   │       └── {Business}IntegrationEvent.cs   # Business integration events
│   ├── 📁 Exceptions/                  # Domain exceptions
│   │   ├── 📁 {Entity}Exceptions/      # Entity-specific exceptions
│   │   │   ├── {Entity}NotFoundException.cs    # Entity not found
│   │   │   ├── {Entity}AlreadyExistsException.cs # Duplicate entity
│   │   │   ├── {Entity}InvalidStateException.cs # Invalid state
│   │   │   └── {Entity}ValidationException.cs   # Entity validation
│   │   ├── 📁 BusinessExceptions/      # Business rule exceptions
│   │   │   ├── {BusinessRule}ViolationException.cs # Business rule violations
│   │   │   └── {Operation}NotAllowedException.cs   # Operation restrictions
│   │   └── 📁 Common/                  # Common domain exceptions
│   │       ├── DomainException.cs      # Base domain exception
│   │       └── ConcurrencyException.cs # Concurrency conflict exception
│   ├── 📁 Repositories/                # Repository contracts
│   │   ├── I{Entity}Repository.cs      # Primary entity repository
│   │   ├── I{SecondaryEntity}Repository.cs # Secondary entity repository
│   │   ├── IUnitOfWork.cs              # Unit of work pattern
│   │   └── IReadOnlyRepository.cs      # Read-only repository base
│   ├── 📁 Services/                    # Domain services
│   │   ├── 📁 Interfaces/              # Domain service interfaces
│   │   │   ├── I{Entity}DomainService.cs   # Entity domain service
│   │   │   └── I{Business}DomainService.cs # Business domain service
│   │   └── 📁 Implementations/         # Domain service implementations
│   │       ├── {Entity}DomainService.cs    # Entity domain service impl
│   │       └── {Business}DomainService.cs  # Business domain service impl
│   ├── 📁 Specifications/              # Domain specifications
│   │   ├── 📁 {Entity}Specifications/  # Entity specifications
│   │   │   ├── {Entity}ActiveSpecification.cs     # Active entity spec
│   │   │   ├── {Entity}ByCriteriaSpecification.cs # Search criteria spec
│   │   │   └── {Entity}ValidationSpecification.cs # Validation spec
│   │   └── 📁 Common/                  # Common specifications
│   │       ├── BaseSpecification.cs    # Base specification class
│   │       └── CompositeSpecification.cs # Composite specifications
│   ├── 📁 Enums/                       # Domain enumerations
│   │   ├── {Entity}Status.cs           # Entity status enumeration
│   │   ├── {Entity}Type.cs             # Entity type enumeration
│   │   └── {Business}Category.cs       # Business category enumeration
│   └── 📁 Constants/                   # Domain constants
│       ├── {Entity}Constants.cs        # Entity-specific constants
│       ├── BusinessRuleConstants.cs    # Business rule constants
│       └── DomainConstants.cs          # General domain constants
├── 📁 Infrastructure/                  # Infrastructure Layer (External Concerns)
│   ├── {ServiceName}.Infrastructure.csproj # Infrastructure project file
│   ├── DependencyInjection.cs         # Infrastructure services registration
│   ├── 📁 Persistence/                 # Database and data persistence
│   │   ├── 📁 Context/                 # Database contexts
│   │   │   ├── {ServiceName}DbContext.cs       # Main database context
│   │   │   ├── {ServiceName}ReadOnlyDbContext.cs # Read-only context
│   │   │   └── I{ServiceName}DbContext.cs      # Context interface
│   │   ├── 📁 Configurations/          # Entity Framework configurations
│   │   │   ├── {Entity}Configuration.cs        # Primary entity config
│   │   │   ├── {SecondaryEntity}Configuration.cs # Secondary entity config
│   │   │   ├── ValueObjectConfiguration.cs     # Value object configs
│   │   │   └── AuditConfiguration.cs           # Audit trail configuration
│   │   ├── 📁 Interceptors/            # EF Core interceptors
│   │   │   ├── AuditInterceptor.cs     # Audit trail interceptor
│   │   │   ├── DomainEventInterceptor.cs # Domain event publishing
│   │   │   └── SoftDeleteInterceptor.cs  # Soft delete implementation
│   │   ├── 📁 Converters/              # Value converters
│   │   │   ├── {ValueObject}Converter.cs # Value object converters
│   │   │   └── StronglyTypedIdConverter.cs # ID converters
│   │   ├── 📁 Seeds/                   # Database seed data
│   │   │   ├── {Entity}Seed.cs         # Entity seed data
│   │   │   └── InitialDataSeed.cs      # Initial database setup
│   │   └── 📁 Extensions/              # Persistence extensions
│   │       ├── ModelBuilderExtensions.cs # EF model configuration
│   │       └── DbContextExtensions.cs    # DbContext utilities
│   ├── 📁 Repositories/                # Repository implementations
│   │   ├── {Entity}Repository.cs       # Primary entity repository
│   │   ├── {SecondaryEntity}Repository.cs # Secondary entity repository
│   │   ├── ReadOnlyRepository.cs       # Read-only repository base
│   │   ├── RepositoryBase.cs           # Base repository implementation
│   │   └── UnitOfWork.cs               # Unit of work implementation
│   ├── 📁 Services/                    # Infrastructure services
│   │   ├── 📁 External/                # External service integrations
│   │   │   ├── {ExternalSystem}Service.cs # External system integration
│   │   │   └── I{ExternalSystem}Service.cs # External service interface
│   │   ├── 📁 Messaging/               # Message bus implementations
│   │   │   ├── {Service}MessagePublisher.cs # Message publishing
│   │   │   ├── {Service}MessageConsumer.cs  # Message consumption
│   │   │   └── MessageMappingProfile.cs     # Message mapping
│   │   ├── 📁 Caching/                 # Caching implementations
│   │   │   ├── {Entity}CacheService.cs # Entity-specific caching
│   │   │   └── CacheKeyGenerator.cs    # Cache key generation
│   │   ├── 📁 Files/                   # File storage services
│   │   │   ├── {Entity}FileService.cs  # Entity file handling
│   │   │   └── FileStorageService.cs   # File storage implementation
│   │   └── 📁 Background/              # Background services
│   │       ├── {Entity}BackgroundService.cs # Entity background processing
│   │       └── OutboxProcessorService.cs    # Outbox pattern processor
│   ├── 📁 Migrations/                  # Database migrations
│   │   ├── {Timestamp}_{Description}.cs        # Migration implementation
│   │   ├── {Timestamp}_{Description}.Designer.cs # Migration designer
│   │   └── {ServiceName}DbContextModelSnapshot.cs # Current model snapshot
│   ├── 📁 Configuration/               # Infrastructure configuration
│   │   ├── DatabaseOptions.cs          # Database connection options
│   │   ├── CacheOptions.cs             # Caching configuration
│   │   ├── MessageBusOptions.cs        # Message bus configuration
│   │   └── ExternalServiceOptions.cs   # External service settings
│   ├── 📁 Logging/                     # Logging implementations
│   │   ├── {Service}Logger.cs          # Service-specific logging
│   │   └── StructuredLoggingExtensions.cs # Structured logging setup
│   ├── 📁 Security/                    # Security implementations
│   │   ├── {Service}AuthorizationHandler.cs # Custom authorization
│   │   ├── EncryptionService.cs        # Data encryption
│   │   └── TokenValidationService.cs   # Token validation
│   └── 📁 Extensions/                  # Infrastructure extensions
│       ├── ServiceCollectionExtensions.cs # DI registration
│       ├── ApplicationBuilderExtensions.cs # Pipeline setup
│       ├── HealthCheckExtensions.cs    # Health check registration
│       └── DatabaseExtensions.cs       # Database setup extensions
├── 📁 {ServiceName}.Tests/             # Test Projects (Optional)
│   ├── {ServiceName}.Tests.csproj      # Test project file
│   ├── 📁 Unit/                        # Unit tests
│   │   ├── 📁 Domain/                  # Domain layer tests
│   │   │   ├── 📁 Entities/            # Entity tests
│   │   │   │   ├── {Entity}Tests.cs    # Entity behavior tests
│   │   │   │   └── {Entity}FactoryTests.cs # Entity factory tests
│   │   │   ├── 📁 ValueObjects/        # Value object tests
│   │   │   │   ├── {ValueObject}Tests.cs # Value object tests
│   │   │   │   └── {Identifier}Tests.cs  # Identifier tests
│   │   │   ├── 📁 Services/            # Domain service tests
│   │   │   │   └── {DomainService}Tests.cs # Domain service tests
│   │   │   └── 📁 Specifications/      # Specification tests
│   │   │       └── {Specification}Tests.cs # Specification tests
│   │   ├── 📁 Application/             # Application layer tests
│   │   │   ├── 📁 Commands/            # Command handler tests
│   │   │   │   ├── {Command}HandlerTests.cs # Command handler tests
│   │   │   │   └── {Command}ValidatorTests.cs # Command validator tests
│   │   │   ├── 📁 Queries/             # Query handler tests
│   │   │   │   ├── {Query}HandlerTests.cs # Query handler tests
│   │   │   │   └── {Query}ValidatorTests.cs # Query validator tests
│   │   │   ├── 📁 EventHandlers/       # Event handler tests
│   │   │   │   └── {EventHandler}Tests.cs # Event handler tests
│   │   │   └── 📁 Services/            # Application service tests
│   │   │       └── {ApplicationService}Tests.cs # Service tests
│   │   └── 📁 Infrastructure/          # Infrastructure layer tests
│   │       ├── 📁 Repositories/        # Repository tests
│   │       │   └── {Repository}Tests.cs # Repository implementation tests
│   │       ├── 📁 Services/            # Infrastructure service tests
│   │       │   └── {InfrastructureService}Tests.cs # Service tests
│   │       └── 📁 Persistence/         # Persistence tests
│   │           └── {DbContext}Tests.cs # Database context tests
│   ├── 📁 Integration/                 # Integration tests
│   │   ├── 📁 API/                     # API integration tests
│   │   │   ├── {Entity}EndpointsTests.cs # Endpoint integration tests
│   │   │   └── {Business}FlowTests.cs    # Business flow tests
│   │   ├── 📁 Database/                # Database integration tests
│   │   │   ├── {Entity}RepositoryIntegrationTests.cs # Repository tests
│   │   │   └── DatabaseMigrationTests.cs # Migration tests
│   │   ├── 📁 External/                # External service integration tests
│   │   │   └── {ExternalService}IntegrationTests.cs # External service tests
│   │   └── 📁 Messaging/               # Message integration tests
│   │       └── {Message}IntegrationTests.cs # Message handling tests
│   ├── 📁 Acceptance/                  # Acceptance/E2E tests
│   │   ├── 📁 Features/                # Feature-based tests
│   │   │   └── {Feature}AcceptanceTests.cs # Feature acceptance tests
│   │   └── 📁 Scenarios/               # Scenario-based tests
│   │       └── {Scenario}Tests.cs      # End-to-end scenario tests
│   ├── 📁 TestUtilities/               # Test utilities and helpers
│   │   ├── 📁 Builders/                # Test data builders
│   │   │   ├── {Entity}Builder.cs      # Entity test builder
│   │   │   └── {Command}Builder.cs     # Command test builder
│   │   ├── 📁 Fixtures/                # Test fixtures
│   │   │   ├── DatabaseFixture.cs      # Database test fixture
│   │   │   └── WebApplicationFixture.cs # API test fixture
│   │   ├── 📁 Mocks/                   # Mock implementations
│   │   │   ├── Mock{Repository}.cs     # Repository mocks
│   │   │   └── Mock{ExternalService}.cs # External service mocks
│   │   └── 📁 Extensions/              # Test extensions
│   │       ├── TestDataExtensions.cs   # Test data generation
│   │       └── AssertionExtensions.cs  # Custom assertions
│   └── 📁 Configuration/               # Test configuration
│       ├── appsettings.Test.json       # Test environment settings
│       └── TestStartup.cs              # Test-specific startup configuration
└── 📁 Documentation/                   # Service documentation (Optional)
    ├── README.md                       # Service overview and setup
    ├── 📁 API/                         # API documentation
    │   ├── OpenAPI.json                # OpenAPI specification
    │   └── Endpoints.md                # Endpoint documentation
    ├── 📁 Architecture/                # Architecture documentation
    │   ├── Overview.md                 # Architecture overview
    │   ├── DomainModel.md              # Domain model documentation
    │   └── DataFlow.md                 # Data flow documentation
    ├── 📁 Deployment/                  # Deployment documentation
    │   ├── Docker.md                   # Docker setup
    │   └── Kubernetes.md               # Kubernetes deployment
    └── 📁 Development/                 # Development documentation
        ├── Setup.md                    # Development environment setup
        ├── Testing.md                  # Testing guidelines
        └── Contributing.md             # Contribution guidelines
```

## Layer Responsibilities

### 📁 API Layer (Presentation)
- **Purpose**: Handles HTTP requests and responses, API contracts
- **Components**:
  - **Endpoints**: Define API routes using Minimal APIs pattern
  - **Validators**: Request validation using FluentValidation
  - **Middleware**: Custom middleware for service-specific concerns
  - **Filters**: Action filters and custom attributes
  - **Extensions**: Service registration and pipeline configuration
  - **Configuration**: API-specific settings and options
- **Dependencies**: Application layer only
- **Responsibilities**:
  - HTTP request/response handling
  - Input validation and sanitization
  - API versioning and documentation
  - Authentication and authorization
  - Error handling and response formatting

### 📁 Application Layer (Use Cases)
- **Purpose**: Orchestrates business logic and handles use cases
- **Components**:
  - **Commands**: Handle write operations with CQRS pattern
  - **Queries**: Handle read operations with CQRS pattern
  - **DTOs**: Data transfer objects for API communication
  - **Event Handlers**: Handle domain and integration events
  - **Services**: Application-specific business logic
  - **Behaviors**: Cross-cutting concerns using MediatR pipeline
  - **Mappings**: Object-to-object mapping configurations
  - **Specifications**: Business rule specifications
  - **Exceptions**: Application-specific exceptions
- **Dependencies**: Domain layer only
- **Responsibilities**:
  - Use case orchestration
  - Business workflow coordination
  - Data transformation and mapping
  - Transaction management
  - Event handling and publishing
  - Validation and business rule enforcement

### 📁 Domain Layer (Business Logic)
- **Purpose**: Contains pure business rules and domain logic
- **Components**:
  - **Aggregates**: Aggregate roots and related entities
  - **Value Objects**: Immutable domain concepts
  - **Events**: Domain events for business notifications
  - **Exceptions**: Domain-specific exceptions
  - **Repositories**: Data access contracts
  - **Services**: Complex domain logic services
  - **Specifications**: Domain rule specifications
  - **Enums**: Domain enumerations
  - **Constants**: Domain constants and rules
- **Dependencies**: None (pure domain logic)
- **Responsibilities**:
  - Business rule enforcement
  - Domain model integrity
  - Business logic encapsulation
  - Domain event publishing
  - Entity lifecycle management
  - Business invariant validation

### 📁 Infrastructure Layer (External Concerns)
- **Purpose**: Implements external concerns and data access
- **Components**:
  - **Persistence**: Database contexts and configurations
  - **Repositories**: Data access implementations
  - **Services**: External service integrations
  - **Migrations**: Database schema management
  - **Configuration**: Infrastructure settings
  - **Logging**: Service-specific logging
  - **Security**: Authentication and authorization implementations
  - **Extensions**: Infrastructure service registrations
- **Dependencies**: Application and Domain layers
- **Responsibilities**:
  - Data persistence and retrieval
  - External system integration
  - Infrastructure service implementation
  - Database migration management
  - Caching and performance optimization
  - Security implementation
  - Monitoring and logging

### 📁 Tests (Quality Assurance)
- **Purpose**: Ensures code quality and functionality
- **Components**:
  - **Unit Tests**: Test individual components in isolation
  - **Integration Tests**: Test component interactions
  - **Acceptance Tests**: Test end-to-end scenarios
  - **Test Utilities**: Helper classes and test infrastructure
- **Responsibilities**:
  - Code coverage and quality assurance
  - Regression testing
  - Performance testing
  - API contract testing
  - Database integration testing

## Naming Conventions

### Files and Classes
- **Entities**: `{EntityName}.cs` (e.g., `User.cs`, `Order.cs`)
- **Commands**: `{Action}{Entity}Command.cs` (e.g., `CreateUserCommand.cs`)
- **Queries**: `Get{Entity}ByIdQuery.cs` or `Get{Entity}sQuery.cs`
- **DTOs**: `{Entity}Response.cs`, `Create{Entity}Request.cs`
- **Events**: `{Entity}{Action}Event.cs` (e.g., `UserCreatedEvent.cs`)
- **Repositories**: `I{Entity}Repository.cs` and `{Entity}Repository.cs`
- **Validators**: `{Action}{Entity}RequestValidator.cs`
- **Endpoints**: `{Entity}Endpoints.cs`
- **Services**: `I{Entity}Service.cs` and `{Entity}Service.cs`

### Namespaces
- **API**: `{ServiceName}.API`
- **Application**: `{ServiceName}.Application`
- **Domain**: `{ServiceName}.Domain`
- **Infrastructure**: `{ServiceName}.Infrastructure`
- **Tests**: `{ServiceName}.Tests`

### Folders
- Use **PascalCase** for all folder names
- Group related functionality together
- Separate interfaces and implementations when appropriate
- Use descriptive names that indicate purpose

## Design Patterns and Principles

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Represent write operations that change system state
- **Queries**: Represent read operations that return data
- **Handlers**: Process commands and queries independently
- **Separation**: Clear separation between read and write operations
- **Benefits**: Scalability, performance optimization, clear intent

### Domain Events
- **Events**: Raised when significant domain changes occur
- **Handlers**: Process events for cross-cutting concerns
- **Decoupling**: Loose coupling between domain logic and side effects
- **Integration**: Support for both domain and integration events

### Repository Pattern
- **Interface**: Defined in Domain layer for dependency inversion
- **Implementation**: Located in Infrastructure layer
- **Unit of Work**: Coordinates multiple repository operations
- **Abstraction**: Clean separation between domain and data access

### Specification Pattern
- **Domain Rules**: Encapsulate business rules and criteria
- **Composability**: Combine specifications using logical operators
- **Reusability**: Share common specifications across use cases
- **Testability**: Easy to test business rules in isolation

### Dependency Injection
- **Registration**: Each layer has its own service registration
- **Inversion**: Dependencies point inward toward domain
- **Testability**: Interfaces allow for easy testing and mocking
- **Lifetime**: Appropriate service lifetimes for different components

## Configuration Management

### Environment-Specific Settings
- **appsettings.json**: Base configuration
- **appsettings.Development.json**: Development overrides
- **appsettings.Staging.json**: Staging environment settings
- **appsettings.Production.json**: Production environment settings

### Configuration Classes
- **Strongly Typed**: Use configuration classes for type safety
- **Validation**: Validate configuration at startup
- **Separation**: Separate concerns by configuration area
- **Documentation**: Document configuration options

## Testing Strategy

### Unit Tests
- **Isolation**: Test components in complete isolation
- **Fast**: Run quickly without external dependencies
- **Focused**: Test single units of functionality
- **Coverage**: Aim for high code coverage

### Integration Tests
- **Interaction**: Test component interactions
- **Database**: Test database operations and migrations
- **External Services**: Test external system integrations
- **API**: Test API endpoints and contracts

### Acceptance Tests
- **End-to-End**: Test complete user scenarios
- **Business Value**: Verify business requirements
- **User Journey**: Test from user perspective
- **Automation**: Automate regression testing

## Example Usage

To create a new service following this structure:

1. **Replace placeholders**:
   - `{ServiceName}` → Your service name (e.g., "Auth", "Order", "Inventory")
   - `{Entity}` → Your main entity name (e.g., "User", "Order", "Product")
   - `{SecondaryEntity}` → Secondary entities (e.g., "Role", "OrderItem", "Category")
   - `{Property}` → Value object properties (e.g., "Email", "Address", "Money")
   - `{BusinessOperation}` → Business operations (e.g., "ActivateUser", "ProcessOrder")

2. **Follow naming conventions** for consistency across services

3. **Implement dependency injection** in each layer's DependencyInjection.cs

4. **Maintain clean architecture** principles with proper dependency direction

5. **Add comprehensive tests** for all layers and components

6. **Document APIs** using OpenAPI/Swagger specifications

7. **Configure environments** with appropriate settings for each environment

This comprehensive structure ensures consistency, maintainability, and scalability across all microservices while following clean architecture principles and modern .NET development practices.