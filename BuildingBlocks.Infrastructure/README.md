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
- **OpenTelemetry** complete observability with logging, metrics, and distributed tracing
- **Structured Logging** with enrichers and correlation IDs
- **Application Insights** integration for telemetry
- **Performance Monitoring** with comprehensive metrics collection
- **Health Checks** for system components and dependencies
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
│   ├── 📊 OpenTelemetry/      # OpenTelemetry configuration and instrumentation
│   ├── 📊 ApplicationInsights/# Application Insights integration
│   └── 🏗️ Structured/         # Structured logging utilities
├── 📁 Observability/          # Observability infrastructure
│   ├── 📊 OpenTelemetryExtensions.cs # Complete OpenTelemetry configuration
│   ├── 📈 MetricsService.cs   # Custom metrics service
│   └── 🔍 TracingService.cs   # Distributed tracing utilities
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

## 🔧 Comprehensive Usage Examples

### 📊 Entity Framework Core Integration

#### Entity Configuration with Value Objects and Strongly Typed IDs

```csharp
// Entity with full audit and domain event support
public class Product : Entity<ProductId>, IAuditableEntity, ISoftDeletable
{
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public ProductCode Code { get; private set; }
    
    // Audit properties (populated by AuditInterceptor)
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Soft delete properties (handled by SoftDeleteInterceptor)
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public Product(ProductId id, string name, Money price, CategoryId categoryId, ProductCode code) 
        : base(id)
    {
        Name = Guard.Against.NullOrWhiteSpace(name);
        Price = price ?? throw new ArgumentNullException(nameof(price));
        CategoryId = categoryId ?? throw new ArgumentNullException(nameof(categoryId));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        
        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price));
    }

    public void UpdatePrice(Money newPrice)
    {
        var oldPrice = Price;
        Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
        AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newPrice));
    }

    protected Product() { } // For EF Core
}

// Entity configuration with value converters
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        // Strongly typed ID conversion
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new ProductId(value));
                
        builder.Property(p => p.CategoryId)
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value));

        // Value object configuration
        builder.OwnsOne(p => p.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(p => p.Code, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("ProductCode")
                .HasMaxLength(20);
        });

        // Audit configuration
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(100);
        builder.Property(p => p.UpdatedAt);
        builder.Property(p => p.UpdatedBy).HasMaxLength(100);

        // Soft delete configuration with global filter
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(p => !p.IsDeleted);
        builder.HasIndex(p => p.IsDeleted);

        // Additional configurations
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(p => p.Name);
    }
}

// ApplicationDbContext with comprehensive setup
public class ApplicationDbContext : DbContextBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDomainEventDispatcher eventDispatcher) 
        : base(options)
    {
        _currentUserService = currentUserService;
        _eventDispatcher = eventDispatcher;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Apply strongly typed ID converters
        modelBuilder.ApplyStronglyTypedIdConverters();
        
        // Configure audit fields globally
        modelBuilder.ConfigureAuditFields();
        
        // Configure soft delete globally
        modelBuilder.ConfigureSoftDelete();
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add interceptors for cross-cutting concerns
        optionsBuilder.AddInterceptors(
            new AuditInterceptor(_currentUserService),
            new DomainEventInterceptor(_eventDispatcher),
            new SoftDeleteInterceptor());
            
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Process domain events before saving
        await DispatchDomainEventsAsync();
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _eventDispatcher.DispatchAsync(domainEvent);
        }
    }
}
```

### 📦 Advanced Repository Pattern Implementation

#### Repository with Specifications and Caching

```csharp
public class ProductRepository : Repository<Product, ProductId>, IProductRepository
{
    private readonly ICacheService _cache;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(
        ApplicationDbContext context,
        ICacheService cache,
        ILogger<ProductRepository> logger) 
        : base(context)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Product?> GetByCodeAsync(ProductCode code, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyGenerator.Generate<Product>("code", code.Value);
        var cachedProduct = await _cache.GetAsync<Product>(cacheKey, cancellationToken);
        
        if (cachedProduct != null)
        {
            _logger.LogDebug("Product {Code} found in cache", code.Value);
            return cachedProduct;
        }

        var product = await Context.Products
            .Where(p => p.Code.Value == code.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (product != null)
        {
            var cachePolicy = new CachePolicy
            {
                AbsoluteExpiration = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            };
            await _cache.SetAsync(cacheKey, product, cachePolicy, cancellationToken);
            _logger.LogDebug("Product {Code} cached for future requests", code.Value);
        }

        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(
        CategoryId categoryId, 
        CancellationToken cancellationToken = default)
    {
        var spec = new ProductsByCategorySpecification(categoryId)
            .And(new ActiveProductsSpecification());
            
        return await FindAsync(spec, cancellationToken);
    }

    public async Task<PagedResult<Product>> SearchProductsAsync(
        string searchTerm, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var spec = new ProductSearchSpecification(searchTerm)
            .WithPaging(page, pageSize)
            .WithOrdering(p => p.Name);
            
        var products = await FindAsync(spec, cancellationToken);
        var totalCount = await CountAsync(new ProductSearchSpecification(searchTerm), cancellationToken);
        
        return new PagedResult<Product>(products, totalCount, page, pageSize);
    }

    public override async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);
        
        // Invalidate cache
        var cacheKey = CacheKeyGenerator.Generate<Product>("code", entity.Code.Value);
        await _cache.RemoveAsync(cacheKey, cancellationToken);
        
        _logger.LogDebug("Cache invalidated for product {Code}", entity.Code.Value);
    }
}

// Product-specific repository interface
public interface IProductRepository : IRepository<Product, ProductId>
{
    Task<Product?> GetByCodeAsync(ProductCode code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> SearchProductsAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
}

// Advanced specifications with complex business logic
public class ProductSearchSpecification : Specification<Product>
{
    public ProductSearchSpecification(string searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            Criteria = p => p.Name.ToLower().Contains(lowerSearchTerm) ||
                           p.Code.Value.ToLower().Contains(lowerSearchTerm);
        }
        
        // Include related data
        AddInclude(p => p.Category);
        
        // Default ordering
        ApplyOrderBy(p => p.Name);
    }
}

public class ProductsByCategorySpecification : Specification<Product>
{
    public ProductsByCategorySpecification(CategoryId categoryId)
    {
        Criteria = p => p.CategoryId == categoryId;
        AddInclude(p => p.Category);
    }
}

public class ActiveProductsSpecification : Specification<Product>
{
    public ActiveProductsSpecification()
    {
        Criteria = p => !p.IsDeleted && p.Price.Amount > 0;
    }
}

public class ExpensiveProductsSpecification : Specification<Product>
{
    public ExpensiveProductsSpecification(decimal threshold, string currency = "USD")
    {
        Criteria = p => p.Price.Amount >= threshold && p.Price.Currency == currency;
    }
}

// Service using repository with Unit of Work pattern
public class ProductManagementService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<ProductManagementService> _logger;

    public ProductManagementService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher,
        ILogger<ProductManagementService> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<Result<ProductId>> CreateProductAsync(
        string name, 
        decimal price, 
        string currency, 
        CategoryId categoryId, 
        string productCode)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return Result<ProductId>.Failure("Category not found");
            }

            // Check if product code is unique
            var existingProduct = await _productRepository.GetByCodeAsync(new ProductCode(productCode));
            if (existingProduct != null)
            {
                return Result<ProductId>.Failure("Product code already exists");
            }

            // Create product
            var product = new Product(
                ProductId.New(),
                name,
                new Money(price, currency),
                categoryId,
                new ProductCode(productCode));

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Dispatch domain events
            await _eventDispatcher.DispatchEventsAsync(product.DomainEvents);
            product.ClearDomainEvents();

            await _unitOfWork.CommitTransactionAsync();
            
            _logger.LogInformation("Product {ProductId} created successfully", product.Id);
            return Result<ProductId>.Success(product.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating product");
            throw;
        }
    }

    public async Task<Result> UpdateProductPriceAsync(ProductId productId, decimal newPrice, string currency)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return Result.Failure("Product not found");
            }

            var oldPrice = product.Price;
            product.UpdatePrice(new Money(newPrice, currency));

            await _productRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Dispatch domain events
            await _eventDispatcher.DispatchEventsAsync(product.DomainEvents);
            product.ClearDomainEvents();

            await _unitOfWork.CommitTransactionAsync();
            
            _logger.LogInformation("Product {ProductId} price updated from {OldPrice} to {NewPrice}", 
                productId, oldPrice, product.Price);
                
            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating product price for {ProductId}", productId);
            throw;
        }
    }

    public async Task<PagedResult<ProductDto>> SearchProductsAsync(ProductSearchQuery query)
    {
        var specification = new ProductSearchSpecification(query.SearchTerm);
        
        if (query.CategoryId.HasValue)
        {
            specification = specification.And(new ProductsByCategorySpecification(query.CategoryId.Value));
        }
        
        if (query.MinPrice.HasValue)
        {
            specification = specification.And(new ExpensiveProductsSpecification(query.MinPrice.Value, query.Currency ?? "USD"));
        }
        
        specification = specification.WithPaging(query.Page, query.PageSize);
        
        var products = await _productRepository.FindAsync(specification);
        var totalCount = await _productRepository.CountAsync(specification.WithoutPaging());
        
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id.Value,
            Name = p.Name,
            Price = p.Price.Amount,
            Currency = p.Price.Currency,
            ProductCode = p.Code.Value,
            CategoryName = p.Category?.Name
        });
        
        return new PagedResult<ProductDto>(productDtos, totalCount, query.Page, query.PageSize);
    }
}
```

### 💾 Advanced Multi-Tier Caching Implementation

#### Cache Service with Multiple Providers

```csharp
// Multi-tier caching service implementation
public class MultiTierCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ICacheKeyGenerator _keyGenerator;
    private readonly ILogger<MultiTierCacheService> _logger;
    private readonly CacheConfiguration _config;

    public MultiTierCacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ICacheKeyGenerator keyGenerator,
        ILogger<MultiTierCacheService> logger,
        IOptions<CacheConfiguration> config)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _keyGenerator = keyGenerator;
        _logger = logger;
        _config = config.Value;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var fullKey = _keyGenerator.GenerateKey<T>(key);
        
        // Try memory cache first (L1)
        if (_memoryCache.TryGetValue(fullKey, out T? cachedValue))
        {
            _logger.LogDebug("Cache HIT (Memory): {Key}", fullKey);
            return cachedValue;
        }

        // Try distributed cache (L2)
        var distributedValue = await _distributedCache.GetStringAsync(fullKey, cancellationToken);
        if (!string.IsNullOrEmpty(distributedValue))
        {
            var deserializedValue = JsonSerializer.Deserialize<T>(distributedValue);
            
            // Populate memory cache
            var memoryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2),
                Priority = CacheItemPriority.Normal
            };
            _memoryCache.Set(fullKey, deserializedValue, memoryOptions);
            
            _logger.LogDebug("Cache HIT (Distributed): {Key}", fullKey);
            return deserializedValue;
        }

        _logger.LogDebug("Cache MISS: {Key}", fullKey);
        return null;
    }

    public async Task SetAsync<T>(string key, T value, CachePolicy? policy = null, CancellationToken cancellationToken = default) where T : class
    {
        var fullKey = _keyGenerator.GenerateKey<T>(key);
        var effectivePolicy = policy ?? _config.DefaultPolicy;
        
        // Set in memory cache (L1)
        var memoryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = effectivePolicy.AbsoluteExpiration,
            SlidingExpiration = effectivePolicy.SlidingExpiration,
            Priority = CacheItemPriority.Normal
        };
        _memoryCache.Set(fullKey, value, memoryOptions);

        // Set in distributed cache (L2)
        var serializedValue = JsonSerializer.Serialize(value);
        var distributedOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = effectivePolicy.AbsoluteExpiration,
            SlidingExpiration = effectivePolicy.SlidingExpiration
        };
        await _distributedCache.SetStringAsync(fullKey, serializedValue, distributedOptions, cancellationToken);
        
        _logger.LogDebug("Cache SET: {Key} (Expiry: {Expiry})", fullKey, effectivePolicy.AbsoluteExpiration);
    }

    public async Task RemoveAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var fullKey = _keyGenerator.GenerateKey<T>(key);
        
        // Remove from both caches
        _memoryCache.Remove(fullKey);
        await _distributedCache.RemoveAsync(fullKey, cancellationToken);
        
        _logger.LogDebug("Cache REMOVED: {Key}", fullKey);
    }

    public async Task RemoveByPatternAsync<T>(string pattern, CancellationToken cancellationToken = default) where T : class
    {
        var fullPattern = _keyGenerator.GenerateKey<T>(pattern);
        
        // Note: Pattern removal requires Redis-specific implementation for distributed cache
        if (_distributedCache is IRedisCache redisCache)
        {
            await redisCache.RemoveByPatternAsync(fullPattern, cancellationToken);
        }
        
        _logger.LogDebug("Cache PATTERN REMOVED: {Pattern}", fullPattern);
    }
}

// Smart cache key generator
public class CacheKeyGenerator : ICacheKeyGenerator
{
    private readonly CacheConfiguration _config;

    public CacheKeyGenerator(IOptions<CacheConfiguration> config)
    {
        _config = config.Value;
    }

    public string GenerateKey<T>(params object[] keyParts)
    {
        var typeName = typeof(T).Name.ToLowerInvariant();
        var keySegments = new List<string> { _config.KeyPrefix, typeName };
        keySegments.AddRange(keyParts.Select(p => p?.ToString()?.ToLowerInvariant() ?? "null"));
        
        return string.Join(":", keySegments.Where(s => !string.IsNullOrEmpty(s)));
    }

    public string GenerateKey<T>(string key)
    {
        return GenerateKey<T>(new object[] { key });
    }
}

// Cached repository pattern
public class CachedProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;
    private readonly ILogger<CachedProductService> _logger;

    public CachedProductService(
        IProductRepository repository,
        ICacheService cache,
        ILogger<CachedProductService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Product?> GetProductAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        var cacheKey = id.Value.ToString();
        var cachedProduct = await _cache.GetAsync<Product>(cacheKey, cancellationToken);
        
        if (cachedProduct != null)
        {
            return cachedProduct;
        }

        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product != null)
        {
            var cachePolicy = new CachePolicy
            {
                AbsoluteExpiration = TimeSpan.FromHours(2),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            await _cache.SetAsync(cacheKey, product, cachePolicy, cancellationToken);
        }

        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"category:{categoryId.Value}";
        var cachedProducts = await _cache.GetAsync<IEnumerable<Product>>(cacheKey, cancellationToken);
        
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        var products = await _repository.GetProductsByCategoryAsync(categoryId, cancellationToken);
        
        var cachePolicy = new CachePolicy
        {
            AbsoluteExpiration = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };
        await _cache.SetAsync(cacheKey, products, cachePolicy, cancellationToken);

        return products;
    }

    public async Task InvalidateProductCacheAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync<Product>(productId.Value.ToString(), cancellationToken);
        
        // Also invalidate category-based caches if needed
        var product = await _repository.GetByIdAsync(productId, cancellationToken);
        if (product != null)
        {
            await _cache.RemoveAsync<IEnumerable<Product>>($"category:{product.CategoryId.Value}", cancellationToken);
        }
        
        _logger.LogInformation("Cache invalidated for product {ProductId}", productId);
    }
}

// Cache configuration setup
public static class CachingExtensions
{
    public static IServiceCollection AddAdvancedCaching(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure cache settings
        services.Configure<CacheConfiguration>(configuration.GetSection("Cache"));
        
        // Register cache services
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = configuration["Cache:InstanceName"] ?? "DefaultInstance";
        });
        
        // Register custom services
        services.AddSingleton<ICacheKeyGenerator, CacheKeyGenerator>();
        services.AddScoped<ICacheService, MultiTierCacheService>();
        
        return services;
    }
}
```

### 🔐 Advanced JWT Authentication Implementation

#### JWT Service with Refresh Token Support

```csharp
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfiguration _config;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenService(
        IOptions<JwtConfiguration> config,
        ILogger<JwtTokenService> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _config = config.Value;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public string GenerateAccessToken(
        string userId,
        string username,
        IEnumerable<string> roles,
        IDictionary<string, string>? additionalClaims = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config.SecretKey);
        var now = _dateTimeProvider.UtcNow;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add roles
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add additional claims
        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddMinutes(_config.AccessTokenExpiryMinutes),
            Issuer = _config.Issuer,
            Audience = _config.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.LogDebug("Access token generated for user {UserId}", userId);
        return tokenString;
    }

    public RefreshToken GenerateRefreshToken(string userId)
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiryDate = _dateTimeProvider.UtcNow.AddDays(_config.RefreshTokenExpiryDays),
            CreatedDate = _dateTimeProvider.UtcNow
        };

        _logger.LogDebug("Refresh token generated for user {UserId}", userId);
        return refreshToken;
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _config.ValidateIssuer,
                ValidIssuer = _config.Issuer,
                ValidateAudience = _config.ValidateAudience,
                ValidAudience = _config.Audience,
                ValidateLifetime = _config.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed for token: {Token}", token[..10] + "...");
        }

        return null;
    }

    public DateTime GetTokenExpiration(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);
        return jsonToken.ValidTo;
    }

    public bool IsTokenExpired(string token)
    {
        var expiration = GetTokenExpiration(token);
        return expiration <= _dateTimeProvider.UtcNow;
    }
}

// Authentication middleware
public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtTokenService _jwtService;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;

    public JwtAuthenticationMiddleware(
        RequestDelegate next,
        IJwtTokenService jwtService,
        ILogger<JwtAuthenticationMiddleware> logger)
    {
        _next = next;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractTokenFromHeader(context.Request);
        
        if (!string.IsNullOrEmpty(token))
        {
            var principal = _jwtService.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
                _logger.LogDebug("JWT authentication successful for user {UserId}", 
                    principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            else
            {
                _logger.LogWarning("JWT authentication failed for token: {Token}", token[..10] + "...");
            }
        }

        await _next(context);
    }

    private static string? ExtractTokenFromHeader(HttpRequest request)
    {
        var authHeader = request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader["Bearer ".Length..].Trim();
        }
        return null;
    }
}
```

### 📨 Event-Driven Messaging Implementation

#### Complete Event Bus with Multiple Providers

```csharp
// Event bus with multiple provider support
public class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventPublisher _publisher;
    private readonly ILogger<EventBus> _logger;
    private readonly MessagingConfiguration _config;

    public EventBus(
        IServiceProvider serviceProvider,
        IEventPublisher publisher,
        ILogger<EventBus> logger,
        IOptions<MessagingConfiguration> config)
    {
        _serviceProvider = serviceProvider;
        _publisher = publisher;
        _logger = logger;
        _config = config.Value;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent
    {
        try
        {
            // Publish to external message bus
            await _publisher.PublishAsync(@event, cancellationToken);
            
            // Handle locally if configured
            if (_config.HandleLocalEvents)
            {
                await HandleLocalEventAsync(@event, cancellationToken);
            }
            
            _logger.LogInformation("Event published: {EventType} - {EventId}", 
                typeof(T).Name, @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event: {EventType} - {EventId}", 
                typeof(T).Name, @event.Id);
            throw;
        }
    }

    private async Task HandleLocalEventAsync<T>(T @event, CancellationToken cancellationToken) where T : class, IEvent
    {
        var handlers = _serviceProvider.GetServices<IEventHandler<T>>();
        var tasks = handlers.Select(handler => HandleEventSafelyAsync(handler, @event, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task HandleEventSafelyAsync<T>(IEventHandler<T> handler, T @event, CancellationToken cancellationToken) where T : class, IEvent
    {
        try
        {
            await handler.HandleAsync(@event, cancellationToken);
            _logger.LogDebug("Event handled successfully by {HandlerType}", handler.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling event {EventType} with handler {HandlerType}", 
                typeof(T).Name, handler.GetType().Name);
            
            // Don't rethrow to prevent one handler failure from affecting others
        }
    }
}

// Domain event handler example
public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(
        IEmailService emailService,
        IInventoryService inventoryService,
        ILogger<ProductCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling ProductCreatedEvent for product {ProductId}", @event.ProductId);

        // Initialize inventory
        await _inventoryService.InitializeProductInventoryAsync(@event.ProductId, cancellationToken);
        
        // Notify administrators
        var email = new EmailMessage
        {
            To = "admin@company.com",
            Subject = "New Product Created",
            Template = "product-created",
            Data = new
            {
                ProductId = @event.ProductId,
                ProductName = @event.ProductName,
                Price = @event.Price,
                CreatedAt = @event.OccurredOn
            }
        };
        
        await _emailService.SendAsync(email, cancellationToken);
        
        _logger.LogInformation("ProductCreatedEvent handled successfully for product {ProductId}", @event.ProductId);
    }
}
```

## 🧪 Testing Infrastructure Components

### Integration Testing with TestContainers

```csharp
public class RepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = _fixture.ServiceProvider.GetRequiredService<IProductRepository>();
        _unitOfWork = _fixture.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    [Fact]
    public async Task CreateProduct_ShouldPersistToDatabase()
    {
        // Arrange
        var product = new Product(
            ProductId.New(),
            "Test Product",
            new Money(99.99m, "USD"),
            CategoryId.New(),
            new ProductCode("TEST001"));

        // Act
        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var retrievedProduct = await _repository.GetByIdAsync(product.Id);
        retrievedProduct.Should().NotBeNull();
        retrievedProduct!.Name.Should().Be("Test Product");
        retrievedProduct.Price.Amount.Should().Be(99.99m);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldUseSpecification()
    {
        // Arrange
        var categoryId = CategoryId.New();
        var products = new[]
        {
            CreateTestProduct("Product 1", categoryId),
            CreateTestProduct("Product 2", categoryId),
            CreateTestProduct("Product 3", CategoryId.New()) // Different category
        };

        foreach (var product in products)
        {
            await _repository.AddAsync(product);
        }
        await _unitOfWork.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsByCategoryAsync(categoryId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.CategoryId.Should().Be(categoryId));
    }

    private static Product CreateTestProduct(string name, CategoryId categoryId)
    {
        return new Product(
            ProductId.New(),
            name,
            new Money(50m, "USD"),
            categoryId,
            new ProductCode(Guid.NewGuid().ToString()[..8].ToUpper()));
    }
}

// Database fixture for integration tests
public class DatabaseFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly TestcontainerDbContext _dbContext;

    public DatabaseFixture()
    {
        var services = new ServiceCollection();
        
        // Configure test database (using Testcontainers)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));
            
        // Register all infrastructure services
        services.AddInfrastructureServices();
        
        ServiceProvider = services.BuildServiceProvider();
        
        // Initialize database
        _dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _dbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        ServiceProvider?.Dispose();
    }
}
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
- `OpenTelemetry` - Complete observability framework
- `OpenTelemetry.Extensions.Hosting` - Hosting integration
- `Microsoft.Extensions.Logging.OpenTelemetry` - OpenTelemetry logging provider
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