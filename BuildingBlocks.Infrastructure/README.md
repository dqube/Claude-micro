# BuildingBlocks.Infrastructure

A comprehensive infrastructure layer implementation for .NET applications following Clean Architecture principles. This library provides essential infrastructure services including data access, caching, messaging, authentication, security, monitoring, and more.

## 🏗️ Architecture Overview

This infrastructure library implements the infrastructure layer of Clean Architecture, providing concrete implementations for all application layer abstractions. It focuses on external concerns like databases, caching, messaging, file storage, and third-party integrations.

## 📦 Core Features

### 📊 Data Access & Persistence
- **Repository Pattern** with generic interfaces and Unit of Work
- **Entity Framework Core** integration with advanced interceptors
- **Domain Event Handling** through specialized interceptors
- **Audit Trail Support** with automatic creation/modification tracking
- **Soft Delete** functionality with query filtering
- **Data Seeding** framework for initial data population
- **Migration Management** with automated runners
- **Strongly Typed ID Converters** for EF Core value conversion
- **Entity Configuration** base classes for consistent mapping

### 💾 Caching Infrastructure
- **Multi-Tier Caching Strategy** (In-Memory, Distributed, Redis)
- **Cache Abstraction Layer** with pluggable implementations
- **Smart Cache Key Management** with automatic generation
- **Cache Policies** with expiration and invalidation strategies
- **Performance Optimized** cache operations with async patterns

### 🔐 Authentication & Authorization
- **JWT Token Services** with comprehensive token management
- **API Key Authentication** with middleware support
- **OAuth Integration** for third-party authentication
- **Identity Management** services and abstractions
- **Authorization Policies** with requirement-based handlers
- **Role-Based Access Control** with flexible policy configuration

### 🛡️ Security Services
- **Encryption Services** (AES, RSA) with configuration management
- **Hashing Services** (PBKDF2, BCrypt) for password security
- **Key Management** integration (Azure Key Vault, AWS KMS)
- **Secrets Management** with secure storage abstractions

### 📁 Storage Solutions
- **File Storage Abstraction** with multiple provider support
- **Local File Storage** implementation
- **Cloud Storage** integration (Azure Blob, Amazon S3)
- **Document Storage** services for structured documents
- **Blob Management** with metadata and versioning support

### 📨 Messaging & Communication
- **Message Bus Pattern** with in-memory and external implementations
- **Event Bus** for domain event propagation
- **Publisher/Subscriber** pattern implementation
- **Message Serialization** with pluggable serializers
- **Email Services** (SMTP, SendGrid) with template support
- **SMS Services** (Twilio) with template management
- **Push Notifications** (Firebase) for mobile applications

### 📝 Logging & Observability
- **Structured Logging** with Serilog integration and enrichers
- **Application Insights** integration for telemetry
- **Performance Monitoring** with metrics collection
- **Health Checks** for system components and dependencies
- **Distributed Tracing** with OpenTelemetry support
- **Custom Log Enrichers** for correlation IDs and user context

### ⚙️ Background Processing
- **Background Task Services** for long-running operations
- **Job Scheduling** with Hangfire integration
- **Queue Management** for task processing
- **Worker Services** base classes for hosted services

### 🔧 Configuration & Mapping
- **Configuration Management** with validation and providers
- **Settings Classes** for typed configuration access
- **AutoMapper Integration** with profile management
- **Mapster Support** for high-performance mapping
- **Manual Mapping** utilities and extensions

### 🔍 Validation & Serialization
- **FluentValidation Integration** with service registration
- **Data Annotations** validation services
- **JSON Serialization** (System.Text.Json, Newtonsoft.Json)
- **XML Serialization** for legacy system integration
- **Binary Serialization** (Protobuf, MessagePack) for performance
- **CSV Processing** for data import/export

### 🌐 External Integrations
- **HTTP Client Services** with configuration and policies
- **Third-Party API** integration abstractions
- **External Service** wrappers and adapters

## 📋 Directory Structure

```
BuildingBlocks.Infrastructure/
├── 📁 Authentication/          # Authentication services and middleware
│   ├── 🔑 JWT/                # JWT token services and configuration
│   ├── 🔐 ApiKey/             # API key authentication
│   ├── 🌐 OAuth/              # OAuth integration services
│   └── 👤 Identity/           # Identity management services
├── 📁 Authorization/           # Authorization policies and handlers
│   ├── 📜 Policies/           # Authorization policy definitions
│   ├── 🎯 Handlers/           # Authorization requirement handlers
│   └── 📋 Requirements/       # Custom authorization requirements
├── 📁 BackgroundServices/     # Background processing infrastructure
│   ├── 📥 Queues/             # Background task queues
│   ├── 💼 Jobs/               # Job scheduling and management
│   └── ⚙️ Workers/            # Worker service implementations
├── 📁 Caching/                # Multi-tier caching implementation
│   ├── 🧠 MemoryCacheService  # In-memory caching
│   ├── 🌐 DistributedCache    # Distributed caching abstraction
│   ├── 🔴 RedisCacheService   # Redis implementation
│   └── 🔑 CacheKeyGenerator   # Smart cache key management
├── 📁 Communication/          # External communication services
│   ├── 📧 Email/              # Email services (SMTP, SendGrid)
│   ├── 📱 SMS/                # SMS services (Twilio)
│   ├── 🔔 Push/               # Push notification services
│   └── 📢 Notifications/      # Unified notification system
├── 📁 Configuration/          # Configuration management
│   ├── ⚙️ Settings/           # Typed configuration classes
│   ├── 🔧 Providers/          # Configuration providers
│   └── ✅ Validation/         # Configuration validation
├── 📁 Data/                   # Data access infrastructure
│   ├── 🗄️ Context/            # EF Core DbContext implementations
│   ├── 🔄 Interceptors/       # EF Core interceptors (Audit, Events, SoftDelete)
│   ├── 📦 Repositories/       # Repository pattern implementations
│   ├── 🔄 UnitOfWork/         # Unit of Work pattern
│   ├── 🌱 Seeding/            # Data seeding framework
│   ├── 📈 Migrations/         # Database migration management
│   ├── ⚙️ Configurations/     # Entity configurations
│   └── 🔄 Converters/         # Value converters for strongly-typed IDs
├── 📁 Extensions/             # Service registration and configuration
│   ├── 🔧 ServiceCollection   # DI container extensions
│   ├── 🏗️ ApplicationBuilder  # Application pipeline extensions
│   └── 📊 Various domain-specific extensions
├── 📁 External/               # External service integrations
│   ├── 🌐 HttpClients/        # HTTP client services
│   ├── 🔌 APIs/               # External API integrations
│   └── 🏢 ThirdParty/         # Third-party service wrappers
├── 📁 Logging/                # Logging and observability
│   ├── 📝 Serilog/            # Serilog configuration and enrichers
│   ├── 📊 ApplicationInsights/# Application Insights integration
│   └── 🏗️ Structured/         # Structured logging utilities
├── 📁 Mapping/                # Object mapping services
│   ├── 🗺️ AutoMapper/         # AutoMapper integration
│   ├── ⚡ Mapster/            # Mapster integration
│   └── ✋ Manual/             # Manual mapping utilities
├── 📁 Messaging/              # Messaging infrastructure
│   ├── 📨 MessageBus/         # Message bus implementations
│   ├── 📡 EventBus/           # Event bus for domain events
│   ├── 📤 Publishers/         # Message publishers
│   ├── 📥 Subscribers/        # Message subscribers
│   ├── 🔄 Serialization/      # Message serialization
│   └── ⚙️ Configuration/      # Messaging configuration
├── 📁 Monitoring/             # System monitoring and health
│   ├── 💓 Health/             # Health check implementations
│   ├── 📊 Metrics/            # Metrics collection and reporting
│   ├── 🔍 Tracing/            # Distributed tracing
│   └── ⚡ Performance/        # Performance monitoring
├── 📁 Security/               # Security services
│   ├── 🔐 Encryption/         # Encryption services (AES, RSA)
│   ├── #️⃣ Hashing/           # Hashing services (PBKDF2, BCrypt)
│   ├── 🔑 KeyManagement/      # Key management (Azure KV, AWS KMS)
│   └── 🤐 Secrets/            # Secrets management
├── 📁 Serialization/          # Serialization services
│   ├── 📄 Json/               # JSON serialization
│   ├── 📋 Xml/                # XML serialization
│   ├── 📦 Binary/             # Binary serialization
│   └── 📊 Csv/                # CSV processing
├── 📁 Storage/                # Storage abstractions and implementations
│   ├── 📁 Files/              # File storage services
│   ├── 🗂️ Blobs/              # Blob storage services
│   └── 📄 Documents/          # Document storage services
└── 📁 Validation/             # Validation services
    ├── ✅ FluentValidation/   # FluentValidation integration
    ├── 📝 DataAnnotations/    # Data annotations validation
    └── 🔧 Custom/             # Custom validation rules
```

## 🚀 Installation

Add the project reference to your infrastructure layer:

```xml
<ProjectReference Include="..\BuildingBlocks.Infrastructure\BuildingBlocks.Infrastructure.csproj" />
```

## ⚡ Quick Start

Register infrastructure services in `Program.cs`:

```csharp
using BuildingBlocks.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register all infrastructure services
builder.Services.AddInfrastructure();

// Or register specific service groups
builder.Services.AddDataLayer();
builder.Services.AddCaching();
builder.Services.AddMessaging();
builder.Services.AddAuthentication();

var app = builder.Build();
```

## ⚙️ Configuration

Configure `appsettings.json` with comprehensive settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourApp;Trusted_Connection=true;",
    "Redis": "localhost:6379"
  },
  "Cache": {
    "KeyPrefix": "yourapp:",
    "DefaultExpiration": "00:30:00",
    "Provider": "Memory" // Memory, Distributed, Redis
  },
  "Jwt": {
    "SecretKey": "your-secret-key-must-be-at-least-32-characters-long",
    "Issuer": "YourApp",
    "Audience": "YourApp-API",
    "ExpiryMinutes": "60",
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true
  },
  "Storage": {
    "Provider": "Local", // Local, AzureBlob, AmazonS3
    "LocalPath": "./uploads",
    "AzureBlob": {
      "ConnectionString": "",
      "ContainerName": "files"
    }
  },
  "Messaging": {
    "Provider": "InMemory", // InMemory, ServiceBus, RabbitMQ
    "ServiceBus": {
      "ConnectionString": ""
    }
  },
  "Email": {
    "Provider": "Smtp", // Smtp, SendGrid
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "",
      "Password": ""
    }
  }
}
```

## 🔧 Key Implementation Examples

### 📊 Entity Framework Core Integration

```csharp
// Automatic audit tracking with interceptors
public class Product : Entity<ProductId>, IAuditableEntity
{
    public string Name { get; private set; }
    public Money Price { get; private set; }
    
    // Automatically populated by AuditInterceptor
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

// Soft delete support with interceptor
public class Category : Entity<CategoryId>, ISoftDeletable
{
    public string Name { get; private set; }
    
    // Handled by SoftDeleteInterceptor
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
}

// ApplicationDbContext with interceptors
public class ApplicationDbContext : DbContextBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .AddInterceptors(new AuditInterceptor(_currentUserService))
            .AddInterceptors(new DomainEventInterceptor(_mediator))
            .AddInterceptors(new SoftDeleteInterceptor());
    }
}
```

### 📦 Repository Pattern Implementation

```csharp
public class ProductService
{
    private readonly IRepository<Product, ProductId> _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Product> CreateProductAsync(Product product)
    {
        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product;
    }
    
    public async Task<Product?> GetBySpecificationAsync(ISpecification<Product> spec)
    {
        return await _repository.GetBySpecificationAsync(spec);
    }
}

// Using specifications for complex queries
public class ActiveProductsSpec : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => !p.IsDeleted && p.Price.Amount > 0;
    }
}
```

### 💾 Advanced Caching Implementation

```csharp
public class CachedProductService
{
    private readonly ICacheService _cache;
    private readonly IRepository<Product, ProductId> _repository;
    
    public async Task<Product?> GetProductAsync(ProductId id)
    {
        // Smart cache key generation
        var cacheKey = CacheKeyGenerator.Generate<Product>(id.Value);
        var product = await _cache.GetAsync<Product>(cacheKey);
        
        if (product == null)
        {
            product = await _repository.GetByIdAsync(id);
            if (product != null)
            {
                var policy = new CachePolicy
                {
                    AbsoluteExpiration = TimeSpan.FromHours(1),
                    SlidingExpiration = TimeSpan.FromMinutes(15)
                };
                await _cache.SetAsync(cacheKey, product, policy);
            }
        }
        
        return product;
    }
}

// Multi-tier caching strategy
services.AddCaching()
    .AddDistributedCaching(redisConnection)
    .AddMemoryCache();
```

### 🔐 JWT Authentication with Advanced Features

```csharp
public class AuthService
{
    private readonly IJwtTokenService _jwtService;
    
    public async Task<AuthResult> AuthenticateAsync(string username, string password)
    {
        var user = await ValidateUserAsync(username, password);
        
        if (user != null)
        {
            // Generate access token with roles and claims
            var accessToken = _jwtService.GenerateToken(
                user.Id.ToString(),
                user.Username,
                user.Roles.Select(r => r.Name),
                new Dictionary<string, string>
                {
                    ["tenant"] = user.TenantId.ToString(),
                    ["permissions"] = string.Join(",", user.Permissions)
                });
            
            // Generate refresh token
            var refreshToken = _jwtService.GenerateRefreshToken();
            
            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _jwtService.GetTokenExpiration(accessToken)
            };
        }
        
        return AuthResult.Failed();
    }
    
    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        if (!_jwtService.ValidateRefreshToken(refreshToken))
            return AuthResult.Failed();
            
        // Refresh token logic...
    }
}
```

### 📨 Event-Driven Architecture

```csharp
// Domain event handling with infrastructure
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IEmailService _emailService;
    
    public async Task HandleAsync(OrderCreatedEvent @event)
    {
        // Send confirmation email
        await _emailService.SendAsync(new Email
        {
            To = @event.CustomerEmail,
            Subject = "Order Confirmation",
            Template = "order-confirmation",
            Data = new { OrderId = @event.OrderId }
        });
        
        // Publish integration event
        await _eventBus.PublishAsync(new OrderProcessingStartedEvent(@event.OrderId));
    }
}

// In-memory event bus implementation
public class InMemoryEventBus : IEventBus
{
    public async Task PublishAsync<T>(T @event) where T : IEvent
    {
        var handlers = _serviceProvider.GetServices<IEventHandler<T>>();
        var tasks = handlers.Select(h => h.HandleAsync(@event));
        await Task.WhenAll(tasks);
    }
}
```

### 🛡️ Security Implementation

```csharp
// Encryption service usage
public class SecureDataService
{
    private readonly IEncryptionService _encryption;
    private readonly IHashingService _hashing;
    
    public async Task<string> SecurePasswordAsync(string password)
    {
        return await _hashing.HashAsync(password);
    }
    
    public async Task<string> EncryptSensitiveDataAsync(string data)
    {
        return await _encryption.EncryptAsync(data);
    }
}

// Key management with Azure Key Vault
services.AddKeyManagement()
    .UseAzureKeyVault(configuration.GetConnectionString("KeyVault"));
```

## 🔌 Service Registration Overview

### 📊 Data Layer Services
- `IDbContext` → `ApplicationDbContext` (Scoped)
- `IRepository<TEntity, TId>` → `Repository<TEntity, TId>` (Scoped)
- `IReadOnlyRepository<TEntity, TId>` → `ReadOnlyRepository<TEntity, TId>` (Scoped)
- `IUnitOfWork` → `UnitOfWork` (Scoped)
- EF Core Interceptors: `AuditInterceptor`, `DomainEventInterceptor`, `SoftDeleteInterceptor`

### 💾 Caching Services
- `ICacheService` → `MemoryCacheService` (Scoped)
- `IDistributedCache` → `DistributedCacheService` (Scoped)
- `RedisCacheService` → Redis implementation (Scoped)
- `CacheKeyGenerator` → Smart key generation (Singleton)
- `CacheConfiguration` → Cache settings (Singleton)

### 🔐 Authentication & Authorization
- `IJwtTokenService` → `JwtTokenService` (Scoped)
- `IApiKeyService` → `ApiKeyService` (Scoped)
- `IOAuthService` → `OAuthService` (Scoped)
- `IAuthorizationService` → `AuthorizationService` (Scoped)
- Authorization Handlers and Policies (Singleton)

### 🛡️ Security Services
- `IEncryptionService` → AES/RSA implementations (Scoped)
- `IHashingService` → PBKDF2/BCrypt implementations (Scoped)
- `IKeyManagementService` → Cloud key management (Scoped)
- `ISecretsService` → Secrets management (Scoped)

### 📁 Storage Services
- `IFileStorageService` → `LocalFileStorageService`/Cloud providers (Scoped)
- `IBlobStorageService` → Blob storage implementations (Scoped)
- `IDocumentStorageService` → Document storage (Scoped)

### 📨 Messaging Services
- `IMessageBus` → `InMemoryMessageBus`/External implementations (Singleton)
- `IEventBus` → `InMemoryEventBus`/External implementations (Singleton)
- `IMessagePublisher` → Publisher implementations (Scoped)
- `IMessageSubscriber` → Subscriber implementations (Scoped)

### 📧 Communication Services
- `IEmailService` → SMTP/SendGrid implementations (Scoped)
- `ISmsService` → Twilio implementation (Scoped)
- `IPushNotificationService` → Firebase implementation (Scoped)
- `INotificationService` → Unified notification service (Scoped)

### 📝 Logging & Monitoring
- `ILoggerService` → `LoggerService` (Scoped)
- `IHealthCheckService` → Health check implementations (Scoped)
- `IMetricsService` → Metrics collection (Scoped)
- `ITracingService` → Distributed tracing (Scoped)

### ⚙️ Configuration & Mapping
- `IConfigurationService` → `ConfigurationService` (Scoped)
- AutoMapper profiles and services (Singleton)
- Mapster configurations (Singleton)

### 🔍 Serialization & Validation
- `IJsonSerializer` → `SystemTextJsonSerializer` (Singleton)
- `IXmlSerializer` → XML serialization (Singleton)
- `IBinarySerializer` → Protobuf/MessagePack (Singleton)
- FluentValidation services and validators

## 🔄 EF Core Interceptors

### AuditInterceptor
- **Purpose**: Automatically tracks creation and modification timestamps
- **Triggers**: On entities implementing `IAuditableEntity`
- **Features**: Sets CreatedAt/CreatedBy and UpdatedAt/UpdatedBy fields
- **Integration**: Requires `ICurrentUserService` for user context

### DomainEventInterceptor  
- **Purpose**: Automatically dispatches domain events from aggregate roots
- **Triggers**: Before SaveChanges on entities with pending domain events
- **Features**: Ensures events are published within the same transaction
- **Integration**: Works with `IDomainEventDispatcher` from domain layer

### SoftDeleteInterceptor
- **Purpose**: Handles soft deletion for entities
- **Triggers**: On entities implementing `ISoftDeletable`
- **Features**: Sets IsDeleted flag and filters deleted entities from queries
- **Integration**: Automatically applies global query filters

## 📦 NuGet Dependencies

This infrastructure library depends on:

### Core Dependencies
- `BuildingBlocks.Domain` - Domain layer abstractions and entities
- `BuildingBlocks.Application` - Application layer interfaces and DTOs
- `Microsoft.Extensions.DependencyInjection` - Dependency injection
- `Microsoft.Extensions.Configuration` - Configuration management

### Data Access
- `Microsoft.EntityFrameworkCore` (9.0+) - ORM framework
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server provider
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory testing

### Caching
- `Microsoft.Extensions.Caching.Memory` - In-memory caching
- `Microsoft.Extensions.Caching.StackExchangeRedis` - Redis caching
- `StackExchange.Redis` - Redis client

### Authentication & Security
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `System.IdentityModel.Tokens.Jwt` - JWT token handling
- `Microsoft.AspNetCore.Cryptography.KeyDerivation` - Password hashing

### Serialization
- `System.Text.Json` - JSON serialization
- `Newtonsoft.Json` - Alternative JSON serialization
- `MessagePack` - Binary serialization
- `CsvHelper` - CSV processing

### Logging & Monitoring
- `Serilog` - Structured logging
- `Serilog.Extensions.Hosting` - Hosting integration
- `Microsoft.ApplicationInsights.AspNetCore` - Application Insights

### Background Processing
- `Hangfire` - Job scheduling
- `Microsoft.Extensions.Hosting` - Background services

## 🏗️ Clean Architecture Integration

```
📁 YourApplication/
├── 📁 Domain/ (BuildingBlocks.Domain)
│   └── Core business logic, entities, value objects
├── 📁 Application/ (BuildingBlocks.Application)  
│   └── Use cases, interfaces, DTOs, CQRS
├── 📁 Infrastructure/ (BuildingBlocks.Infrastructure) ← This Library
│   └── Data access, external services, frameworks
├── 📁 Presentation/ (API/Web/Desktop)
│   └── Controllers, views, API endpoints
└── 📁 Tests/
    ├── 📁 Unit/ → Domain & Application tests
    ├── 📁 Integration/ → Infrastructure tests
    └── 📁 E2E/ → Full application tests
```

### Dependency Flow
```
Presentation → Application → Domain
     ↓
Infrastructure → Application → Domain
```

The Infrastructure layer implements interfaces defined in the Application layer and depends on both Application and Domain layers, following the Dependency Inversion Principle.