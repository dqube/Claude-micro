# BuildingBlocks.API

A comprehensive, production-ready API layer building blocks library for ASP.NET Core 9 applications using Minimal APIs, following Clean Architecture principles. This library provides essential infrastructure components for modern web APIs including authentication, validation, documentation, monitoring, and more.

## 🏗️ Architecture Overview

BuildingBlocks.API serves as the presentation layer in Clean Architecture, providing concrete implementations for HTTP concerns while maintaining separation from business logic. It focuses on API-specific cross-cutting concerns like request/response handling, authentication, validation, documentation, and monitoring.

## 🚀 Core Features

### 🏗️ Minimal API Foundation
- **Modern Endpoint System**: Advanced base classes (`EndpointBase`, `CrudEndpoints`, `QueryEndpoints`) for rapid API development
- **Standardized Response Models**: Consistent API response structure with correlation IDs and timestamps
- **Smart Route Conventions**: Automatic endpoint routing and OpenAPI integration
- **Request/Response Pipeline**: Streamlined request processing with validation and error handling
- **Type-Safe Endpoints**: Strongly-typed endpoint definitions with automatic model binding

### 🔒 Advanced Authentication & Security
- **JWT Bearer Authentication**: Industry-standard JWT token authentication with configurable validation
- **API Key Authentication**: Simple yet secure API key-based authentication with custom headers
- **Multi-Scheme Authentication**: Support for multiple authentication schemes simultaneously
- **CORS Management**: Comprehensive cross-origin resource sharing configuration
- **Security Headers**: Essential security headers (HSTS, CSP, X-Frame-Options, etc.)
- **Rate Limiting**: Advanced rate limiting with per-user, per-IP, and global limits
- **Security Middleware Pipeline**: Layered security with customizable policies

### 🛡️ Robust Middleware Pipeline
- **Global Exception Handling**: Structured error responses with detailed logging and correlation tracking
- **Request/Response Logging**: Comprehensive HTTP logging with customizable levels and filters
- **Correlation ID Management**: Distributed tracing support with automatic correlation ID propagation
- **Performance Monitoring**: Request timing and performance metrics collection
- **Security Middleware**: Integrated security headers, CORS, and rate limiting middleware

### 📚 Modern Documentation & API Discovery
- **Scalar Integration**: Beautiful, interactive API documentation interface (next-generation Swagger UI)
- **OpenAPI 3.0 Support**: Full OpenAPI specification with automatic schema generation
- **Interactive Testing**: Built-in API testing capabilities within documentation
- **Customizable Documentation**: Branded documentation with custom themes and examples
- **Versioned Documentation**: Automatic documentation versioning with API versions

### 💯 Advanced Validation & Error Handling
- **FluentValidation Integration**: Comprehensive request validation with custom rules and localization
- **Structured Error Responses**: Consistent error format with detailed validation messages
- **Problem Details Support**: RFC 7807 Problem Details for HTTP APIs compliance
- **Custom Validation Attributes**: Reusable validation components for common scenarios
- **Validation Pipeline Integration**: Automatic validation with clear error propagation

### 📊 Health Monitoring & Observability
- **Comprehensive Health Checks**: Built-in health monitoring for databases, external services, and system resources
- **Custom Health Check Support**: Extensible health check framework for domain-specific monitoring
- **Health Dashboard**: JSON and UI-based health reporting with detailed status information
- **Dependency Monitoring**: Automatic health checks for database connections, Redis, external APIs
- **Alerting Integration**: Health check results suitable for monitoring and alerting systems

### 🔧 Flexible Configuration Management
- **Options Pattern**: Strongly-typed configuration classes with validation and binding
- **Environment-Specific Settings**: Support for multiple environments with configuration overrides
- **Configuration Validation**: Startup-time configuration validation with detailed error messages
- **Hot Reload Support**: Runtime configuration updates without application restart
- **Secret Management**: Integration with secure configuration providers (Azure Key Vault, etc.)

### 🌐 API Versioning & Evolution
- **URL-Based Versioning**: Clean, RESTful API versioning with automatic route generation
- **Backward Compatibility**: Support for multiple API versions simultaneously
- **Deprecation Management**: Built-in support for API deprecation warnings and migration guides
- **Version-Specific Documentation**: Separate documentation for each API version
- **Smooth Migration Path**: Tools and conventions for API evolution without breaking changes

### 🔄 Advanced JSON Processing
- **Custom JSON Converters**: Specialized converters for common data types (DateTime, Decimal, GUID, etc.)
- **Flexible Date Handling**: Multiple date format support with automatic parsing
- **Phone Number Formatting**: International phone number validation and formatting
- **Enum Serialization**: String-based enum serialization with custom naming policies
- **Null Handling**: Robust null value processing with configurable behavior

## 📋 Comprehensive Directory Structure

```
BuildingBlocks.API/
├── 📁 Authentication/                    # Authentication schemes and handlers
│   ├── 🔑 JWT/                          # JWT Bearer token authentication
│   │   ├── JwtAuthenticationExtensions.cs    # JWT service registration and configuration
│   │   └── JwtBearerOptionsSetup.cs          # JWT bearer options configuration
│   └── 🔐 ApiKey/                       # API key authentication
│       ├── ApiKeyAuthenticationExtensions.cs # API key service registration
│       └── ApiKeyAuthenticationHandler.cs    # Custom API key authentication handler
├── 📁 Configuration/                     # Configuration management and options
│   ├── 📁 Examples/                     # Configuration examples and templates
│   │   └── appsettings.ratelimiting.example.json # Rate limiting configuration example
│   ├── 📁 Extensions/                   # Configuration extension methods
│   │   └── ConfigurationExtensions.cs       # Configuration binding and validation helpers
│   └── 📁 Options/                      # Strongly-typed configuration classes
│       └── ApiOptions.cs                    # Core API configuration options
├── 📁 Converters/                       # Custom JSON converters for specialized types
│   ├── 📅 CustomDateTimeConverter.cs        # Flexible DateTime parsing and formatting
│   ├── 📅 CustomDateTimeOffsetConverter.cs  # DateTimeOffset with timezone support
│   ├── 💰 CustomDecimalConverter.cs         # Decimal precision and formatting
│   ├── 🆔 CustomGuidConverter.cs            # GUID format handling
│   ├── ❓ CustomNullableDateTimeConverter.cs # Nullable DateTime support
│   ├── 📱 CustomPhoneNumberConverter.cs     # International phone number formatting
│   ├── 📝 FlexibleStringConverter.cs        # String normalization and trimming
│   └── 🏷️ JsonStringEnumConverter.cs        # Enum to string serialization
├── 📁 Endpoints/                        # Minimal API endpoint base classes and conventions
│   ├── 📁 Base/                         # Base endpoint classes for inheritance
│   │   ├── EndpointBase.cs                  # Abstract base with common functionality
│   │   ├── CrudEndpoints.cs                 # Full CRUD operations template
│   │   └── QueryEndpoints.cs                # Read-only query operations template
│   ├── 📁 Conventions/                  # Endpoint routing and naming conventions
│   │   └── ApiEndpointConvention.cs         # Standardized endpoint conventions
│   └── 📁 Extensions/                   # Endpoint registration and configuration helpers
│       ├── EndpointRouteBuilderExtensions.cs # Route builder extension methods
│       └── MinimalApiExtensions.cs          # Minimal API utility extensions
├── 📁 Extensions/                       # Core API extension methods for service registration
│   ├── 🔧 ApiExtensions.cs                  # Main API service registration entry point
│   ├── 👤 ClaimsPrincipalExtensions.cs      # User claims and identity helpers
│   ├── ❌ ErrorHandlingExtensions.cs        # Error handling middleware registration
│   ├── 🌐 HttpContextExtensions.cs          # HTTP context utility methods
│   ├── 📄 JsonExtensions.cs                 # JSON serialization extensions
│   ├── 🔗 MiddlewareExtensions.cs           # Middleware pipeline configuration
│   ├── ⏱️ RateLimitingExtensions.cs          # Rate limiting service registration
│   ├── 📝 RequestExtensions.cs              # HTTP request helper methods
│   ├── 📤 ResponseExtensions.cs             # HTTP response helper methods
│   ├── 🛡️ SecurityExtensions.cs             # Security middleware and headers
│   ├── ✅ ValidationExtensions.cs           # Validation service registration
│   └── 🔢 VersioningExtensions.cs           # API versioning configuration
├── 📁 Health/                           # Health check implementations and reporting
│   ├── 📁 Extensions/                   # Health check service registration
│   │   └── HealthCheckExtensions.cs         # Health check registration helpers
│   └── 📁 Reporters/                    # Health check result formatters
│       └── JsonHealthReporter.cs            # JSON health check reporting
├── 📁 Middleware/                       # HTTP middleware components
│   ├── 📁 ErrorHandling/                # Global exception handling and error responses
│   │   ├── GlobalExceptionMiddleware.cs     # Global exception handling middleware
│   │   └── ProblemDetailsFactory.cs         # RFC 7807 problem details factory
│   ├── 📁 Logging/                      # Request logging and correlation
│   │   ├── CorrelationIdMiddleware.cs       # Correlation ID generation and propagation
│   │   └── RequestLoggingMiddleware.cs      # HTTP request/response logging
│   └── 📁 Security/                     # Security middleware components
│       ├── RateLimitingMiddleware.cs        # Rate limiting enforcement
│       └── SecurityHeadersMiddleware.cs     # Security headers injection
├── 📁 OpenApi/                          # OpenAPI documentation configuration
│   ├── 📁 Configuration/                # OpenAPI setup and options
│   │   └── ApiDocumentationOptions.cs      # Documentation configuration options
│   └── 📁 Extensions/                   # OpenAPI service registration
│       └── OpenApiExtensions.cs             # OpenAPI and Scalar integration
├── 📁 Responses/                        # Standardized API response models
│   ├── 📁 Base/                         # Core response classes
│   │   └── ApiResponse.cs                   # Base API response with metadata
│   └── 📁 Builders/                     # Response building utilities
│       ├── ApiResponseBuilder.cs            # Fluent API response builder
│       └── ErrorResponseBuilder.cs          # Error response construction
├── 📁 Utilities/                        # Helper classes and utility functions
│   ├── 📁 Constants/                    # API-related constants
│   │   ├── ApiConstants.cs                  # General API constants
│   │   ├── HeaderConstants.cs               # HTTP header name constants
│   │   └── HttpConstants.cs                 # HTTP status and method constants
│   ├── 📁 Factories/                    # Object creation factories
│   │   ├── ErrorFactory.cs                  # Error object creation
│   │   └── ResponseFactory.cs               # Response object creation
│   └── 📁 Helpers/                      # Utility helper methods
│       ├── CorrelationHelper.cs             # Correlation ID management
│       ├── ResponseHelper.cs                # Response formatting helpers
│       └── ValidationHelper.cs              # Validation result processing
├── 📁 Validation/                       # Request validation framework
│   ├── 📁 Extensions/                   # Validation extension methods
│   │   ├── FluentValidationExtensions.cs   # FluentValidation integration
│   │   └── ValidationExtensions.cs         # General validation extensions
│   ├── 📁 Results/                      # Validation result models
│   │   └── ValidationResult.cs              # Structured validation results
│   └── 📁 Validators/                   # Reusable validator classes
│       ├── PaginationValidator.cs           # Pagination parameter validation
│       └── RequestValidator.cs              # Base request validator
├── 📁 Versioning/                       # API versioning implementation
│   ├── 📁 Conventions/                  # Versioning conventions and rules
│   │   └── VersioningConvention.cs          # API versioning conventions
│   └── 📁 Extensions/                   # Versioning service registration
│       └── ApiVersioningExtensions.cs      # API versioning configuration
├── 📄 BuildingBlocks.API.csproj             # Project file with NuGet dependencies
├── 📚 BuildingBlocks.API.md                 # Technical architecture documentation
└── 📖 README.md                             # This comprehensive usage guide
```

## 📦 Installation

### 1. Add Project Reference

Add the project reference to your API layer:

```xml
<ProjectReference Include="..\BuildingBlocks.API\BuildingBlocks.API.csproj" />
```

### 2. NuGet Dependencies

The following NuGet packages are automatically included:

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authorization" Version="9.0.0" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
<PackageReference Include="Scalar.AspNetCore" Version="1.0.7" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
<PackageReference Include="AspNetCore.HealthChecks.UI" Version="9.0.0" />
<PackageReference Include="AspNetCore.HealthChecks.Redis" Version="9.0.0" />
<PackageReference Include="AspNetCore.HealthChecks.Uris" Version="9.0.0" />
<PackageReference Include="Asp.Versioning.AspNetCore" Version="9.0.0" />
<PackageReference Include="Asp.Versioning.AspNetCore.ApiExplorer" Version="9.0.0" />
```

## 🚀 Quick Start

### Basic Setup

Setup in `Program.cs`:

```csharp
using BuildingBlocks.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add all BuildingBlocks.API services
builder.Services.AddBuildingBlocksApi(builder.Configuration);

var app = builder.Build();

// Configure the complete API middleware pipeline
app.UseBuildingBlocksApi(builder.Configuration);

app.Run();
```

### Custom Configuration Setup

For more control over which components to include:

```csharp
using BuildingBlocks.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services with custom options
builder.Services.AddBuildingBlocksApi(builder.Configuration, options =>
{
    options.IncludeAuthentication = true;
    options.IncludeRateLimiting = true;
    options.IncludeHealthChecks = true;
    options.IncludeDocumentation = true;
    // Disable validation if using custom validation
    options.IncludeValidation = false;
});

var app = builder.Build();

// Configure pipeline with custom options
app.UseBuildingBlocksApi(builder.Configuration, options =>
{
    options.IncludeSecurityHeaders = true;
    options.IncludeRateLimiting = true;
    options.EnableDocumentationInProduction = false;
    options.HealthCheckPath = "/api/health";
    options.DocumentationTitle = "My API v1";
});

app.Run();
```

## ⚙️ Configuration

### Complete `appsettings.json` Configuration

```json
{
  "Api": {
    "Title": "My API",
    "Version": "v1.0",
    "Description": "Comprehensive API with BuildingBlocks.API",
    "ContactName": "API Team",
    "ContactEmail": "api@mycompany.com",
    "LicenseName": "MIT",
    "LicenseUrl": "https://opensource.org/licenses/MIT"
  },
  "Authentication": {
    "Jwt": {
      "SecretKey": "your-256-bit-secret-key-here-must-be-at-least-32-characters-long",
      "Issuer": "MyApp",
      "Audience": "MyApp-API",
      "ExpirationMinutes": 60,
      "ClockSkewMinutes": 5
    },
    "ApiKey": {
      "HeaderName": "X-API-Key",
      "ApiKeys": [
        "your-api-key-1",
        "your-api-key-2"
      ]
    }
  },
  "Cors": {
    "PolicyName": "DefaultCorsPolicy",
    "AllowedOrigins": [
      "https://localhost:3000",
      "https://myapp.com"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "PATCH"],
    "AllowedHeaders": ["*"],
    "AllowCredentials": true,
    "MaxAge": 86400
  },
  "RateLimiting": {
    "GlobalLimitPerMinute": 1000,
    "UserLimitPerMinute": 100,
    "IpLimitPerMinute": 200,
    "EnableGlobalLimit": true,
    "EnableUserLimit": true,
    "EnableIpLimit": true
  },
  "HealthChecks": {
    "DetailedErrors": true,
    "Timeout": 30
  }
}
```

### Environment Variables Alternative

You can also use environment variables:

```bash
# JWT Configuration
JWT__SECRETKEY=your-256-bit-secret-key-here
JWT__ISSUER=MyApp
JWT__AUDIENCE=MyApp-API

# API Key Configuration
APIKEY__HEADERNAME=X-API-Key
APIKEY__APIKEYS__0=your-first-api-key
APIKEY__APIKEYS__1=your-second-api-key

# CORS Configuration
CORS__ALLOWEDORIGINS__0=https://localhost:3000
CORS__ALLOWEDORIGINS__1=https://myapp.com
```

## 🏗️ Core Components Usage

### 1. Endpoint Base Classes

#### CRUD Endpoints

```csharp
using BuildingBlocks.API.Endpoints.Base;
using MediatR;

public class ProductEndpoints : CrudEndpoints<Product, ProductDto, Guid>
{
    public ProductEndpoints(IMediator mediator) : base(mediator) { }
    
    protected override string Tag => "Products";
    protected override string Route => "/api/v1/products";
    
    // Optionally override specific methods
    protected override async Task<IResult> HandleGetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Custom logic here
        return await base.HandleGetByIdAsync(id, cancellationToken);
    }
}

// Register endpoints in Program.cs
app.MapGroup("/api/v1/products")
   .WithTags("Products")
   .WithOpenApi()
   .RegisterEndpoints<ProductEndpoints>();
```

#### Query-Only Endpoints

```csharp
using BuildingBlocks.API.Endpoints.Base;

public class ReportEndpoints : QueryEndpoints<ReportDto, ReportQuery>
{
    public ReportEndpoints(IMediator mediator) : base(mediator) { }
    
    protected override string Tag => "Reports";
    protected override string Route => "/api/v1/reports";
    
    protected override async Task<IResult> HandleGetAsync(ReportQuery query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return ApiResponse.Success(result);
    }
}
```

### 2. Custom Endpoints

```csharp
using BuildingBlocks.API.Endpoints.Base;
using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.API.Utilities.Helpers;

public class CustomEndpoints : EndpointBase
{
    private readonly ICustomService _customService;
    
    public CustomEndpoints(ICustomService customService)
    {
        _customService = customService;
    }
    
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/custom")
                      .WithTags("Custom")
                      .WithOpenApi();
        
        group.MapGet("/", GetAllAsync);
        group.MapGet("/{id:guid}", GetByIdAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:guid}", UpdateAsync);
        group.MapDelete("/{id:guid}", DeleteAsync);
    }
    
    private async Task<IResult> GetAllAsync(
        [AsParameters] PaginationQuery pagination,
        CancellationToken cancellationToken)
    {
        var result = await _customService.GetAllAsync(pagination, cancellationToken);
        
        if (!result.IsSuccessful)
            return ApiResponse.Error(result.Error, GetCorrelationId());
        
        return ApiResponse.Success(result.Data, GetCorrelationId());
    }
    
    private async Task<IResult> CreateAsync(
        CreateCustomRequest request,
        IValidator<CreateCustomRequest> validator,
        CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ValidationHelper.ValidationError(validationResult.Errors, GetCorrelationId());
        }
        
        // Process request
        var result = await _customService.CreateAsync(request, cancellationToken);
        
        if (!result.IsSuccessful)
            return ApiResponse.Error(result.Error, GetCorrelationId());
        
        return ApiResponse.Created(result.Data, GetCorrelationId());
    }
}
```

### 3. Request Validation

#### FluentValidation Setup

```csharp
using FluentValidation;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThan(10000).WithMessage("Price cannot exceed $10,000");
        
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(BeValidCategory).WithMessage("Invalid category");
        
        RuleFor(x => x.Tags)
            .Must(x => x.Count <= 5).WithMessage("Cannot have more than 5 tags");
    }
    
    private bool BeValidCategory(string category)
    {
        var validCategories = new[] { "Electronics", "Clothing", "Books", "Home" };
        return validCategories.Contains(category);
    }
}
```

#### Custom Validator Base Class

```csharp
using BuildingBlocks.API.Validation.Validators;

public class ProductValidator : RequestValidator<CreateProductRequest>
{
    public ProductValidator()
    {
        ValidateId(x => x.Id); // Built-in ID validation
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
```

### 4. Response Building

#### Standard API Responses

```csharp
public class ProductController : EndpointBase
{
    public async Task<IResult> GetProduct(Guid id)
    {
        var product = await GetProductAsync(id);
        
        // Success response
        if (product != null)
            return ApiResponse.Success(product, GetCorrelationId());
        
        // Error responses
        return ApiResponse.NotFound("Product not found", GetCorrelationId());
    }
    
    public async Task<IResult> CreateProduct(CreateProductRequest request)
    {
        var product = await CreateProductAsync(request);
        
        // Created response with location header
        return ApiResponse.Created(product, $"/api/products/{product.Id}", GetCorrelationId());
    }
    
    public async Task<IResult> HandleError()
    {
        try
        {
            // Some operation
        }
        catch (ValidationException ex)
        {
            return ApiResponse.ValidationError(ex.Errors, GetCorrelationId());
        }
        catch (NotFoundException ex)
        {
            return ApiResponse.NotFound(ex.Message, GetCorrelationId());
        }
        catch (Exception ex)
        {
            return ApiResponse.Error("Internal server error", GetCorrelationId());
        }
    }
}
```

#### Custom Response Building

```csharp
using BuildingBlocks.API.Responses.Builders;

public class CustomResponseExample
{
    public IResult BuildCustomResponse()
    {
        // Using ApiResponseBuilder for complex responses
        var response = ApiResponseBuilder
            .Success(new { Message = "Operation completed" })
            .WithCorrelationId(Guid.NewGuid().ToString())
            .WithTimestamp(DateTimeOffset.UtcNow)
            .Build();
        
        return Results.Ok(response);
    }
    
    public IResult BuildErrorResponse()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } },
            { "Email", new[] { "Invalid email format" } }
        };
        
        var response = ApiResponseBuilder
            .ValidationError(errors)
            .WithCorrelationId("12345")
            .Build();
        
        return Results.BadRequest(response);
    }
}
```

## 🔧 Advanced Implementation Examples

### 🏗️ Modern Endpoint Development

#### Advanced CRUD Endpoints with Custom Logic
```csharp
using BuildingBlocks.API.Endpoints.Base;
using BuildingBlocks.API.Responses.Base;
using MediatR;

public class ProductEndpoints : CrudEndpoints<Product, ProductDto, ProductId>
{
    private readonly IProductService _productService;
    
    public ProductEndpoints(IMediator mediator, IProductService productService) 
        : base(mediator)
    {
        _productService = productService;
    }
    
    protected override string Tag => "Products";
    protected override string Route => "/api/v1/products";
    
    // Override with custom business logic
    protected override async Task<IResult> HandleCreateAsync(
        CreateProductRequest request, 
        CancellationToken cancellationToken)
    {
        // Custom validation
        if (await _productService.ProductExistsAsync(request.Sku))
        {
            return ApiError("Product with this SKU already exists", 409, GetCorrelationId());
        }
        
        // Custom creation logic
        var command = new CreateProductCommand(request);
        var result = await Mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
            return ApiError(result.Error, correlationId: GetCorrelationId());
        
        return Created(result.Data, $"/api/v1/products/{result.Data.Id}", 
            correlationId: GetCorrelationId());
    }
    
    // Custom endpoint beyond CRUD
    public void RegisterCustomEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Route).WithTags(Tag);
        
        group.MapGet("/{id}/analytics", GetProductAnalyticsAsync)
            .RequireAuthorization()
            .WithOpenApi(op => new(op)
            {
                Summary = "Get product analytics",
                Description = "Retrieve detailed analytics for a specific product"
            });
            
        group.MapPost("/{id}/inventory", UpdateInventoryAsync)
            .RequireAuthorization("InventoryManager")
            .WithOpenApi();
    }
    
    private async Task<IResult> GetProductAnalyticsAsync(
        ProductId id,
        [AsParameters] AnalyticsQuery query,
        CancellationToken cancellationToken)
    {
        var analytics = await _productService.GetAnalyticsAsync(id, query, cancellationToken);
        return ApiResponse(analytics, correlationId: GetCorrelationId());
    }
}
```

#### Custom Query Endpoints with Advanced Filtering
```csharp
public class ProductQueryEndpoints : QueryEndpoints<ProductDto, ProductQuery>
{
    public ProductQueryEndpoints(IMediator mediator) : base(mediator) { }
    
    protected override string Tag => "Product Queries";
    protected override string Route => "/api/v1/products/query";
    
    public void RegisterAdvancedQueries(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/products")
            .WithTags("Products")
            .WithOpenApi();
        
        // Advanced search with faceting
        group.MapPost("/search", SearchProductsAsync)
            .WithOpenApi(op => new(op)
            {
                Summary = "Advanced product search",
                Description = "Search products with filters, facets, and full-text search"
            });
            
        // Bulk operations
        group.MapPost("/bulk/update", BulkUpdateProductsAsync)
            .RequireAuthorization("ProductManager")
            .WithOpenApi();
            
        // Export endpoints
        group.MapGet("/export/{format}", ExportProductsAsync)
            .RequireAuthorization()
            .WithOpenApi();
    }
    
    private async Task<IResult> SearchProductsAsync(
        ProductSearchRequest request,
        IValidator<ProductSearchRequest> validator,
        CancellationToken cancellationToken)
    {
        // Validation
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return ValidationError(validationResult.ToDictionary(), GetCorrelationId());
        
        // Execute search
        var query = new SearchProductsQuery(request);
        var result = await Mediator.Send(query, cancellationToken);
        
        return ApiResponse(result, correlationId: GetCorrelationId());
    }
}
```

### 🔒 Advanced Authentication & Security Implementation

#### Multi-Scheme Authentication Setup
```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure multiple authentication schemes
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "MultiAuthSchemes";
    options.DefaultChallengeScheme = "MultiAuthSchemes";
})
.AddScheme<MultiAuthenticationHandler>("MultiAuthSchemes", options => { })
.AddJwtBearer("JWT", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
})
.AddScheme<ApiKeyAuthenticationHandler>("ApiKey", options =>
{
    options.ApiKeys = builder.Configuration.GetSection("ApiKey:Keys").Get<string[]>();
    options.HeaderName = "X-API-Key";
});

// Advanced authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin")
              .RequireAuthenticatedUser());
              
    options.AddPolicy("RequireValidSubscription", policy =>
        policy.Requirements.Add(new SubscriptionRequirement()));
        
    options.AddPolicy("RateLimitExempt", policy =>
        policy.RequireClaim("rate_limit_exempt", "true"));
});
```

#### Custom Authorization Requirements
```csharp
public class SubscriptionRequirement : IAuthorizationRequirement
{
    public string RequiredPlan { get; set; } = "Premium";
}

public class SubscriptionAuthorizationHandler : AuthorizationHandler<SubscriptionRequirement>
{
    private readonly ISubscriptionService _subscriptionService;
    
    public SubscriptionAuthorizationHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SubscriptionRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            context.Fail();
            return;
        }
        
        var subscription = await _subscriptionService.GetUserSubscriptionAsync(userId);
        if (subscription?.Plan == requirement.RequiredPlan && subscription.IsActive)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
```

### 🛡️ Advanced Middleware Pipeline

#### Custom Request/Response Middleware
```csharp
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    
    public RequestResponseLoggingMiddleware(
        RequestDelegate next, 
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.TraceIdentifier;
        var stopwatch = Stopwatch.StartNew();
        
        // Log request
        await LogRequestAsync(context, correlationId);
        
        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Log response
            await LogResponseAsync(context, correlationId, stopwatch.ElapsedMilliseconds);
            
            // Copy response back
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
    
    private async Task LogRequestAsync(HttpContext context, string correlationId)
    {
        var request = context.Request;
        var requestBody = "";
        
        if (request.ContentLength > 0 && request.ContentType?.Contains("application/json") == true)
        {
            request.EnableBuffering();
            requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;
        }
        
        _logger.LogInformation(
            "HTTP Request: {Method} {Path} | CorrelationId: {CorrelationId} | Body: {Body}",
            request.Method, request.Path, correlationId, requestBody);
    }
    
    private async Task LogResponseAsync(HttpContext context, string correlationId, long elapsed)
    {
        var response = context.Response;
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        
        _logger.LogInformation(
            "HTTP Response: {StatusCode} | CorrelationId: {CorrelationId} | Elapsed: {Elapsed}ms | Body: {Body}",
            response.StatusCode, correlationId, elapsed, responseBody);
    }
}
```

### 📊 Advanced Health Checks

#### Custom Health Check Implementation
```csharp
public class DatabaseConnectionHealthCheck : IHealthCheck
{
    private readonly IDbContext _dbContext;
    private readonly ILogger<DatabaseConnectionHealthCheck> _logger;
    
    public DatabaseConnectionHealthCheck(IDbContext dbContext, ILogger<DatabaseConnectionHealthCheck> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Test database connectivity
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy("Cannot connect to database");
            }
            
            // Test query performance
            var stopwatch = Stopwatch.StartNew();
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            stopwatch.Stop();
            
            var data = new Dictionary<string, object>
            {
                { "connection_time_ms", stopwatch.ElapsedMilliseconds },
                { "database_provider", _dbContext.Database.ProviderName }
            };
            
            return stopwatch.ElapsedMilliseconds < 1000
                ? HealthCheckResult.Healthy("Database is healthy", data)
                : HealthCheckResult.Degraded("Database response is slow", null, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database health check failed", ex);
        }
    }
}
```

### 💯 Advanced Validation Patterns

#### Complex FluentValidation Rules
```csharp
public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;
    
    public CreateOrderValidator(IProductService productService, ICustomerService customerService)
    {
        _productService = productService;
        _customerService = customerService;
        
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .MustAsync(CustomerExists)
            .WithMessage("Customer does not exist")
            .MustAsync(CustomerIsActive)
            .WithMessage("Customer account is inactive");
            
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must contain at least one item")
            .Must(x => x.Count <= 50)
            .WithMessage("Order cannot contain more than 50 items");
            
        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemValidator(_productService));
            
        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .SetValidator(new AddressValidator());
            
        // Business rules
        RuleFor(x => x)
            .MustAsync(ValidateOrderTotalAsync)
            .WithMessage("Order total calculation is invalid")
            .MustAsync(ValidateInventoryAsync)
            .WithMessage("Insufficient inventory for one or more items");
    }
    
    private async Task<bool> CustomerExists(Guid customerId, CancellationToken cancellationToken)
    {
        return await _customerService.ExistsAsync(customerId, cancellationToken);
    }
    
    private async Task<bool> CustomerIsActive(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(customerId, cancellationToken);
        return customer?.IsActive == true;
    }
    
    private async Task<bool> ValidateOrderTotalAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var calculatedTotal = 0m;
        foreach (var item in request.Items)
        {
            var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);
            calculatedTotal += product.Price * item.Quantity;
        }
        
        return Math.Abs(calculatedTotal - request.Total) < 0.01m;
    }
}
```

## 🔧 Advanced Configuration

### 1. Individual Component Registration

If you need granular control, register components individually:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Core components
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddApiMiddleware();
builder.Services.AddApiErrorHandling();

// Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApiKeyAuthentication("key1", "key2", "key3");

// Validation
builder.Services.AddApiValidation();
builder.Services.AddApiValidation(typeof(ProductValidator).Assembly);

// Rate limiting with custom options
builder.Services.AddApiRateLimiting(options =>
{
    options.GlobalLimitPerMinute = 500;
    options.UserLimitPerMinute = 50;
    options.EnableGlobalLimit = true;
});

// Health checks
builder.Services.AddApiHealthChecks();
builder.Services.AddDatabaseHealthCheck("ConnectionString");
builder.Services.AddRedisHealthCheck("RedisConnection");
builder.Services.AddUrlHealthCheck("https://external-api.com/health");

// Documentation
var docOptions = new ApiDocumentationOptions
{
    Title = "My API",
    Version = "v1.0",
    Description = "Comprehensive API documentation"
};
builder.Services.AddOpenApiDocumentation(docOptions);
builder.Services.AddScalarDocumentation();

var app = builder.Build();

// Configure middleware pipeline manually
if (app.Environment.IsDevelopment())
{
    app.UseDevelopmentErrorHandling();
}
else
{
    app.UseApiErrorHandling();
}

app.UseApiSecurityHeaders();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseCorrelationId();
app.UseRequestLogging();
app.UseApiVersioning();

// Map endpoints
app.MapHealthChecks("/health");
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();
```

### 2. Custom Middleware Configuration

```csharp
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Middleware.Logging;
using BuildingBlocks.API.Middleware.Security;

var builder = WebApplication.CreateBuilder(args);

// Register specific middleware
builder.Services.AddScoped<GlobalExceptionMiddleware>();
builder.Services.AddScoped<RequestLoggingMiddleware>();
builder.Services.AddScoped<CorrelationIdMiddleware>();
builder.Services.AddScoped<SecurityHeadersMiddleware>();

var app = builder.Build();

// Use specific middleware in custom order
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.Run();
```

### 3. Custom Authentication Schemes

```csharp
using BuildingBlocks.API.Authentication.ApiKey;
using BuildingBlocks.API.Authentication.JWT;

var builder = WebApplication.CreateBuilder(args);

// JWT with custom configuration
builder.Services.AddJwtAuthentication(
    secretKey: "your-custom-secret-key-here-at-least-32-chars",
    issuer: "CustomIssuer",
    audience: "CustomAudience",
    expiration: TimeSpan.FromHours(2)
);

// API Key with custom options
builder.Services.AddApiKeyAuthentication(options =>
{
    options.HeaderName = "X-Custom-API-Key";
    options.ApiKeys = new[] { "key1", "key2" };
    options.IgnoreCaseInKeys = true;
});

// Multiple authentication schemes
builder.Services.AddAuthentication()
    .AddJwtBearer("jwt", options => { /* JWT options */ })
    .AddScheme<ApiKeyAuthenticationHandler>("apikey", options => { /* API key options */ });

var app = builder.Build();
app.Run();
```

## 📊 Health Checks & Monitoring

### Built-in Health Checks

The library includes several built-in health checks:

```csharp
// Basic health checks (memory, self-check)
builder.Services.AddApiHealthChecks();

// Database health checks
builder.Services.AddDatabaseHealthCheck(connectionString);

// Redis health checks
builder.Services.AddRedisHealthCheck(connectionString);

// External service health checks
builder.Services.AddUrlHealthCheck("https://api.external-service.com/health");
```

### Custom Health Checks

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class CustomHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Perform your custom health check logic
            var isHealthy = await CheckExternalDependency();
            
            return isHealthy
                ? HealthCheckResult.Healthy("Custom service is healthy")
                : HealthCheckResult.Unhealthy("Custom service is down");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Custom service check failed", ex);
        }
    }
    
    private async Task<bool> CheckExternalDependency()
    {
        // Implementation here
        await Task.Delay(100);
        return true;
    }
}

// Register custom health check
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom-service");
```

## 📝 API Documentation

### Scalar Documentation (Default)

The library uses Scalar as the default API documentation interface:

- **Modern UI**: Clean, responsive interface
- **Interactive**: Built-in API testing capabilities
- **Customizable**: Themes and branding options
- **OpenAPI 3.0**: Full OpenAPI specification support

Access documentation at: `https://localhost:5001/scalar/v1`

### Swagger UI (Alternative)

If you prefer Swagger UI:

```csharp
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

## 🔐 Security Best Practices

### 1. JWT Security

```json
{
  "Authentication": {
    "Jwt": {
      "SecretKey": "use-a-strong-256-bit-key-here-minimum-32-characters-required",
      "Issuer": "YourApp",
      "Audience": "YourApp-API",
      "ExpirationMinutes": 60,
      "ClockSkewMinutes": 5,
      "RequireHttpsMetadata": true,
      "ValidateIssuer": true,
      "ValidateAudience": true,
      "ValidateLifetime": true,
      "ValidateIssuerSigningKey": true
    }
  }
}
```

### 2. API Key Security

```json
{
  "Authentication": {
    "ApiKey": {
      "HeaderName": "X-API-Key",
      "ApiKeys": [
        "generate-strong-random-keys",
        "use-guid-or-crypto-random"
      ],
      "IgnoreCaseInKeys": false
    }
  }
}
```

### 3. CORS Configuration

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://app.yourdomain.com"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["Content-Type", "Authorization"],
    "AllowCredentials": true,
    "MaxAge": 86400
  }
}
```

### 4. Rate Limiting

```json
{
  "RateLimiting": {
    "GlobalLimitPerMinute": 1000,
    "UserLimitPerMinute": 100,
    "IpLimitPerMinute": 200,
    "EnableGlobalLimit": true,
    "EnableUserLimit": true,
    "EnableIpLimit": true
  }
}
```

## 🏛️ Architecture Integration

This library is designed to work within Clean Architecture:

```
📁 YourSolution/
├── 📁 src/
│   ├── 📁 Domain/                    (BuildingBlocks.Domain)
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── Interfaces/
│   ├── 📁 Application/               (BuildingBlocks.Application)
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Handlers/
│   │   └── DTOs/
│   ├── 📁 Infrastructure/            (BuildingBlocks.Infrastructure)
│   │   ├── Persistence/
│   │   ├── ExternalServices/
│   │   └── Configuration/
│   ├── 📁 API/                      (Your API Project + BuildingBlocks.API)
│   │   ├── Endpoints/
│   │   ├── Validators/
│   │   ├── Program.cs
│   │   └── appsettings.json
├── 📁 tests/
│   ├── 📁 UnitTests/
│   ├── 📁 IntegrationTests/
│   └── 📁 ArchitectureTests/
└── 📁 docs/
```

## 📦 Dependencies

This library depends on the following packages:

### Core Dependencies
- `Microsoft.AspNetCore.App` (9.0.0)
- `Microsoft.AspNetCore.Authentication.JwtBearer` (9.0.0)
- `Microsoft.AspNetCore.Authorization` (9.0.0)

### Validation & Documentation
- `FluentValidation.AspNetCore` (11.3.0)
- `Scalar.AspNetCore` (1.0.7)
- `Swashbuckle.AspNetCore` (7.2.0)

### Health Checks & Monitoring
- `AspNetCore.HealthChecks.UI` (9.0.0)
- `AspNetCore.HealthChecks.Redis` (9.0.0)
- `AspNetCore.HealthChecks.Uris` (9.0.0)

### API Versioning
- `Asp.Versioning.AspNetCore` (9.0.0)
- `Asp.Versioning.AspNetCore.ApiExplorer` (9.0.0)

### Project Dependencies
- `BuildingBlocks.Domain`: Domain layer abstractions
- `BuildingBlocks.Application`: Application layer interfaces

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Add comprehensive unit tests
3. Update documentation
4. Follow semantic versioning
5. Ensure all health checks pass

## 📄 License

This project is licensed under the MIT License.