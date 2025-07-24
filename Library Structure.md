```markdown
BuildingBlocks.Domain/
├── Entities/
│   ├── Entity.cs
│   ├── AggregateRoot.cs
│   ├── IAuditableEntity.cs
│   └── ISoftDeletable.cs
├── ValueObjects/
│   ├── ValueObject.cs
│   ├── SingleValueObject.cs
│   └── Enumeration.cs
├── StronglyTypedIds/
│   ├── IStronglyTypedId.cs
│   ├── StronglyTypedId.cs
│   ├── IntId.cs
│   ├── LongId.cs
│   ├── GuidId.cs
│   └── StringId.cs
├── DomainEvents/
│   ├── IDomainEvent.cs
│   ├── IDomainEventDispatcher.cs
│   ├── DomainEventDispatcher.cs
│   ├── DomainEventBase.cs
│   └── IDomainEventHandler.cs
├── Repository/
│   ├── IRepository.cs
│   ├── IReadOnlyRepository.cs
│   ├── IUnitOfWork.cs
│   └── RepositoryBase.cs
├── Specifications/
│   ├── ISpecification.cs
│   ├── Specification.cs
│   ├── AndSpecification.cs
│   ├── OrSpecification.cs
│   ├── NotSpecification.cs
│   └── SpecificationEvaluator.cs
├── Exceptions/
│   ├── DomainException.cs
│   ├── BusinessRuleValidationException.cs
│   ├── AggregateNotFoundException.cs
│   ├── ConcurrencyException.cs
│   └── InvalidOperationDomainException.cs
├── BusinessRules/
│   ├── IBusinessRule.cs
│   ├── BusinessRuleBase.cs
│   └── CompositeBusinessRule.cs
├── Common/
│   ├── Money.cs
│   ├── DateRange.cs
│   ├── Address.cs
│   ├── Email.cs
│   └── PhoneNumber.cs
└── Extensions/
    └── DomainExtensions.cs
```

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
│   ├── ValidationError.cs            # Validation error
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
│   └── InboxProcessor.cs             # Inbox processor implementation
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
│   └── ISagaManager.cs               # Saga manager interface
└── 📁 Extensions/                    # Dependency injection extensions
    ├── ServiceCollectionExtensions.cs # Service collection extensions
    ├── ApplicationExtensions.cs       # Application extensions
    └── MediatorExtensions.cs          # Mediator registration extensions
```
```markdown
BuildingBlocks.Infrastructure/
├── Data/
│   ├── Repositories/
│   │   ├── IRepository.cs
│   │   ├── Repository.cs
│   │   ├── IReadOnlyRepository.cs
│   │   ├── ReadOnlyRepository.cs
│   │   └── RepositoryBase.cs
│   ├── UnitOfWork/
│   │   ├── IUnitOfWork.cs
│   │   ├── UnitOfWork.cs
│   │   └── IDbTransaction.cs
│   ├── Context/
│   │   ├── IDbContext.cs
│   │   ├── ApplicationDbContext.cs
│   │   ├── DbContextBase.cs
│   │   └── IDbContextFactory.cs
│   ├── Migrations/
│   │   ├── IMigrationRunner.cs
│   │   └── MigrationRunner.cs
│   ├── Seeding/
│   │   ├── IDataSeeder.cs
│   │   ├── DataSeederBase.cs
│   │   └── SeedDataExtensions.cs
│   ├── Interceptors/
│   │   ├── AuditInterceptor.cs
│   │   ├── DomainEventInterceptor.cs
│   │   └── SoftDeleteInterceptor.cs
│   └── Configurations/
│       ├── EntityConfigurationBase.cs
│       ├── AuditableEntityConfiguration.cs
│       └── ValueObjectConfiguration.cs
├── Caching/
│   ├── ICacheService.cs
│   ├── MemoryCacheService.cs
│   ├── DistributedCacheService.cs
│   ├── RedisCacheService.cs
│   ├── CacheKeyGenerator.cs
│   └── CacheConfiguration.cs
├── Messaging/
│   ├── MessageBus/
│   │   ├── IMessageBus.cs
│   │   ├── InMemoryMessageBus.cs
│   │   ├── ServiceBusMessageBus.cs
│   │   └── RabbitMQMessageBus.cs
│   ├── EventBus/
│   │   ├── IEventBus.cs
│   │   ├── InMemoryEventBus.cs
│   │   ├── ServiceBusEventBus.cs
│   │   └── RabbitMQEventBus.cs
│   ├── Publishers/
│   │   ├── IMessagePublisher.cs
│   │   ├── MessagePublisherBase.cs
│   │   ├── ServiceBusPublisher.cs
│   │   └── RabbitMQPublisher.cs
│   ├── Subscribers/
│   │   ├── IMessageSubscriber.cs
│   │   ├── MessageSubscriberBase.cs
│   │   ├── ServiceBusSubscriber.cs
│   │   └── RabbitMQSubscriber.cs
│   ├── Serialization/
│   │   ├── IMessageSerializer.cs
│   │   ├── JsonMessageSerializer.cs
│   │   └── BinaryMessageSerializer.cs
│   └── Configuration/
│       ├── MessageBusConfiguration.cs
│       ├── ServiceBusConfiguration.cs
│       └── RabbitMQConfiguration.cs
├── Logging/
│   ├── ILoggerService.cs
│   ├── LoggerService.cs
│   ├── Serilog/
│   │   ├── SerilogConfiguration.cs
│   │   ├── SerilogExtensions.cs
│   │   └── Enrichers/
│   │       ├── UserContextEnricher.cs
│   │       ├── CorrelationIdEnricher.cs
│   │       └── ApplicationEnricher.cs
│   ├── ApplicationInsights/
│   │   ├── ApplicationInsightsConfiguration.cs
│   │   └── ApplicationInsightsExtensions.cs
│   └── Structured/
│       ├── IStructuredLogger.cs
│       ├── StructuredLogger.cs
│       └── LogEventExtensions.cs
├── Authentication/
│   ├── JWT/
│   │   ├── IJwtTokenService.cs
│   │   ├── JwtTokenService.cs
│   │   ├── JwtConfiguration.cs
│   │   └── JwtMiddleware.cs
│   ├── OAuth/
│   │   ├── IOAuthService.cs
│   │   ├── OAuthService.cs
│   │   └── OAuthConfiguration.cs
│   ├── ApiKey/
│   │   ├── IApiKeyService.cs
│   │   ├── ApiKeyService.cs
│   │   └── ApiKeyMiddleware.cs
│   └── Identity/
│       ├── IIdentityService.cs
│       ├── IdentityService.cs
│       └── IdentityConfiguration.cs
├── Authorization/
│   ├── IAuthorizationService.cs
│   ├── AuthorizationService.cs
│   ├── Policies/
│   │   ├── IPolicyService.cs
│   │   ├── PolicyService.cs
│   │   └── PolicyConfiguration.cs
│   ├── Handlers/
│   │   ├── PermissionHandler.cs
│   │   ├── RoleHandler.cs
│   │   └── ResourceHandler.cs
│   └── Requirements/
│       ├── PermissionRequirement.cs
│       ├── RoleRequirement.cs
│       └── ResourceRequirement.cs
├── Storage/
│   ├── Files/
│   │   ├── IFileStorageService.cs
│   │   ├── LocalFileStorageService.cs
│   │   ├── AzureBlobStorageService.cs
│   │   ├── AmazonS3StorageService.cs
│   │   └── FileStorageConfiguration.cs
│   ├── Blobs/
│   │   ├── IBlobStorageService.cs
│   │   ├── BlobStorageService.cs
│   │   └── BlobConfiguration.cs
│   └── Documents/
│       ├── IDocumentStorageService.cs
│       ├── DocumentStorageService.cs
│       └── DocumentConfiguration.cs
├── Communication/
│   ├── Email/
│   │   ├── IEmailService.cs
│   │   ├── SmtpEmailService.cs
│   │   ├── SendGridEmailService.cs
│   │   ├── EmailTemplate.cs
│   │   └── EmailConfiguration.cs
│   ├── SMS/
│   │   ├── ISmsService.cs
│   │   ├── TwilioSmsService.cs
│   │   ├── SmsTemplate.cs
│   │   └── SmsConfiguration.cs
│   ├── Push/
│   │   ├── IPushNotificationService.cs
│   │   ├── FirebasePushService.cs
│   │   ├── PushTemplate.cs
│   │   └── PushConfiguration.cs
│   └── Notifications/
│       ├── INotificationService.cs
│       ├── NotificationService.cs
│       ├── NotificationChannel.cs
│       └── NotificationConfiguration.cs
├── Monitoring/
│   ├── Health/
│   │   ├── IHealthCheckService.cs
│   │   ├── HealthCheckService.cs
│   │   ├── DatabaseHealthCheck.cs
│   │   ├── CacheHealthCheck.cs
│   │   ├── MessageBusHealthCheck.cs
│   │   └── HealthCheckConfiguration.cs
│   ├── Metrics/
│   │   ├── IMetricsService.cs
│   │   ├── MetricsService.cs
│   │   ├── PrometheusMetricsService.cs
│   │   └── MetricsConfiguration.cs
│   ├── Tracing/
│   │   ├── ITracingService.cs
│   │   ├── TracingService.cs
│   │   ├── OpenTelemetryConfiguration.cs
│   │   └── TracingMiddleware.cs
│   └── Performance/
│       ├── IPerformanceMonitor.cs
│       ├── PerformanceMonitor.cs
│       └── PerformanceConfiguration.cs
├── BackgroundServices/
│   ├── IBackgroundTaskService.cs
│   ├── BackgroundTaskService.cs
│   ├── Queues/
│   │   ├── IBackgroundQueue.cs
│   │   ├── BackgroundQueue.cs
│   │   └── QueueConfiguration.cs
│   ├── Jobs/
│   │   ├── IJobScheduler.cs
│   │   ├── JobScheduler.cs
│   │   ├── HangfireJobScheduler.cs
│   │   └── JobConfiguration.cs
│   └── Workers/
│       ├── IWorkerService.cs
│       ├── WorkerServiceBase.cs
│       └── WorkerConfiguration.cs
├── External/
│   ├── HttpClients/
│   │   ├── IHttpClientService.cs
│   │   ├── HttpClientService.cs
│   │   ├── HttpClientConfiguration.cs
│   │   └── HttpClientExtensions.cs
│   ├── APIs/
│   │   ├── IExternalApiService.cs
│   │   ├── ExternalApiService.cs
│   │   └── ApiConfiguration.cs
│   └── ThirdParty/
│       ├── IThirdPartyIntegrationService.cs
│       ├── ThirdPartyIntegrationService.cs
│       └── ThirdPartyConfiguration.cs
├── Security/
│   ├── Encryption/
│   │   ├── IEncryptionService.cs
│   │   ├── AesEncryptionService.cs
│   │   ├── RsaEncryptionService.cs
│   │   └── EncryptionConfiguration.cs
│   ├── Hashing/
│   │   ├── IHashingService.cs
│   │   ├── Pbkdf2HashingService.cs
│   │   ├── BcryptHashingService.cs
│   │   └── HashingConfiguration.cs
│   ├── KeyManagement/
│   │   ├── IKeyManagementService.cs
│   │   ├── AzureKeyVaultService.cs
│   │   ├── AwsKmsService.cs
│   │   └── KeyManagementConfiguration.cs
│   └── Secrets/
│       ├── ISecretsService.cs
│       ├── SecretsService.cs
│       └── SecretsConfiguration.cs
├── Mapping/
│   ├── AutoMapper/
│   │   ├── AutoMapperService.cs
│   │   ├── AutoMapperProfile.cs
│   │   └── AutoMapperConfiguration.cs
│   ├── Mapster/
│   │   ├── MapsterService.cs
│   │   ├── MapsterProfile.cs
│   │   └── MapsterConfiguration.cs
│   └── Manual/
│       ├── ManualMapperService.cs
│       └── MappingExtensions.cs
├── Validation/
│   ├── FluentValidation/
│   │   ├── FluentValidationService.cs
│   │   ├── ValidatorBase.cs
│   │   └── FluentValidationConfiguration.cs
│   ├── DataAnnotations/
│   │   ├── DataAnnotationValidationService.cs
│   │   └── ValidationAttributes.cs
│   └── Custom/
│       ├── CustomValidationService.cs
│       └── ValidationRules.cs
├── Serialization/
│   ├── Json/
│   │   ├── IJsonSerializer.cs
│   │   ├── SystemTextJsonSerializer.cs
│   │   ├── NewtonsoftJsonSerializer.cs
│   │   └── JsonConfiguration.cs
│   ├── Xml/
│   │   ├── IXmlSerializer.cs
│   │   ├── XmlSerializer.cs
│   │   └── XmlConfiguration.cs
│   ├── Binary/
│   │   ├── IBinarySerializer.cs
│   │   ├── ProtobufSerializer.cs
│   │   ├── MessagePackSerializer.cs
│   │   └── BinaryConfiguration.cs
│   └── Csv/
│       ├── ICsvSerializer.cs
│       ├── CsvSerializer.cs
│       └── CsvConfiguration.cs
├── Configuration/
│   ├── IConfigurationService.cs
│   ├── ConfigurationService.cs
│   ├── Settings/
│   │   ├── ApplicationSettings.cs
│   │   ├── DatabaseSettings.cs
│   │   ├── CacheSettings.cs
│   │   ├── MessageBusSettings.cs
│   │   ├── LoggingSettings.cs
│   │   ├── AuthenticationSettings.cs
│   │   ├── AuthorizationSettings.cs
│   │   ├── StorageSettings.cs
│   │   ├── CommunicationSettings.cs
│   │   ├── MonitoringSettings.cs
│   │   ├── BackgroundServiceSettings.cs
│   │   ├── ExternalSettings.cs
│   │   ├── SecuritySettings.cs
│   │   └── SerializationSettings.cs
│   ├── Providers/
│   │   ├── IConfigurationProvider.cs
│   │   ├── JsonConfigurationProvider.cs
│   │   ├── EnvironmentConfigurationProvider.cs
│   │   ├── AzureConfigurationProvider.cs
│   │   └── AwsConfigurationProvider.cs
│   └── Validation/
│       ├── IConfigurationValidator.cs
│       ├── ConfigurationValidator.cs
│       └── ConfigurationValidationExtensions.cs
└── Extensions/
    ├── ServiceCollectionExtensions.cs
    ├── ApplicationBuilderExtensions.cs
    ├── HostBuilderExtensions.cs
    ├── ConfigurationExtensions.cs
    ├── DatabaseExtensions.cs
    ├── CachingExtensions.cs
    ├── MessagingExtensions.cs
    ├── LoggingExtensions.cs
    ├── AuthenticationExtensions.cs
    ├── AuthorizationExtensions.cs
    ├── StorageExtensions.cs
    ├── CommunicationExtensions.cs
    ├── MonitoringExtensions.cs
    ├── BackgroundServiceExtensions.cs
    ├── ExternalExtensions.cs
    ├── SecurityExtensions.cs
    ├── MappingExtensions.cs
    ├── ValidationExtensions.cs
    ├── SerializationExtensions.cs
    └── InfrastructureExtensions.cs
```

```
BuildingBlocks.API/
├── Endpoints/
│   ├── Base/
│   │   ├── EndpointBase.cs
│   │   ├── CrudEndpoints.cs
│   │   └── QueryEndpoints.cs
│   ├── Extensions/
│   │   ├── EndpointRouteBuilderExtensions.cs
│   │   └── MinimalApiExtensions.cs
│   └── Conventions/
│       ├── ApiEndpointConvention.cs
│       └── VersioningEndpointConvention.cs
├── Middleware/
│   ├── ErrorHandling/
│   │   ├── GlobalExceptionMiddleware.cs
│   │   ├── ErrorResponse.cs
│   │   └── ProblemDetailsFactory.cs
│   ├── Logging/
│   │   ├── RequestLoggingMiddleware.cs
│   │   └── CorrelationIdMiddleware.cs
│   └── Security/
│       ├── SecurityHeadersMiddleware.cs
│       └── RateLimitingMiddleware.cs
├── Responses/
│   ├── Base/
│   │   ├── ApiResponse.cs
│   │   ├── PagedResponse.cs
│   │   └── ErrorResponse.cs
│   └── Builders/
│       ├── ApiResponseBuilder.cs
│       └── ErrorResponseBuilder.cs
├── Authentication/
│   ├── JWT/
│   │   ├── JwtAuthenticationExtensions.cs
│   │   └── JwtBearerOptionsSetup.cs
│   └── ApiKey/
│       ├── ApiKeyAuthenticationExtensions.cs
│       └── ApiKeyAuthenticationHandler.cs
├── Validation/
│   ├── Validators/
│   │   ├── RequestValidator.cs
│   │   └── PaginationValidator.cs
│   ├── Extensions/
│   │   ├── ValidationExtensions.cs
│   │   └── FluentValidationExtensions.cs
│   └── Results/
│       ├── ValidationResult.cs
│       └── ValidationError.cs
├── OpenApi/
│   ├── Configuration/
│   │   ├── OpenApiConfiguration.cs
│   │   ├── ScalarConfiguration.cs
│   │   └── ApiDocumentationOptions.cs
│   ├── Filters/
│   │   ├── AuthorizationOperationFilter.cs
│   │   └── DefaultResponseOperationFilter.cs
│   └── Extensions/
│       ├── OpenApiExtensions.cs
│       └── ScalarExtensions.cs
├── Versioning/
│   ├── Extensions/
│   │   ├── ApiVersioningExtensions.cs
│   │   └── VersionedEndpointExtensions.cs
│   └── Conventions/
│       ├── VersioningConvention.cs
│       └── EndpointVersioningConvention.cs
├── Health/
│   ├── Extensions/
│   │   ├── HealthCheckExtensions.cs
│   │   └── HealthEndpointExtensions.cs
│   └── Reporters/
│       ├── JsonHealthReporter.cs
│       └── SimpleHealthReporter.cs
├── Configuration/
│   ├── Options/
│   │   ├── ApiOptions.cs
│   │   ├── CorsOptions.cs
│   │   ├── AuthenticationOptions.cs
│   │   └── RateLimitingOptions.cs
│   └── Extensions/
│       ├── ConfigurationExtensions.cs
│       └── OptionsExtensions.cs
├── Extensions/
│   ├── ApiExtensions.cs
│   ├── AuthenticationExtensions.cs
│   ├── CorsExtensions.cs
│   ├── OpenApiExtensions.cs
│   ├── VersioningExtensions.cs
│   ├── RateLimitingExtensions.cs
│   ├── HealthCheckExtensions.cs
│   ├── ValidationExtensions.cs
│   ├── MiddlewareExtensions.cs
│   ├── SecurityExtensions.cs
│   ├── ErrorHandlingExtensions.cs
│   ├── HttpContextExtensions.cs
│   ├── ClaimsPrincipalExtensions.cs
│   ├── RequestExtensions.cs
│   └── ResponseExtensions.cs
└── Utilities/
    ├── Helpers/
    │   ├── ResponseHelper.cs
    │   ├── ValidationHelper.cs
    │   └── CorrelationHelper.cs
    ├── Constants/
    │   ├── ApiConstants.cs
    │   ├── HttpConstants.cs
    │   └── HeaderConstants.cs
    └── Factories/
        ├── ResponseFactory.cs
        └── ErrorFactory.cs
```