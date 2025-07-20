# BuildingBlocks.Infrastructure

A comprehensive infrastructure layer implementation for .NET applications following Clean Architecture principles. This library provides essential infrastructure services including data access, caching, messaging, authentication, and more.

## 🏗️ Architecture Overview

This infrastructure layer implements the following patterns and components:

- **Repository Pattern** with Unit of Work
- **Entity Framework Core** with interceptors for cross-cutting concerns
- **Multi-tier Caching** (Memory, Distributed, Redis)
- **Domain Event Handling** with interceptors
- **JWT Authentication** services
- **File Storage** abstraction
- **Serialization** services
- **Configuration Management**
- **Structured Logging**
- **Data Seeding and Migrations**

## 📦 Installation

Add the package reference to your project:

```xml
<PackageReference Include="BuildingBlocks.Infrastructure" Version="1.0.0" />
```

## 🚀 Quick Start

### 1. Register Infrastructure Services

In your `Program.cs` or `Startup.cs`:

```csharp
using BuildingBlocks.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register all infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();
```

### 2. Configure appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourApp;Trusted_Connection=true;TrustServerCertificate=true;",
    "Redis": "localhost:6379"
  },
  "Cache": {
    "KeyPrefix": "yourapp:",
    "DefaultExpiration": "00:30:00"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-must-be-at-least-32-characters-long",
    "Issuer": "YourApp",
    "Audience": "YourApp-API",
    "ExpiryMinutes": "60"
  }
}
```

### 3. Use in Your Application

```csharp
// Inject and use repository
public class ProductService
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    
    public ProductService(
        IRepository<Product, Guid> productRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }
    
    public async Task<Product> GetProductAsync(Guid id)
    {
        var cacheKey = new CacheKey($"product:{id}", TimeSpan.FromMinutes(30));
        
        var product = await _cacheService.GetAsync<Product>(cacheKey);
        if (product != null) return product;
        
        product = await _productRepository.GetByIdAsync(id);
        if (product != null)
        {
            await _cacheService.SetAsync(cacheKey, product);
        }
        
        return product;
    }
}
```

## 🔧 Service Registration Options

### Complete Registration
```csharp
// Registers all infrastructure services
services.AddInfrastructureServices(configuration);
```

### Selective Registration
```csharp
// Register only specific service groups
services.AddDataServices(configuration);
services.AddCachingServices(configuration);
services.AddMessagingServices();
services.AddAuthenticationServices(configuration);
services.AddStorageServices();
services.AddSerializationServices();
services.AddConfigurationServices();
services.AddLoggingServices();
```

## 📊 Data Layer

### Entity Framework Core with Interceptors

```csharp
// Automatic audit tracking
public class Product : Entity<Guid>, IAuditableEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    // These are automatically populated by AuditInterceptor
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

// Soft delete support
public class Category : Entity<Guid>, ISoftDeletable
{
    public string Name { get; set; }
    
    // Automatically handled by SoftDeleteInterceptor
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    
    public void Delete() => IsDeleted = true;
}

// Domain events
public class Order : AggregateRoot<Guid>
{
    public void CompleteOrder()
    {
        Status = OrderStatus.Completed;
        // Domain event automatically dispatched by DomainEventInterceptor
        AddDomainEvent(new OrderCompletedEvent(Id));
    }
}
```

### Repository Pattern

```csharp
// Generic repository usage
public class ProductService
{
    private readonly IRepository<Product, Guid> _repository;
    
    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _repository
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
    
    public async Task<Product> CreateProductAsync(Product product)
    {
        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product;
    }
}
```

## 💾 Caching

### Multi-tier Caching Support

```csharp
public class CachedProductService
{
    private readonly ICacheService _cache;
    private readonly IRepository<Product, Guid> _repository;
    
    public async Task<Product> GetProductAsync(Guid id)
    {
        // Try cache first
        var cacheKey = new CacheKey($"product:{id}", TimeSpan.FromHours(1));
        var product = await _cache.GetAsync<Product>(cacheKey);
        
        if (product == null)
        {
            // Load from database
            product = await _repository.GetByIdAsync(id);
            
            if (product != null)
            {
                // Cache with policy
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
```

### Cache Implementations
- **MemoryCacheService**: In-memory caching (default)
- **DistributedCacheService**: Distributed caching
- **RedisCacheService**: Redis-based caching (when Redis is configured)

## 📨 Messaging & Domain Events

```csharp
// Domain event handling
public class OrderCompletedEventHandler : IEventHandler<OrderCompletedEvent>
{
    public async Task HandleAsync(OrderCompletedEvent domainEvent)
    {
        // Send email notification
        // Update inventory
        // Create invoice
    }
}

// Register domain event handlers
services.AddScoped<IEventHandler<OrderCompletedEvent>, OrderCompletedEventHandler>();
```

## 🔐 Authentication

### JWT Token Service

```csharp
public class AuthService
{
    private readonly IJwtTokenService _jwtService;
    
    public async Task<string> AuthenticateAsync(string username, string password)
    {
        // Validate credentials
        var user = await ValidateUserAsync(username, password);
        
        if (user != null)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            
            return _jwtService.GenerateToken(claims);
        }
        
        return null;
    }
}
```

## 📁 File Storage

```csharp
public class DocumentService
{
    private readonly IFileStorageService _fileStorage;
    
    public async Task<string> UploadDocumentAsync(Stream fileStream, string fileName)
    {
        var filePath = $"documents/{Guid.NewGuid()}/{fileName}";
        await _fileStorage.SaveAsync(filePath, fileStream);
        return filePath;
    }
    
    public async Task<Stream> GetDocumentAsync(string filePath)
    {
        return await _fileStorage.GetAsync(filePath);
    }
}
```

## 🔧 Configuration

```csharp
public class SettingsService
{
    private readonly IConfigurationService _config;
    
    public async Task<T> GetSettingAsync<T>(string key, T defaultValue = default)
    {
        return await _config.GetValueAsync(key, defaultValue);
    }
    
    public async Task SetSettingAsync<T>(string key, T value)
    {
        await _config.SetValueAsync(key, value);
    }
}
```

## 📝 Logging

```csharp
public class OrderService
{
    private readonly ILoggerService _logger;
    
    public async Task ProcessOrderAsync(Order order)
    {
        _logger.LogInformation("Processing order {OrderId}", order.Id);
        
        try
        {
            // Process order
            await ProcessOrderInternal(order);
            
            _logger.LogInformation("Order {OrderId} processed successfully", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process order {OrderId}", order.Id);
            throw;
        }
    }
}
```

## 🗄️ Database Migrations & Seeding

```csharp
// In Program.cs
var app = builder.Build();

// Run migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    await migrationRunner.MigrateAsync();
    
    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await dataSeeder.SeedAsync();
}
```

## 🏗️ Registered Services

### Data Layer
- `IDbContext` → `ApplicationDbContext`
- `IRepository<TEntity, TId>` → `Repository<TEntity, TId>`
- `IReadOnlyRepository<TEntity, TId>` → `ReadOnlyRepository<TEntity, TId>`
- `IUnitOfWork` → `UnitOfWork`
- `IMigrationRunner` → `MigrationRunner`
- `IDataSeeder` → `DataSeederBase`

### Caching
- `ICacheService` → `MemoryCacheService` (primary)
- `MemoryCacheService`, `DistributedCacheService`, `RedisCacheService`
- `CacheKeyGenerator`, `CacheConfiguration`

### Messaging
- `InMemoryMessageBus`
- `IDomainEventService` → `DomainEventService`

### Authentication
- `IJwtTokenService` → `JwtTokenService`
- `JwtConfiguration`

### Storage & Serialization
- `IFileStorageService` → `LocalFileStorageService`
- `IJsonSerializer` → `SystemTextJsonSerializer`

### Configuration & Logging
- `IConfigurationService` → `ConfigurationService`
- `ILoggerService` → `LoggerService`

## 🔧 Interceptors

### AuditInterceptor
Automatically tracks creation and modification timestamps and user information for entities implementing `IAuditableEntity`.

### DomainEventInterceptor
Automatically dispatches domain events from aggregate roots before saving changes to the database.

### SoftDeleteInterceptor
Handles soft deletion for entities implementing `ISoftDeletable` by calling the `Delete()` method instead of actually removing records.

## 📋 Requirements

- .NET 9.0+
- Entity Framework Core 9.0+
- Microsoft.Extensions.DependencyInjection
- Optional: Redis (for distributed caching)
- Optional: SQL Server (or other EF Core supported database)

## 🔄 Dependencies

This package depends on:
- `BuildingBlocks.Domain` - Domain layer with entities, value objects, and domain events
- `BuildingBlocks.Application` - Application layer with interfaces and services

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.