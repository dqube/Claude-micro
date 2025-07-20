# Infrastructure Service Registration

## Overview

The `ServiceRegistration` class provides extension methods to register all infrastructure dependencies in your DI container.

## Usage

### Basic Registration

```csharp
using BuildingBlocks.Infrastructure.Extensions;

// In Program.cs or Startup.cs
services.AddInfrastructureServices(configuration);
```

### Individual Service Registration

You can also register individual service groups:

```csharp
// Data layer (EF Core, repositories, interceptors)
services.AddDataServices(configuration);

// Caching (Memory, Redis, Distributed)
services.AddCachingServices(configuration);

// Messaging (Message bus, domain events)
services.AddMessagingServices();

// Authentication (JWT)
services.AddAuthenticationServices(configuration);

// Storage (File storage)
services.AddStorageServices();

// Serialization (JSON)
services.AddSerializationServices();

// Configuration
services.AddConfigurationServices();

// Logging
services.AddLoggingServices();
```

## Configuration Requirements

### Database Connection

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourApp;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Redis Cache (Optional)

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

### Cache Settings

```json
{
  "Cache": {
    "KeyPrefix": "yourapp:",
    "DefaultExpiration": "00:30:00"
  }
}
```

### JWT Configuration

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-must-be-at-least-32-characters-long",
    "Issuer": "YourApp",
    "Audience": "YourApp-API",
    "ExpiryMinutes": "60"
  }
}
```

## Registered Services

### Data Layer
- `IDbContext` → `ApplicationDbContext`
- `IRepository<TEntity, TId>` → `Repository<TEntity, TId>`
- `IReadOnlyRepository<TEntity, TId>` → `ReadOnlyRepository<TEntity, TId>`
- `IUnitOfWork` → `UnitOfWork`
- `IMigrationRunner` → `MigrationRunner`
- `IDataSeeder` → `DataSeederBase`
- EF Core Interceptors: `AuditInterceptor`, `DomainEventInterceptor`, `SoftDeleteInterceptor`

### Caching
- `ICacheService` → `MemoryCacheService` (primary)
- `MemoryCacheService`, `DistributedCacheService`, `RedisCacheService` (specific implementations)
- `CacheKeyGenerator`
- `CacheConfiguration`

### Messaging
- `InMemoryMessageBus`
- `IDomainEventService` → `DomainEventService`

### Authentication
- `IJwtTokenService` → `JwtTokenService`
- `JwtConfiguration`

### Storage
- `IFileStorageService` → `LocalFileStorageService`

### Serialization
- `IJsonSerializer` → `SystemTextJsonSerializer`

### Configuration
- `IConfigurationService` → `ConfigurationService`

### Logging
- `ILoggerService` → `LoggerService`

## Notes

- All services are registered with appropriate lifetimes (Singleton, Scoped, Transient)
- EF Core interceptors are automatically configured when DbContext is registered
- Redis caching falls back to in-memory distributed cache if Redis connection is not configured
- The registration handles interface ambiguity by using concrete implementations where needed