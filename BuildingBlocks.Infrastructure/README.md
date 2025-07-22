# BuildingBlocks.Infrastructure

A comprehensive infrastructure layer implementation for .NET applications following Clean Architecture principles. This library provides essential infrastructure services including data access, caching, messaging, authentication, and more.

## Features

### 📊 Data Layer
- **Repository Pattern** with Unit of Work
- **Entity Framework Core** with interceptors for cross-cutting concerns
- **Domain Event Handling** with interceptors
- **Data Seeding and Migrations**
- **Strongly Typed ID Converters** for EF Core

### 💾 Caching
- **Multi-tier Caching** (Memory, Distributed, Redis)
- **Cache Policies** and key management
- **Cache Key Generation** and expiration strategies

### 🔐 Authentication & Security
- **JWT Authentication** services
- **API Key Authentication** support
- **Authorization** services and policies
- **Encryption** and hashing services

### 📁 Storage & Serialization
- **File Storage** abstraction with local implementation
- **JSON Serialization** with System.Text.Json
- **Configuration Management** services
- **Blob Storage** interfaces

### 📨 Messaging
- **In-Memory Message Bus** implementation
- **Event Bus** for domain events
- **Message Publishers** and subscribers

### 📝 Logging & Monitoring
- **Structured Logging** with Serilog support
- **Performance Monitoring** and metrics
- **Health Checks** integration

## Installation

Add the project reference to your infrastructure layer:

```xml
<ProjectReference Include="..\BuildingBlocks.Infrastructure\BuildingBlocks.Infrastructure.csproj" />
```

## Quick Start

Register infrastructure services in `Program.cs`:

```csharp
using BuildingBlocks.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register all infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();
```

## Configuration

Configure `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourApp;Trusted_Connection=true;",
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

## Key Components

### Entity Framework Core Integration

```csharp
// Automatic audit tracking
public class Product : Entity<Guid>, IAuditableEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    // Automatically populated by AuditInterceptor
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

// Soft delete support
public class Category : Entity<Guid>, ISoftDeletable
{
    public string Name { get; set; }
    
    // Handled by SoftDeleteInterceptor
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
}
```

### Repository Pattern

```csharp
public class ProductService
{
    private readonly IRepository<Product, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Product> CreateProductAsync(Product product)
    {
        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product;
    }
}
```

### Caching Services

```csharp
public class CachedProductService
{
    private readonly ICacheService _cache;
    private readonly IRepository<Product, Guid> _repository;
    
    public async Task<Product> GetProductAsync(Guid id)
    {
        var cacheKey = new CacheKey($"product:{id}", TimeSpan.FromHours(1));
        var product = await _cache.GetAsync<Product>(cacheKey);
        
        if (product == null)
        {
            product = await _repository.GetByIdAsync(id);
            if (product != null)
            {
                await _cache.SetAsync(cacheKey, product);
            }
        }
        
        return product;
    }
}
```

### JWT Authentication

```csharp
public class AuthService
{
    private readonly IJwtTokenService _jwtService;
    
    public async Task<string> AuthenticateAsync(string username, string password)
    {
        var user = await ValidateUserAsync(username, password);
        
        if (user != null)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            
            return _jwtService.GenerateToken(claims);
        }
        
        return null;
    }
}
```

## Registered Services

### Data Layer
- `IDbContext` → `ApplicationDbContext`
- `IRepository<TEntity, TId>` → `Repository<TEntity, TId>`
- `IReadOnlyRepository<TEntity, TId>` → `ReadOnlyRepository<TEntity, TId>`
- `IUnitOfWork` → `UnitOfWork`

### Caching
- `ICacheService` → `MemoryCacheService`
- `DistributedCacheService`, `RedisCacheService`
- `CacheKeyGenerator`, `CacheConfiguration`

### Authentication & Security
- `IJwtTokenService` → `JwtTokenService`
- `IEncryptionService` → Implementation based on configuration

### Storage & Serialization
- `IFileStorageService` → `LocalFileStorageService`
- `IJsonSerializer` → `SystemTextJsonSerializer`
- `IBlobStorageService` → Implementation based on configuration

### Configuration & Logging
- `IConfigurationService` → `ConfigurationService`
- `ILoggerService` → `LoggerService`

## EF Core Interceptors

### AuditInterceptor
Automatically tracks creation and modification timestamps for entities implementing `IAuditableEntity`.

### DomainEventInterceptor
Automatically dispatches domain events from aggregate roots before saving changes.

### SoftDeleteInterceptor
Handles soft deletion for entities implementing `ISoftDeletable`.

## Dependencies

This package depends on:
- `BuildingBlocks.Domain` - Domain layer abstractions
- `BuildingBlocks.Application` - Application layer interfaces
- Entity Framework Core 9.0+
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Caching.Memory
- System.Text.Json

## Architecture Integration

```
📁 YourApplication/
├── 📁 Domain/ (BuildingBlocks.Domain)
├── 📁 Application/ (BuildingBlocks.Application)
├── 📁 Infrastructure/ (BuildingBlocks.Infrastructure) ← This library
├── 📁 API/
└── 📁 Tests/
```