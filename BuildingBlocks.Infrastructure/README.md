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
- **Authentication Middleware** with token validation
- **Identity Management** services and abstractions

### 📝 **Logging & Observability** ✨ NEW
- **Advanced Data Redaction** for sensitive information with configurable patterns
- **OpenTelemetry Complete Integration** with logging, metrics, and distributed tracing
- **Environment-Specific Configuration** (Development/Production)
- **Structured Logging** with enrichers and correlation IDs
- **Performance Monitoring** with comprehensive metrics collection
- **Health Checks** for system components and dependencies
- **Custom Log Enrichers** for correlation IDs and user context

### 📁 Storage Solutions
- **File Storage Abstraction** with local file storage implementation
- **Document Storage** services for structured documents

### 📨 Messaging & Communication
- **Message Bus Pattern** with in-memory implementation
- **Event Bus** for domain event propagation
- **Publisher/Subscriber** pattern implementation
- **Message Serialization** with pluggable serializers
- **Email Services** (SMTP) with template support

### 🔧 Configuration & Mapping
- **Configuration Management** with validation and providers
- **Settings Classes** for typed configuration access
- **Environment-Specific Configurations** (Development/Production)

### 🔍 Validation & Serialization
- **JSON Serialization** (System.Text.Json)
- **XML and Binary Serialization** support
- **Validation Services** integration

### 🌐 External Integrations
- **HTTP Client Services** with configuration and policies
- **Third-Party API** integration abstractions

## 📋 Current Directory Structure

```
BuildingBlocks.Infrastructure/
├── 📁 Authentication/          # JWT authentication services
│   └── 🔑 JWT/                # JWT token services and configuration
├── 📁 Authorization/           # Authorization policies and handlers (placeholder)
├── 📁 BackgroundServices/     # Background processing infrastructure (placeholder)
├── 📁 Caching/                # Multi-tier caching implementation
│   ├── 🧠 MemoryCacheService.cs      # In-memory caching
│   ├── 🌐 DistributedCacheService.cs # Distributed caching
│   ├── 🔴 RedisCacheService.cs       # Redis implementation
│   ├── 🔑 CacheKeyGenerator.cs       # Smart cache key management
│   └── ⚙️ CacheConfiguration.cs      # Cache configuration
├── 📁 Communication/          # External communication services
│   └── 📧 Email/              # Email services (SMTP)
│       ├── SmtpEmailService.cs
│       ├── EmailTemplateService.cs
│       └── EmailConfiguration.cs
├── 📁 Configuration/          # Configuration management
│   ├── ⚙️ ConfigurationService.cs    # Configuration service
│   ├── 🔧 DevelopmentHostEnvironment.cs # Development settings
│   └── 🏭 ProductionHostEnvironment.cs  # Production settings
├── 📁 Data/                   # Data access infrastructure
│   ├── 🗄️ Context/            # EF Core DbContext implementations
│   ├── 🔄 Interceptors/       # EF Core interceptors
│   │   ├── AuditInterceptor.cs
│   │   ├── DomainEventInterceptor.cs
│   │   └── SoftDeleteInterceptor.cs
│   ├── 📦 Repositories/       # Repository pattern implementations
│   ├── 🔄 UnitOfWork/         # Unit of Work pattern
│   ├── 🌱 Seeding/            # Data seeding framework
│   ├── 📈 Migrations/         # Database migration management
│   ├── ⚙️ Configurations/     # Entity configurations
│   └── 🔄 Converters/         # Value converters for strongly-typed IDs
├── 📁 Extensions/             # Service registration and configuration
│   ├── 🔧 ServiceRegistration.cs     # Main service registration
│   ├── 🏗️ ServiceCollectionExtensions.cs # DI extensions
│   ├── 📊 CachingExtensions.cs       # Caching setup
│   ├── 🗄️ DatabaseExtensions.cs     # Database setup
│   └── 🏗️ ApplicationBuilderExtensions.cs # Pipeline setup
├── 📁 External/               # External service integrations
│   └── 🌐 HttpClients/        # HTTP client services
├── 📁 Logging/ ✨ NEW          # Advanced logging and redaction
│   ├── 🔒 IDataRedactionService.cs   # Data redaction interface
│   ├── 🔒 RedactionOptions.cs        # Comprehensive redaction config
│   ├── 🔒 RedactionLoggerProvider.cs # Custom logger provider
│   ├── 🔒 RedactionLogProcessor.cs   # Log processing
│   ├── 📖 RedactionExamples.cs       # Usage examples
│   ├── 📝 README_Redaction.md        # Detailed redaction docs
│   ├── 📊 ILoggerService.cs          # Logger service interface
│   └── 📊 LoggerService.cs           # Logger service implementation
├── 📁 Messaging/              # Messaging infrastructure
│   ├── 📨 MessageBus/         # Message bus implementations
│   │   ├── InMemoryMessageBus.cs
│   │   └── IMessageBus.cs
│   ├── 📡 EventBus/           # Event bus for domain events
│   ├── 📤 Publishers/         # Message publishers
│   ├── 📥 Subscribers/        # Message subscribers
│   ├── 🔄 Serialization/      # Message serialization
│   └── ⚙️ Configuration/      # Messaging configuration
├── 📁 Monitoring/             # System monitoring and health
│   └── 💓 Health/             # Health check implementations
├── 📁 Observability/ ✨ NEW    # OpenTelemetry observability
│   └── 📊 OpenTelemetryExtensions.cs # Complete OpenTelemetry setup
├── 📁 Security/               # Security services
│   └── 🔐 Encryption/         # Encryption services
├── 📁 Serialization/          # Serialization services
│   ├── 📄 Json/               # JSON serialization
│   ├── 📋 Xml/                # XML serialization
│   └── 📦 Binary/             # Binary serialization
├── 📁 Services/               # Shared services (placeholder)
├── 📁 Storage/                # Storage abstractions and implementations
│   ├── 📁 Files/              # File storage services
│   │   ├── LocalFileStorageService.cs
│   │   └── IFileStorageService.cs
│   └── 🗂️ Blobs/              # Blob storage services
└── 📁 Validation/             # Validation services
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
builder.Services.AddInfrastructureServices(builder.Configuration);

// Or register specific service groups
builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddCachingServices(builder.Configuration);
builder.Services.AddMessagingServices();
builder.Services.AddAuthenticationServices(builder.Configuration);

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
    "Provider": "Memory"
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
    "Provider": "Local",
    "LocalPath": "./uploads"
  },
  "Messaging": {
    "Provider": "InMemory"
  },
  "Email": {
    "Provider": "Smtp",
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "Username": "",
      "Password": ""
    }
  },
  "OpenTelemetry": {
    "ServiceName": "YourApp",
    "ServiceVersion": "1.0.0",
    "Redaction": {
      "Enabled": true,
      "RedactionText": "[REDACTED]",
      "Mode": "Full",
      "SensitiveFields": [
        "password",
        "secret",
        "token",
        "apikey",
        "authorization",
        "creditcard",
        "ssn",
        "email",
        "phone"
      ]
    }
  }
}
```

## 🔧 Key Usage Examples

### 📊 Entity Framework Core with Interceptors

```csharp
// Entity with audit and soft delete support
public class Product : Entity<ProductId>, IAuditableEntity, ISoftDeletable
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    
    // Audit properties (populated by AuditInterceptor)
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Soft delete properties (handled by SoftDeleteInterceptor)
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public Product(ProductId id, string name, decimal price) : base(id)
    {
        Name = name;
        Price = price;
        
        // Domain events handled by DomainEventInterceptor
        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price));
    }
}

// Repository with caching
public class ProductRepository : Repository<Product, ProductId>, IProductRepository
{
    private readonly ICacheService _cache;

    public ProductRepository(ApplicationDbContext context, ICacheService cache) 
        : base(context)
    {
        _cache = cache;
    }

    public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"product:name:{name}";
        var cached = await _cache.GetAsync<Product>(cacheKey, cancellationToken);
        
        if (cached != null) return cached;

        var product = await Context.Products
            .Where(p => p.Name == name)
            .FirstOrDefaultAsync(cancellationToken);

        if (product != null)
        {
            await _cache.SetAsync(cacheKey, product, cancellationToken: cancellationToken);
        }

        return product;
    }
}
```

### 💾 Multi-Tier Caching

```csharp
public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;

    public async Task<Product?> GetProductAsync(ProductId id)
    {
        // Cache automatically handles L1 (Memory) -> L2 (Distributed/Redis) -> Database
        var cacheKey = id.Value.ToString();
        var cached = await _cache.GetAsync<Product>(cacheKey);
        
        if (cached != null) return cached;

        var product = await _repository.GetByIdAsync(id);
        if (product != null)
        {
            await _cache.SetAsync(cacheKey, product);
        }

        return product;
    }
}
```

### 🔐 JWT Authentication

```csharp
public class AuthService
{
    private readonly IJwtTokenService _jwtService;
    
    public async Task<AuthResult> AuthenticateAsync(string username, string password)
    {
        var user = await ValidateUserAsync(username, password);
        
        if (user != null)
        {
            var accessToken = _jwtService.GenerateAccessToken(
                user.Id.ToString(),
                user.Username,
                user.Roles.Select(r => r.Name),
                new Dictionary<string, string>
                {
                    ["tenant"] = user.TenantId.ToString()
                });
            
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id.ToString());
            
            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = _jwtService.GetTokenExpiration(accessToken)
            };
        }
        
        return AuthResult.Failed();
    }
}
```

### 📨 Event-Driven Messaging

```csharp
// Domain event handler
public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IMessageBus _messageBus;

    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Send notification email
        await _emailService.SendAsync(new EmailMessage
        {
            To = "admin@company.com",
            Subject = "New Product Created",
            Template = "product-created",
            Data = new { ProductName = @event.ProductName, Price = @event.Price }
        }, cancellationToken);
        
        // Publish integration event
        await _messageBus.PublishAsync(new ProductCreatedIntegrationEvent(@event.ProductId), cancellationToken);
    }
}
```

### 📝 Data Redaction for Logging ✨ NEW

```csharp
// Automatic redaction in logs
public class UserService
{
    private readonly ILogger<UserService> _logger;

    public async Task CreateUserAsync(CreateUserRequest request)
    {
        // This log will automatically redact sensitive fields like password, email, etc.
        _logger.LogInformation("Creating user: {@Request}", request);
        
        // Advanced redaction with custom patterns
        _logger.LogInformation("Processing payment for user {UserId} with card {CardNumber}", 
            request.UserId, request.CreditCard); // Credit card will be redacted
    }
}

// Configure redaction patterns
services.Configure<RedactionOptions>(options =>
{
    options.Enabled = true;
    options.RedactionText = "[CONFIDENTIAL]";
    options.Mode = RedactionMode.Partial; // Show first/last characters
    options.SensitiveFields.AddRange(new[] { "password", "ssn", "creditcard" });
    
    // Custom regex patterns
    options.RegexPatterns.Add(new RegexRedactionRule
    {
        Pattern = @"\b\d{4}[-\s]?\d{4}[-\s]?\d{4}[-\s]?\d{4}\b",
        Replacement = "[CARD-REDACTED]"
    });
});
```

## 🔌 Service Registration Overview

### Currently Implemented Services

#### 📊 Data Layer Services
- `ApplicationDbContext` with interceptors (Scoped)
- `IRepository<TEntity, TId>` → `Repository<TEntity, TId>` (Scoped)
- `IUnitOfWork` → `UnitOfWork` (Scoped)
- EF Core Interceptors: `AuditInterceptor`, `DomainEventInterceptor`, `SoftDeleteInterceptor`

#### 💾 Caching Services
- `ICacheService` → `MemoryCacheService` / `DistributedCacheService` / `RedisCacheService` (Scoped)
- `ICacheKeyGenerator` → `CacheKeyGenerator` (Singleton)
- `CacheConfiguration` (Singleton)

#### 🔐 Authentication Services
- `IJwtTokenService` → `JwtTokenService` (Scoped)
- JWT Authentication Middleware

#### 📝 Logging & Observability Services ✨ NEW
- `IDataRedactionService` → `DataRedactionService` (Singleton)
- OpenTelemetry with Logging, Metrics, and Tracing
- `ILoggerService` → `LoggerService` (Scoped)
- Redaction Logger Provider with configurable patterns

#### 📁 Storage Services
- `IFileStorageService` → `LocalFileStorageService` (Scoped)

#### 📨 Messaging Services
- `IMessageBus` → `InMemoryMessageBus` (Singleton)

#### 📧 Communication Services
- `IEmailService` → `SmtpEmailService` (Scoped)
- `IEmailTemplateService` → `EmailTemplateService` (Scoped)

#### ⚙️ Configuration Services
- `IConfigurationService` → `ConfigurationService` (Scoped)
- Environment-specific configurations (Development/Production)

#### 🌐 External Services
- HTTP Client services with configuration

#### 💓 Health Check Services
- Comprehensive health checks for database, cache, and external services

## 🔄 EF Core Interceptors

### AuditInterceptor
- **Purpose**: Automatically tracks creation and modification timestamps
- **Triggers**: On entities implementing `IAuditableEntity`
- **Features**: Sets CreatedAt/CreatedBy and UpdatedAt/UpdatedBy fields

### DomainEventInterceptor  
- **Purpose**: Automatically dispatches domain events from aggregate roots
- **Triggers**: Before SaveChanges on entities with pending domain events
- **Features**: Ensures events are published within the same transaction

### SoftDeleteInterceptor
- **Purpose**: Handles soft deletion for entities
- **Triggers**: On entities implementing `ISoftDeletable`
- **Features**: Sets IsDeleted flag and filters deleted entities from queries

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

## 📚 Additional Documentation

For detailed information about specific features:

- **Data Redaction**: See `Logging/README_Redaction.md` for comprehensive redaction configuration and examples
- **Service Registration**: See `Extensions/README.md` for detailed service registration patterns
- **OpenTelemetry**: See the `Observability/` directory for advanced observability setup

## 🚧 Planned Features

Future implementations may include:
- Advanced authorization policies and handlers
- Multiple storage providers (Azure Blob, AWS S3)
- Additional messaging providers (Service Bus, RabbitMQ)
- SMS and push notification services
- Key management and secrets services
- Enhanced security services (encryption, hashing)
- Background job processing

---

*This library follows Clean Architecture principles and provides a solid foundation for building scalable, maintainable .NET applications.*