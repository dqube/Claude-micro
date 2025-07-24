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