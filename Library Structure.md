# BuildingBlocks Library Structure

## BuildingBlocks.Domain
```
BuildingBlocks.Domain/
├── 📁 Entities/                        # Domain entities
│   ├── Entity.cs                       # Base entity implementation
│   ├── AggregateRoot.cs                # Aggregate root base class
│   ├── IAuditableEntity.cs             # Auditable entity interface
│   └── ISoftDeletable.cs               # Soft delete capability
├── 📁 ValueObjects/                    # Value objects
│   ├── ValueObject.cs                  # Base value object
│   ├── SingleValueObject.cs            # Single-value value object
│   └── Enumeration.cs                  # Enumeration base class
├── 📁 StronglyTypedIds/                # Strongly typed identifiers
│   ├── IStronglyTypedId.cs             # Base interface for typed IDs
│   ├── StronglyTypedId.cs              # Base strongly typed ID
│   ├── IntId.cs                        # Integer-based ID
│   ├── LongId.cs                       # Long-based ID
│   ├── GuidId.cs                       # GUID-based ID
│   ├── StringId.cs                     # String-based ID
│   └── 📁 Json/                        # JSON serialization support
│       ├── StronglyTypedIdJsonConverter.cs      # JSON converter
│       └── StronglyTypedIdJsonConverterFactory.cs  # Converter factory
├── 📁 DomainEvents/                    # Domain event handling
│   ├── IDomainEvent.cs                 # Domain event interface
│   ├── IDomainEventDispatcher.cs       # Event dispatcher interface
│   ├── DomainEventDispatcher.cs        # Event dispatcher implementation
│   ├── DomainEventBase.cs              # Base domain event
│   └── IDomainEventHandler.cs          # Event handler interface
├── 📁 Repository/                      # Repository pattern
│   ├── IRepository.cs                  # Repository interface
│   ├── IReadOnlyRepository.cs          # Read-only repository interface
│   ├── IUnitOfWork.cs                  # Unit of work interface
│   └── RepositoryBase.cs               # Base repository implementation
├── 📁 Specifications/                  # Specification pattern
│   ├── ISpecification.cs               # Specification interface
│   ├── Specification.cs                # Base specification
│   ├── AndSpecification.cs             # AND specification
│   ├── OrSpecification.cs              # OR specification
│   ├── NotSpecification.cs             # NOT specification
│   └── SpecificationEvaluator.cs       # Specification evaluator
├── 📁 Exceptions/                      # Domain exceptions
│   ├── DomainException.cs              # Base domain exception
│   ├── BusinessRuleValidationException.cs # Business rule violation
│   ├── AggregateNotFoundException.cs   # Aggregate not found
│   ├── ConcurrencyException.cs         # Concurrency conflicts
│   └── InvalidOperationDomainException.cs # Invalid domain operations
├── 📁 BusinessRules/                   # Business rules
│   ├── IBusinessRule.cs                # Business rule interface
│   ├── BusinessRuleBase.cs             # Base business rule
│   └── CompositeBusinessRule.cs        # Composite business rules
├── 📁 Common/                          # Common value objects
│   ├── Money.cs                        # Money value object
│   ├── DateRange.cs                    # Date range value object
│   ├── Address.cs                      # Address value object
│   ├── Email.cs                        # Email value object
│   └── PhoneNumber.cs                  # Phone number value object
├── 📁 Guards/                          # Guard clauses
│   └── Guard.cs                        # Guard utilities
└── 📁 Extensions/                      # Domain extensions
    └── DomainExtensions.cs             # Domain-specific extensions
```

## BuildingBlocks.Application
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
│   ├── InboxProcessor.cs             # Inbox processor implementation
│   └── IInboxMessageHandler.cs       # Inbox message handler interface
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
│   ├── ISagaOrchestrator.cs          # Saga orchestrator interface
│   ├── ISagaRepository.cs            # Saga repository interface
│   ├── SagaStep.cs                   # Saga step definition
│   └── SagaExtensions.cs             # Saga extensions
└── 📁 Extensions/                    # Dependency injection extensions
    ├── ServiceCollectionExtensions.cs # Service collection extensions
    ├── ApplicationExtensions.cs       # Application extensions
    └── MediatorExtensions.cs          # Mediator registration extensions
```
## BuildingBlocks.Infrastructure
```
BuildingBlocks.Infrastructure/
├── 📁 Data/                            # Data access layer
│   ├── 📁 Repositories/                # Repository implementations
│   │   ├── IRepository.cs              # Repository interface
│   │   ├── Repository.cs               # Repository implementation
│   │   ├── IReadOnlyRepository.cs      # Read-only repository interface
│   │   ├── ReadOnlyRepository.cs       # Read-only repository implementation
│   │   └── RepositoryBase.cs           # Base repository
│   ├── 📁 UnitOfWork/                  # Unit of work pattern
│   │   ├── IUnitOfWork.cs              # Unit of work interface
│   │   ├── UnitOfWork.cs               # Unit of work implementation
│   │   └── IDbTransaction.cs           # Database transaction interface
│   ├── 📁 Context/                     # Database contexts
│   │   ├── IDbContext.cs               # Database context interface
│   │   ├── ApplicationDbContext.cs     # Application database context
│   │   ├── DbContextBase.cs            # Base database context
│   │   └── IDbContextFactory.cs        # Database context factory
│   ├── 📁 Migrations/                  # Database migrations
│   │   ├── IMigrationRunner.cs         # Migration runner interface
│   │   └── MigrationRunner.cs          # Migration runner implementation
│   ├── 📁 Seeding/                     # Database seeding
│   │   ├── IDataSeeder.cs              # Data seeder interface
│   │   ├── DataSeederBase.cs           # Base data seeder
│   │   └── SeedDataExtensions.cs       # Seed data extensions
│   ├── 📁 Interceptors/                # Entity Framework interceptors
│   │   ├── AuditInterceptor.cs         # Audit trail interceptor
│   │   ├── DomainEventInterceptor.cs   # Domain event interceptor
│   │   └── SoftDeleteInterceptor.cs    # Soft delete interceptor
│   ├── 📁 Converters/                  # Value converters
│   │   ├── StronglyTypedIdValueConverter.cs         # Strongly typed ID converter
│   │   └── StronglyTypedIdValueConverterSelector.cs # Converter selector
│   └── 📁 Configurations/              # Entity configurations
│       ├── EntityConfigurationBase.cs  # Base entity configuration
│       ├── AuditableEntityConfiguration.cs # Auditable entity configuration
│       ├── ValueObjectConfiguration.cs # Value object configuration
│       ├── InboxMessageConfiguration.cs # Inbox message configuration
│       └── OutboxMessageConfiguration.cs # Outbox message configuration
├── 📁 Caching/                         # Caching implementations
│   ├── MemoryCacheService.cs           # In-memory cache service
│   ├── DistributedCacheService.cs      # Distributed cache service
│   ├── RedisCacheService.cs            # Redis cache service
│   ├── CacheKeyGenerator.cs            # Cache key generator
│   └── CacheConfiguration.cs           # Cache configuration
├── 📁 Messaging/                       # Message bus implementations
│   ├── 📁 MessageBus/                  # Message bus implementations
│   │   ├── IMessageBus.cs              # Message bus interface
│   │   └── InMemoryMessageBus.cs       # In-memory message bus
│   ├── 📁 EventBus/                    # Event bus implementations
│   │   ├── IEventBus.cs                # Event bus interface
│   │   └── InMemoryEventBus.cs         # In-memory event bus
│   ├── 📁 Publishers/                  # Message publishers
│   │   └── IMessagePublisher.cs        # Message publisher interface
│   ├── 📁 Subscribers/                 # Message subscribers
│   │   └── IMessageSubscriber.cs       # Message subscriber interface
│   ├── 📁 Serialization/              # Message serialization
│   │   └── IMessageSerializer.cs       # Message serializer interface
│   └── 📁 Configuration/               # Messaging configuration
│       └── MessageBusConfiguration.cs  # Message bus configuration
├── 📁 Logging/                         # Logging implementations
│   ├── ILoggerService.cs               # Logger service interface
│   ├── LoggerService.cs                # Logger service implementation
│   ├── 📁 Serilog/                     # Serilog configuration
│   │   └── 📁 Enrichers/               # Log enrichers
│   ├── 📁 ApplicationInsights/         # Application Insights
│   └── 📁 Structured/                  # Structured logging
├── 📁 Authentication/                  # Authentication implementations
│   ├── 📁 JWT/                         # JWT authentication
│   │   ├── IJwtTokenService.cs         # JWT token service interface
│   │   ├── JwtTokenService.cs          # JWT token service implementation
│   │   └── JwtConfiguration.cs         # JWT configuration
│   ├── 📁 OAuth/                       # OAuth authentication
│   ├── 📁 ApiKey/                      # API key authentication
│   └── 📁 Identity/                    # Identity management
├── 📁 Authorization/                   # Authorization implementations
│   ├── IAuthorizationService.cs        # Authorization service interface
│   ├── 📁 Policies/                    # Authorization policies
│   ├── 📁 Handlers/                    # Authorization handlers
│   └── 📁 Requirements/                # Authorization requirements
├── 📁 Storage/                         # Storage implementations
│   ├── 📁 Files/                       # File storage
│   │   ├── IFileStorageService.cs      # File storage interface
│   │   └── LocalFileStorageService.cs  # Local file storage
│   ├── 📁 Blobs/                       # Blob storage
│   │   └── IBlobStorageService.cs      # Blob storage interface
│   └── 📁 Documents/                   # Document storage
├── 📁 Communication/                   # Communication services
│   ├── 📁 Email/                       # Email services
│   │   ├── IEmailService.cs            # Email service interface
│   │   ├── SmtpEmailService.cs         # SMTP email service
│   │   ├── IEmailTemplateService.cs    # Email template service interface
│   │   ├── EmailTemplateService.cs     # Email template service
│   │   └── EmailConfiguration.cs       # Email configuration
│   ├── 📁 SMS/                         # SMS services
│   ├── 📁 Push/                        # Push notifications
│   └── 📁 Notifications/               # Notification services
├── 📁 Monitoring/                      # Monitoring and observability
│   ├── 📁 Health/                      # Health checks
│   │   ├── IHealthCheckService.cs      # Health check service interface
│   │   ├── HealthCheckConfiguration.cs # Health check configuration
│   │   ├── HealthCheckExtensions.cs    # Health check extensions
│   │   ├── HttpHealthCheck.cs          # HTTP health check
│   │   ├── MemoryHealthCheck.cs        # Memory health check
│   │   └── SmtpHealthCheck.cs          # SMTP health check
│   ├── 📁 Metrics/                     # Metrics collection
│   ├── 📁 Tracing/                     # Distributed tracing
│   └── 📁 Performance/                 # Performance monitoring
├── 📁 OpenTelemetry/                   # OpenTelemetry integration
│   ├── OpenTelemetryConfiguration.cs   # OpenTelemetry configuration
│   └── OpenTelemetryExtensions.cs      # OpenTelemetry extensions
├── 📁 BackgroundServices/              # Background service implementations
│   ├── IBackgroundTaskService.cs       # Background task service interface
│   ├── 📁 Queues/                      # Background queues
│   ├── 📁 Jobs/                        # Job scheduling
│   └── 📁 Workers/                     # Worker services
├── 📁 External/                        # External service integrations
│   ├── 📁 HttpClients/                 # HTTP client services
│   │   ├── IHttpClientService.cs       # HTTP client service interface
│   │   ├── HttpClientService.cs        # HTTP client service
│   │   └── HttpClientConfiguration.cs  # HTTP client configuration
│   ├── 📁 APIs/                        # External API integrations
│   └── 📁 ThirdParty/                  # Third-party integrations
├── 📁 Security/                        # Security implementations
│   ├── 📁 Encryption/                  # Encryption services
│   │   └── IEncryptionService.cs       # Encryption service interface
│   ├── 📁 Hashing/                     # Hashing services
│   ├── 📁 KeyManagement/               # Key management
│   └── 📁 Secrets/                     # Secret management
├── 📁 Mapping/                         # Object mapping implementations
│   ├── 📁 AutoMapper/                  # AutoMapper integration
│   ├── 📁 Mapster/                     # Mapster integration
│   └── 📁 Manual/                      # Manual mapping
├── 📁 Validation/                      # Validation implementations
│   ├── 📁 FluentValidation/            # FluentValidation integration
│   │   └── FluentValidationService.cs  # FluentValidation service
│   ├── 📁 DataAnnotations/             # Data annotations validation
│   └── 📁 Custom/                      # Custom validation
├── 📁 Serialization/                   # Serialization implementations
│   ├── 📁 Json/                        # JSON serialization
│   │   ├── IJsonSerializer.cs          # JSON serializer interface
│   │   └── SystemTextJsonSerializer.cs # System.Text.Json serializer
│   ├── 📁 Xml/                         # XML serialization
│   │   └── IXmlSerializer.cs           # XML serializer interface
│   ├── 📁 Binary/                      # Binary serialization
│   │   └── IBinarySerializer.cs        # Binary serializer interface
│   └── 📁 Csv/                         # CSV serialization
├── 📁 Configuration/                   # Configuration management
│   ├── IConfigurationService.cs        # Configuration service interface
│   ├── ConfigurationService.cs         # Configuration service
│   ├── 📁 Settings/                    # Configuration settings
│   ├── 📁 Providers/                   # Configuration providers
│   └── 📁 Validation/                  # Configuration validation
├── 📁 Services/                        # Infrastructure services
│   ├── InboxService.cs                 # Inbox service implementation
│   └── OutboxService.cs                # Outbox service implementation
└── 📁 Extensions/                      # Infrastructure extensions
    ├── ServiceCollectionExtensions.cs  # Service registration extensions
    ├── ApplicationBuilderExtensions.cs # Application builder extensions
    ├── DatabaseExtensions.cs           # Database extensions
    ├── CachingExtensions.cs             # Caching extensions
    ├── ModelBuilderExtensions.cs       # Entity Framework model extensions
    ├── ServiceRegistration.cs          # Service registration helper
    └── InfrastructureExtensions.cs     # General infrastructure extensions
```

## BuildingBlocks.API
```
BuildingBlocks.API/
├── 📁 Endpoints/                       # API endpoint definitions
│   ├── 📁 Base/                        # Base endpoint classes
│   │   ├── EndpointBase.cs             # Base endpoint implementation
│   │   ├── CrudEndpoints.cs            # CRUD endpoint template
│   │   └── QueryEndpoints.cs           # Query endpoint template
│   ├── 📁 Extensions/                  # Endpoint extensions
│   │   ├── EndpointRouteBuilderExtensions.cs # Route builder extensions
│   │   └── MinimalApiExtensions.cs     # Minimal API extensions
│   └── 📁 Conventions/                 # API conventions
│       └── ApiEndpointConvention.cs    # Endpoint conventions
├── 📁 Middleware/                      # API middleware
│   ├── 📁 ErrorHandling/               # Error handling middleware
│   │   ├── GlobalExceptionMiddleware.cs # Global exception handler
│   │   └── ProblemDetailsFactory.cs    # Problem details factory
│   ├── 📁 Logging/                     # Logging middleware
│   │   ├── RequestLoggingMiddleware.cs # Request logging
│   │   └── CorrelationIdMiddleware.cs  # Correlation ID tracking
│   └── 📁 Security/                    # Security middleware
│       ├── SecurityHeadersMiddleware.cs # Security headers
│       └── RateLimitingMiddleware.cs   # Rate limiting
├── 📁 Responses/                       # API response handling
│   ├── 📁 Base/                        # Base response classes
│   │   └── ApiResponse.cs              # Standard API response
│   └── 📁 Builders/                    # Response builders
│       ├── ApiResponseBuilder.cs       # API response builder
│       └── ErrorResponseBuilder.cs     # Error response builder
├── 📁 Authentication/                  # Authentication handling
│   ├── 📁 JWT/                         # JWT authentication
│   │   ├── JwtAuthenticationExtensions.cs # JWT extensions
│   │   └── JwtBearerOptionsSetup.cs    # JWT bearer options
│   └── 📁 ApiKey/                      # API key authentication
│       ├── ApiKeyAuthenticationExtensions.cs # API key extensions
│       └── ApiKeyAuthenticationHandler.cs    # API key handler
├── 📁 Validation/                      # Request validation
│   ├── 📁 Validators/                  # Validation classes
│   │   ├── RequestValidator.cs         # Generic request validator
│   │   └── PaginationValidator.cs      # Pagination validator
│   ├── 📁 Extensions/                  # Validation extensions
│   │   ├── ValidationExtensions.cs     # Validation extensions
│   │   └── FluentValidationExtensions.cs # FluentValidation extensions
│   └── 📁 Results/                     # Validation results
│       └── ValidationResult.cs         # Validation result model
├── 📁 OpenApi/                         # OpenAPI/Swagger configuration
│   ├── 📁 Configuration/               # OpenAPI configuration
│   │   └── ApiDocumentationOptions.cs # API documentation options
│   └── 📁 Extensions/                  # OpenAPI extensions
│       └── OpenApiExtensions.cs        # OpenAPI setup extensions
├── 📁 Versioning/                      # API versioning
│   ├── 📁 Extensions/                  # Versioning extensions
│   │   └── ApiVersioningExtensions.cs  # API versioning extensions
│   └── 📁 Conventions/                 # Versioning conventions
│       └── VersioningConvention.cs     # Version conventions
├── 📁 Health/                          # Health check endpoints
│   ├── 📁 Extensions/                  # Health check extensions
│   │   └── HealthCheckExtensions.cs    # Health check setup
│   └── 📁 Reporters/                   # Health check reporters
│       └── JsonHealthReporter.cs       # JSON health reporter
├── 📁 Configuration/                   # API configuration
│   ├── 📁 Options/                     # Configuration options
│   │   └── ApiOptions.cs               # API configuration options
│   ├── 📁 Examples/                    # Configuration examples
│   │   └── appsettings.ratelimiting.example.json # Rate limiting example
│   └── 📁 Extensions/                  # Configuration extensions
│       └── ConfigurationExtensions.cs  # Configuration helpers
├── 📁 Converters/                      # JSON converters
│   ├── CustomDateTimeConverter.cs      # DateTime converter
│   ├── CustomDateTimeOffsetConverter.cs # DateTimeOffset converter
│   ├── CustomDecimalConverter.cs       # Decimal converter
│   ├── CustomGuidConverter.cs          # GUID converter
│   ├── CustomNullableDateTimeConverter.cs # Nullable DateTime converter
│   ├── CustomPhoneNumberConverter.cs   # Phone number converter
│   ├── FlexibleStringConverter.cs      # Flexible string converter
│   └── JsonStringEnumConverter.cs      # String enum converter
├── 📁 Extensions/                      # API extensions
│   ├── ApiExtensions.cs                # General API extensions
│   ├── ClaimsPrincipalExtensions.cs    # Claims principal extensions
│   ├── ErrorHandlingExtensions.cs     # Error handling extensions
│   ├── HttpContextExtensions.cs       # HTTP context extensions
│   ├── JsonExtensions.cs               # JSON extensions
│   ├── MiddlewareExtensions.cs         # Middleware extensions
│   ├── RateLimitingExtensions.cs       # Rate limiting extensions
│   ├── RequestExtensions.cs            # Request extensions
│   ├── ResponseExtensions.cs           # Response extensions
│   ├── SecurityExtensions.cs           # Security extensions
│   ├── ValidationExtensions.cs         # Validation extensions
│   └── VersioningExtensions.cs         # Versioning extensions
└── 📁 Utilities/                       # API utilities
    ├── 📁 Helpers/                     # Helper classes
    │   ├── ResponseHelper.cs           # Response helpers
    │   ├── ValidationHelper.cs         # Validation helpers
    │   └── CorrelationHelper.cs        # Correlation helpers
    ├── 📁 Constants/                   # API constants
    │   ├── ApiConstants.cs             # General API constants
    │   ├── HeaderConstants.cs          # HTTP header constants
    │   └── HttpConstants.cs            # HTTP constants
    └── 📁 Factories/                   # Factory classes
        ├── ErrorFactory.cs             # Error factory
        └── ResponseFactory.cs          # Response factory
```