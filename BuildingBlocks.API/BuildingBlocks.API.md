# BuildingBlocks.API - Comprehensive Technical Architecture

## 🏗️ Architectural Overview

The BuildingBlocks.API library represents the **Presentation Layer** in Clean Architecture, providing a comprehensive, production-ready foundation for modern ASP.NET Core 9 APIs. This library implements essential infrastructure components specifically designed for the API layer while maintaining strict separation of concerns and dependency inversion principles.

### 🎯 Design Philosophy

- **Clean Architecture Compliance**: Implements the outer layer of Clean Architecture with proper dependency flow
- **Minimal API Focus**: Leverages ASP.NET Core 9's Minimal APIs for lightweight, high-performance endpoints
- **Cross-Cutting Concerns**: Centralizes API-specific concerns like authentication, validation, documentation, and monitoring
- **Production-Ready**: Enterprise-grade implementations with security, observability, and maintainability built-in
- **Extensibility**: Designed for customization and extension without modifying core components

## 📋 Comprehensive Directory Structure & Implementation Details

```
BuildingBlocks.API/
├── 📁 Authentication/                           # Multi-scheme authentication infrastructure
│   ├── 🔑 JWT/                                 # JSON Web Token authentication
│   │   ├── JwtAuthenticationExtensions.cs          # Service registration and JWT configuration
│   │   └── JwtBearerOptionsSetup.cs               # Advanced JWT bearer token options setup
│   └── 🔐 ApiKey/                              # API key-based authentication
│       ├── ApiKeyAuthenticationExtensions.cs       # API key service registration and configuration
│       └── ApiKeyAuthenticationHandler.cs          # Custom authentication handler for API keys
├── 📁 Configuration/                            # Centralized configuration management
│   ├── 📁 Examples/                            # Configuration templates and examples
│   │   └── appsettings.ratelimiting.example.json   # Rate limiting configuration example
│   ├── 📁 Extensions/                          # Configuration binding and validation
│   │   └── ConfigurationExtensions.cs              # IConfiguration extension methods and helpers
│   └── 📁 Options/                             # Strongly-typed configuration classes
│       └── ApiOptions.cs                           # Core API configuration options with validation
├── 📁 Converters/                              # Specialized JSON type converters
│   ├── 📅 CustomDateTimeConverter.cs               # Multi-format DateTime parsing and serialization
│   ├── 📅 CustomDateTimeOffsetConverter.cs         # DateTimeOffset with timezone handling
│   ├── 💰 CustomDecimalConverter.cs                # Precision decimal formatting and parsing
│   ├── 🆔 CustomGuidConverter.cs                   # GUID format normalization and validation
│   ├── ❓ CustomNullableDateTimeConverter.cs       # Nullable DateTime with multiple format support
│   ├── 📱 CustomPhoneNumberConverter.cs            # International phone number formatting
│   ├── 📝 FlexibleStringConverter.cs               # String trimming and normalization
│   └── 🏷️ JsonStringEnumConverter.cs               # Enum serialization with custom naming policies
├── 📁 Endpoints/                               # Minimal API endpoint framework
│   ├── 📁 Base/                                # Abstract base classes for endpoint inheritance
│   │   ├── EndpointBase.cs                         # Core endpoint functionality with response helpers
│   │   ├── CrudEndpoints.cs                        # Complete CRUD operations template with overrides
│   │   └── QueryEndpoints.cs                       # Read-only operations with advanced querying
│   ├── 📁 Conventions/                         # Endpoint routing and naming standards
│   │   └── ApiEndpointConvention.cs                # Consistent endpoint naming and routing conventions
│   └── 📁 Extensions/                          # Endpoint registration and utility methods
│       ├── EndpointRouteBuilderExtensions.cs       # IEndpointRouteBuilder extension methods
│       └── MinimalApiExtensions.cs                 # Minimal API utility extensions and helpers
├── 📁 Extensions/                              # Core service registration and configuration
│   ├── 🔧 ApiExtensions.cs                         # Main entry point for API service registration
│   ├── 👤 ClaimsPrincipalExtensions.cs             # User identity and claims utility methods
│   ├── ❌ ErrorHandlingExtensions.cs               # Error handling middleware registration
│   ├── 🌐 HttpContextExtensions.cs                 # HTTP context utility and helper methods
│   ├── 📄 JsonExtensions.cs                        # JSON serialization configuration extensions
│   ├── 🔗 MiddlewareExtensions.cs                  # Middleware pipeline configuration helpers
│   ├── ⏱️ RateLimitingExtensions.cs                 # Rate limiting service registration and configuration
│   ├── 📝 RequestExtensions.cs                     # HTTP request parsing and validation helpers
│   ├── 📤 ResponseExtensions.cs                    # HTTP response formatting and utility methods
│   ├── 🛡️ SecurityExtensions.cs                    # Security middleware and header configuration
│   ├── ✅ ValidationExtensions.cs                  # Validation service registration and configuration
│   └── 🔢 VersioningExtensions.cs                  # API versioning service registration
├── 📁 Health/                                  # Health monitoring and reporting infrastructure
│   ├── 📁 Extensions/                          # Health check service registration
│   │   └── HealthCheckExtensions.cs                # Health check registration and configuration helpers
│   └── 📁 Reporters/                           # Health check result formatting
│       └── JsonHealthReporter.cs                   # Structured JSON health check reporting
├── 📁 Middleware/                              # HTTP middleware pipeline components
│   ├── 📁 ErrorHandling/                       # Global exception handling and error responses
│   │   ├── GlobalExceptionMiddleware.cs            # Centralized exception handling with logging
│   │   └── ProblemDetailsFactory.cs                # RFC 7807 Problem Details factory implementation
│   ├── 📁 Logging/                             # Request/response logging and correlation
│   │   ├── CorrelationIdMiddleware.cs              # Correlation ID generation and HTTP header management
│   │   └── RequestLoggingMiddleware.cs             # Comprehensive HTTP request/response logging
│   └── 📁 Security/                            # Security middleware implementations
│       ├── RateLimitingMiddleware.cs               # Advanced rate limiting with multiple strategies
│       └── SecurityHeadersMiddleware.cs            # Security header injection (HSTS, CSP, etc.)
├── 📁 OpenApi/                                 # OpenAPI documentation and Scalar integration
│   ├── 📁 Configuration/                       # OpenAPI setup and customization
│   │   └── ApiDocumentationOptions.cs             # Documentation configuration with branding options
│   └── 📁 Extensions/                          # OpenAPI service registration
│       └── OpenApiExtensions.cs                    # OpenAPI and Scalar documentation integration
├── 📁 Responses/                               # Standardized API response framework
│   ├── 📁 Base/                                # Core response models with metadata
│   │   └── ApiResponse.cs                          # Base API response with success/error structure
│   └── 📁 Builders/                            # Fluent response construction utilities
│       ├── ApiResponseBuilder.cs                   # Fluent API for building complex responses
│       └── ErrorResponseBuilder.cs                 # Specialized error response construction
├── 📁 Utilities/                               # Shared utilities and helper functions
│   ├── 📁 Constants/                           # API-related constant definitions
│   │   ├── ApiConstants.cs                         # General API constants and default values
│   │   ├── HeaderConstants.cs                      # HTTP header name constants
│   │   └── HttpConstants.cs                        # HTTP status codes and method constants
│   ├── 📁 Factories/                           # Object creation and factory patterns
│   │   ├── ErrorFactory.cs                         # Centralized error object creation
│   │   └── ResponseFactory.cs                      # Response object factory with templates
│   └── 📁 Helpers/                             # Utility helper methods and functions
│       ├── CorrelationHelper.cs                    # Correlation ID management and propagation
│       ├── ResponseHelper.cs                       # Response formatting and transformation helpers
│       └── ValidationHelper.cs                     # Validation result processing and formatting
├── 📁 Validation/                              # Comprehensive request validation framework
│   ├── 📁 Extensions/                          # Validation integration extensions
│   │   ├── FluentValidationExtensions.cs           # FluentValidation framework integration
│   │   └── ValidationExtensions.cs                 # General validation utility extensions
│   ├── 📁 Results/                             # Validation result models and structures
│   │   └── ValidationResult.cs                     # Structured validation results with error details
│   └── 📁 Validators/                          # Reusable validator implementations
│       ├── PaginationValidator.cs                  # Pagination parameter validation with limits
│       └── RequestValidator.cs                     # Base request validator with common rules
├── 📁 Versioning/                              # API versioning implementation and management
│   ├── 📁 Conventions/                         # Versioning standards and conventions
│   │   └── VersioningConvention.cs                 # API versioning rules and URL conventions
│   └── 📁 Extensions/                          # Versioning service registration
│       └── ApiVersioningExtensions.cs              # API versioning configuration and setup
├── 📄 BuildingBlocks.API.csproj                    # Project file with NuGet package dependencies
├── 📚 BuildingBlocks.API.md                        # This comprehensive technical documentation
└── 📖 README.md                                    # User-friendly implementation guide and examples
```

## 🔍 Detailed Component Analysis & Implementation Patterns

### 🏗️ **Advanced Minimal API Endpoint Framework**
- **EndpointBase.cs**: 
  - Abstract base class providing common HTTP response patterns
  - Built-in correlation ID management with `GetCorrelationId(HttpContext)`
  - User identity extraction with `GetUserId()` and `GetUserIdAsGuid()`
  - Standardized response methods: `ApiResponse<T>()`, `ApiError()`, `ValidationError()`, `Created<T>()`
  - Consistent error handling with proper HTTP status codes and Problem Details integration
- **CrudEndpoints.cs**: 
  - Generic CRUD template with `TEntity`, `TDto`, and `TId` type parameters
  - Overridable methods: `HandleGetAsync()`, `HandleCreateAsync()`, `HandleUpdateAsync()`, `HandleDeleteAsync()`
  - Automatic MediatR integration for command/query pattern
  - Built-in validation support with automatic model state checking
  - OpenAPI metadata generation with proper response type documentation
- **QueryEndpoints.cs**: 
  - Read-only operations with advanced querying capabilities
  - Pagination support with `PagedResponse<T>` wrapper
  - Filtering and sorting parameter binding
  - Specification pattern integration for complex queries
- **EndpointRouteBuilderExtensions.cs**: 
  - Fluent endpoint registration with `RegisterEndpoints<T>()`
  - Automatic route pattern generation based on conventions
  - OpenAPI integration with `WithOpenApi()` extensions
  - Authorization policy binding helpers

### 🛡️ **Comprehensive Middleware Pipeline Architecture**
- **GlobalExceptionMiddleware.cs**: 
  - Centralized exception handling with structured logging using `LoggerMessage.Define`
  - Exception type-specific handling (ArgumentException, UnauthorizedAccessException, etc.)
  - RFC 7807 Problem Details response generation
  - Correlation ID preservation across error responses
  - Environment-specific error detail exposure (development vs production)
- **RequestLoggingMiddleware.cs**: 
  - HTTP request/response logging with configurable detail levels
  - Performance timing with request duration measurement
  - Request body capturing for POST/PUT operations (with size limits)
  - Response body logging with sensitive data filtering
  - Structured logging with correlation ID propagation
- **CorrelationIdMiddleware.cs**: 
  - Automatic correlation ID generation using `HttpContext.TraceIdentifier`
  - HTTP header propagation (`X-Correlation-ID`, `X-Request-ID`)
  - Thread-local correlation context for downstream services
  - Integration with logging scopes for request tracing
- **SecurityHeadersMiddleware.cs**: 
  - Security header injection: HSTS, CSP, X-Frame-Options, X-Content-Type-Options
  - Configurable security policies with environment-specific settings
  - HTTPS enforcement with automatic redirection
  - Browser security feature enablement

### 🔐 **Multi-Scheme Authentication Infrastructure**
- **JwtAuthenticationExtensions.cs**: 
  - JWT Bearer token configuration with `TokenValidationParameters`
  - Configurable token validation: issuer, audience, lifetime, signing key
  - Custom JWT events: `OnAuthenticationFailed`, `OnTokenValidated`, `OnChallenge`
  - Support for multiple JWT configurations (development, staging, production)
  - Integration with ASP.NET Core Identity for user management
- **ApiKeyAuthenticationHandler.cs**: 
  - Custom authentication handler implementing `AuthenticationHandler<TOptions>`
  - Header-based API key extraction with configurable header names
  - API key validation against configured key store
  - Support for multiple API keys with different permission levels
  - Rate limiting integration for API key-based requests

### 📤 **Advanced Response Framework**
- **ApiResponse.cs**: 
  - Base response structure with `Success`, `Message`, `CorrelationId`, `Timestamp`
  - Generic `ApiResponse<T>` for typed data responses
  - `PagedResponse<T>` with comprehensive pagination metadata
  - `ErrorResponse` with detailed error information and error codes
  - `ValidationErrorResponse` with field-specific validation errors
- **ApiResponseBuilder.cs**: 
  - Fluent API for building complex responses
  - Method chaining for response construction: `Success()`, `Error()`, `WithCorrelationId()`
  - Template-based response generation for common scenarios
  - Metadata attachment for response enrichment
- **ErrorResponseBuilder.cs**: 
  - Specialized builder for error responses with error categorization
  - Support for nested error details and error hierarchies
  - Integration with validation frameworks for detailed field errors
  - Error code standardization with descriptive messages

### ✅ **Enterprise Validation Framework**
- **FluentValidationExtensions.cs**: 
  - Deep FluentValidation integration with automatic validator discovery
  - Custom validation result processing with detailed error mapping
  - Async validation support for database-dependent rules
  - Localization support for multi-language error messages
  - Integration with dependency injection for validator services
- **RequestValidator.cs**: 
  - Base validator class with common validation rules
  - Built-in validation for pagination parameters, IDs, and common fields
  - Reusable validation methods: `ValidateId()`, `ValidateEmail()`, `ValidatePhoneNumber()`
  - Business rule integration with domain layer validation
- **ValidationResult.cs**: 
  - Structured validation results with field-level error details
  - Error severity levels (Error, Warning, Information)
  - Hierarchical error structure for complex object validation
  - Integration with Problem Details for consistent error responses

### 📚 **Modern API Documentation**
- **OpenApiExtensions.cs**: 
  - OpenAPI 3.0 specification generation with comprehensive schema documentation
  - Scalar documentation UI integration (modern alternative to Swagger UI)
  - Custom operation filters for authentication requirements
  - Response example generation with sample data
  - API versioning integration with separate documentation per version
- **ApiDocumentationOptions.cs**: 
  - Configurable documentation settings: title, version, description, contact info
  - Environment-specific documentation behavior
  - Custom CSS and branding support for documentation UI
  - Authentication documentation with example tokens

### 🔢 **API Versioning & Evolution Management**
- **ApiVersioningExtensions.cs**: 
  - URL-based versioning with automatic route generation (`/api/v1/`, `/api/v2/`)
  - Version-specific endpoint registration and documentation
  - Backward compatibility support with version deprecation warnings
  - Version-specific authorization policies and rate limiting
- **VersioningConvention.cs**: 
  - Consistent versioning patterns across all endpoints
  - Automatic version detection from URL patterns
  - Version-specific OpenAPI documentation generation

### 📊 **Health Monitoring & Observability**
- **HealthCheckExtensions.cs**: 
  - Built-in health checks for database, Redis, external APIs
  - Custom health check registration with dependency injection
  - Health check categorization (liveness, readiness, startup)
  - Detailed health check responses with diagnostic information
- **JsonHealthReporter.cs**: 
  - Structured JSON health reporting with detailed status information
  - Health check dependency mapping and status aggregation
  - Performance metrics inclusion in health responses
  - Integration with monitoring systems (Prometheus, Application Insights)

### 🔄 **Advanced JSON Processing**
- **Custom JSON Converters**: 
  - Multi-format DateTime parsing with timezone support
  - Decimal precision handling for financial applications
  - GUID format normalization and validation
  - Phone number internationalization with E.164 format support
  - Enum serialization with configurable naming policies
- **JsonExtensions.cs**: 
  - Centralized JSON configuration with custom converters
  - Performance-optimized serialization settings
  - Null handling policies and property naming strategies

## 🎯 Enterprise Design Patterns & Architectural Principles

### 1. **Clean Architecture Compliance**
- **Dependency Inversion**: API layer depends only on abstractions from Application and Domain layers
- **Separation of Concerns**: HTTP concerns isolated from business logic
- **Single Responsibility**: Each component has a single, well-defined purpose
- **Open/Closed Principle**: Extensible through inheritance and composition without modification

### 2. **Advanced Behavioral Patterns**
- **Mediator Pattern**: Complete MediatR integration for command/query separation
- **Chain of Responsibility**: Configurable middleware pipeline with ordered execution
- **Strategy Pattern**: Pluggable authentication schemes and validation strategies
- **Observer Pattern**: Event-driven architecture with domain event propagation
- **Template Method**: Base endpoint classes with overrideable lifecycle methods

### 3. **Creational Patterns**
- **Factory Pattern**: Centralized object creation for responses, errors, and configurations
- **Builder Pattern**: Fluent APIs for response construction and configuration setup
- **Dependency Injection**: Constructor injection with service lifetime management
- **Options Pattern**: Strongly-typed configuration with validation and binding

### 4. **Structural Patterns**
- **Decorator Pattern**: Middleware wrapping with cross-cutting concern injection
- **Adapter Pattern**: External service integration with consistent internal interfaces
- **Facade Pattern**: Simplified API surface over complex underlying systems
- **Composite Pattern**: Hierarchical validation and error structures

## ⚙️ Production Configuration Management

### Comprehensive Configuration Structure
```json
{
  "Api": {
    "Title": "Enterprise API",
    "Version": "v1.0",
    "Description": "Production-ready API with comprehensive features",
    "Environment": "Production",
    "ContactName": "API Team",
    "ContactEmail": "api-support@company.com",
    "LicenseName": "Proprietary",
    "TermsOfService": "https://company.com/terms"
  },
  "Authentication": {
    "Jwt": {
      "SecretKey": "${JWT_SECRET_KEY}",
      "Issuer": "company.com",
      "Audience": "api.company.com",
      "ExpirationMinutes": 60,
      "ClockSkewMinutes": 5,
      "ValidateIssuer": true,
      "ValidateAudience": true,
      "ValidateLifetime": true,
      "ValidateIssuerSigningKey": true,
      "RequireHttpsMetadata": true
    },
    "ApiKey": {
      "HeaderName": "X-API-Key",
      "QueryParameterName": "apikey",
      "AllowInQuery": false,
      "Keys": [
        {
          "Key": "${API_KEY_ADMIN}",
          "Roles": ["Admin"],
          "RateLimit": 10000
        },
        {
          "Key": "${API_KEY_CLIENT}",
          "Roles": ["Client"],
          "RateLimit": 1000
        }
      ]
    }
  },
  "Cors": {
    "DefaultPolicy": {
      "AllowedOrigins": ["https://app.company.com", "https://admin.company.com"],
      "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"],
      "AllowedHeaders": ["Content-Type", "Authorization", "X-API-Key", "X-Correlation-ID"],
      "ExposedHeaders": ["X-Correlation-ID", "X-Rate-Limit-Remaining"],
      "AllowCredentials": true,
      "PreflightMaxAge": 86400
    }
  },
  "RateLimiting": {
    "GlobalPolicy": {
      "PermitLimit": 10000,
      "Window": "00:01:00",
      "QueueLimit": 100,
      "QueueProcessingOrder": "OldestFirst"
    },
    "UserPolicy": {
      "PermitLimit": 1000,
      "Window": "00:01:00",
      "SegmentedBy": "UserId"
    },
    "IpPolicy": {
      "PermitLimit": 100,
      "Window": "00:01:00",
      "SegmentedBy": "ClientIp"
    }
  },
  "Logging": {
    "RequestResponse": {
      "Enabled": true,
      "LogRequestBody": true,
      "LogResponseBody": false,
      "MaxBodySize": 32768,
      "ExcludedPaths": ["/health", "/metrics"],
      "SensitiveHeaders": ["Authorization", "X-API-Key"]
    }
  },
  "HealthChecks": {
    "Detailed": true,
    "Timeout": "00:00:30",
    "Endpoints": {
      "Liveness": "/health/live",
      "Readiness": "/health/ready",
      "Startup": "/health/startup"
    }
  },
  "OpenApi": {
    "IncludeInProduction": false,
    "Title": "Company API Documentation",
    "Version": "1.0",
    "Description": "Comprehensive API documentation with examples",
    "DocumentationPath": "/docs",
    "JsonPath": "/docs/openapi.json"
  }
}
```

### Environment-Specific Configuration
```json
// appsettings.Development.json
{
  "Api": {
    "Environment": "Development"
  },
  "Authentication": {
    "Jwt": {
      "RequireHttpsMetadata": false,
      "ExpirationMinutes": 480
    }
  },
  "OpenApi": {
    "IncludeInProduction": true
  },
  "Logging": {
    "RequestResponse": {
      "LogResponseBody": true
    }
  }
}
```

## 📦 Comprehensive Dependency Management

### Core Framework Dependencies
- **Microsoft.AspNetCore.App** (9.0.0): Core ASP.NET Core framework
- **Microsoft.AspNetCore.Authentication.JwtBearer** (9.0.0): JWT authentication
- **Microsoft.AspNetCore.Authorization** (9.0.0): Authorization framework
- **Microsoft.AspNetCore.Mvc.Versioning** (5.0.0): API versioning support

### Validation & Documentation
- **FluentValidation.AspNetCore** (11.3.0): Advanced validation framework
- **Scalar.AspNetCore** (1.2.0): Modern API documentation UI
- **Microsoft.AspNetCore.OpenApi** (9.0.0): OpenAPI specification generation

### Monitoring & Observability
- **Microsoft.Extensions.Diagnostics.HealthChecks** (9.0.0): Health checks framework
- **Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore** (9.0.0): EF health checks
- **AspNetCore.HealthChecks.Redis** (9.0.0): Redis health checks
- **AspNetCore.HealthChecks.Uris** (9.0.0): URI health checks

### Security & Performance
- **Microsoft.AspNetCore.RateLimiting** (9.0.0): Rate limiting middleware
- **Microsoft.AspNetCore.DataProtection** (9.0.0): Data protection services

### Integration Dependencies
- **BuildingBlocks.Domain**: Domain layer abstractions and entities
- **BuildingBlocks.Application**: Application layer interfaces and DTOs
- **MediatR** (12.0.0): Mediator pattern implementation for CQRS

## 🚀 Production-Ready Features

### **High-Performance Minimal APIs**
- Lightweight endpoint definitions with optimal memory usage
- Compile-time route generation for maximum performance
- Built-in model binding with validation pipeline integration
- Automatic OpenAPI metadata generation without reflection overhead

### **Enterprise Security**
- Multi-scheme authentication with fallback strategies
- Advanced authorization policies with custom requirements
- Security header injection with CSP and HSTS support
- Rate limiting with distributed coordination

### **Comprehensive Monitoring**
- Structured logging with correlation tracking
- Performance metrics collection and exposure
- Health checks with dependency validation
- Distributed tracing integration

### **Developer Experience**
- Interactive API documentation with live testing
- Comprehensive error responses with debugging information
- Automatic API versioning with backward compatibility
- Hot-reload configuration support

## 📚 Detailed Usage Examples & Implementation Guide

### 🏗️ **Setting Up BuildingBlocks.API in Your Service**

#### 1. **Basic Service Registration**

```csharp
// Program.cs - Basic setup
using BuildingBlocks.API.Extensions;
using BuildingBlocks.Application.Extensions;
using YourService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register BuildingBlocks.API services
builder.Services.AddBuildingBlocksApi(builder.Configuration, options =>
{
    options.ServiceName = "YourService";
    options.Version = "v1.0";
    options.Description = "Your service description";
    options.EnableSwagger = builder.Environment.IsDevelopment();
    options.EnableHealthChecks = true;
    options.EnableRateLimiting = true;
    options.EnableCors = true;
});

// Add your application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure BuildingBlocks.API middleware pipeline
app.UseBuildingBlocksApi();

// Add your endpoints
app.MapYourServiceEndpoints();

app.Run();
```

#### 2. **Advanced Configuration with Custom Options**

```csharp
// Program.cs - Advanced setup
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration, options =>
{
    // API Information
    options.ServiceName = "UserService";
    options.Version = "v2.0";
    options.Description = "User management service with advanced features";
    options.ContactName = "API Team";
    options.ContactEmail = "api-team@company.com";
    
    // Feature toggles
    options.EnableSwagger = !builder.Environment.IsProduction();
    options.EnableHealthChecks = true;
    options.EnableRateLimiting = true;
    options.EnableCors = true;
    options.EnableRequestLogging = true;
    options.EnableResponseCompression = true;
    
    // Security settings
    options.RequireHttps = builder.Environment.IsProduction();
    options.EnableSecurityHeaders = true;
    
    // Performance settings
    options.EnableResponseCaching = true;
    options.EnableOutputCaching = true;
    
    // Monitoring settings
    options.EnableMetrics = true;
    options.EnableTracing = true;
});

// Configure authentication
builder.Services.AddAuthentication()
    .AddJwtBearer(builder.Configuration)
    .AddApiKey(builder.Configuration);

// Configure authorization
builder.Services.AddAuthorizationPolicies(policies =>
{
    policies.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    policies.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
});

var app = builder.Build();

// Configure middleware pipeline
app.UseBuildingBlocksApi(pipeline =>
{
    pipeline.UseSecurityHeaders();
    pipeline.UseCorrelationId();
    pipeline.UseRequestLogging();
    pipeline.UseRateLimiting();
    pipeline.UseAuthentication();
    pipeline.UseAuthorization();
    pipeline.UseResponseCompression();
    pipeline.UseOutputCaching();
});

app.Run();
```

### 🔗 **Creating Minimal API Endpoints**

#### 1. **Basic CRUD Endpoints Using EndpointBase**

```csharp
// UserEndpoints.cs
using BuildingBlocks.API.Endpoints.Base;
using BuildingBlocks.API.Responses.Base;
using YourService.Application.Users.Commands;
using YourService.Application.Users.Queries;
using YourService.Application.Users.DTOs;

public class UserEndpoints : EndpointBase
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/users")
            .WithTags("Users")
            .WithOpenApi();

        // GET /api/v1/users/{id}
        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieves a user by their unique identifier")
            .Produces<ApiResponse<UserResponse>>(200)
            .Produces<ApiResponse>(404)
            .RequireAuthorization("UserOrAdmin");

        // GET /api/v1/users
        group.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get all users")
            .WithDescription("Retrieves a paginated list of users")
            .Produces<ApiResponse<PagedResponse<UserResponse>>>(200)
            .RequireAuthorization("AdminOnly");

        // POST /api/v1/users
        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create new user")
            .WithDescription("Creates a new user account")
            .Produces<ApiResponse<UserResponse>>(201)
            .Produces<ValidationErrorResponse>(400)
            .RequireAuthorization("AdminOnly");

        // PUT /api/v1/users/{id}
        group.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update user")
            .WithDescription("Updates an existing user")
            .Produces<ApiResponse<UserResponse>>(200)
            .Produces<ApiResponse>(404)
            .Produces<ValidationErrorResponse>(400)
            .RequireAuthorization("UserOrAdmin");

        // DELETE /api/v1/users/{id}
        group.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete user")
            .WithDescription("Soft deletes a user account")
            .Produces<ApiResponse>(204)
            .Produces<ApiResponse>(404)
            .RequireAuthorization("AdminOnly");
    }

    private static async Task<IResult> GetUserById(
        Guid id,
        IMediator mediator,
        HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        try
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await mediator.Send(query);
            
            if (result == null)
            {
                return NotFound($"User with ID {id} not found", correlationId);
            }
            
            return Success(result, "User retrieved successfully", correlationId);
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving user: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> GetUsers(
        [AsParameters] GetUsersRequest request,
        IMediator mediator,
        HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        try
        {
            var query = new GetUsersQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                SearchTerm = request.SearchTerm,
                SortBy = request.SortBy,
                SortDirection = request.SortDirection
            };
            
            var result = await mediator.Send(query);
            return Success(result, "Users retrieved successfully", correlationId);
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving users: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> CreateUser(
        CreateUserRequest request,
        IMediator mediator,
        HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        try
        {
            var command = new CreateUserCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };
            
            var result = await mediator.Send(command);
            return Created(result, "User created successfully", correlationId);
        }
        catch (ValidationException ex)
        {
            return ValidationError(ex.Errors, correlationId);
        }
        catch (Exception ex)
        {
            return Error($"Error creating user: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> UpdateUser(
        Guid id,
        UpdateUserRequest request,
        IMediator mediator,
        HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        try
        {
            var command = new UpdateUserCommand
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };
            
            var result = await mediator.Send(command);
            
            if (result == null)
            {
                return NotFound($"User with ID {id} not found", correlationId);
            }
            
            return Success(result, "User updated successfully", correlationId);
        }
        catch (ValidationException ex)
        {
            return ValidationError(ex.Errors, correlationId);
        }
        catch (Exception ex)
        {
            return Error($"Error updating user: {ex.Message}", correlationId);
        }
    }

    private static async Task<IResult> DeleteUser(
        Guid id,
        IMediator mediator,
        HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        try
        {
            var command = new DeleteUserCommand { Id = id };
            var success = await mediator.Send(command);
            
            if (!success)
            {
                return NotFound($"User with ID {id} not found", correlationId);
            }
            
            return NoContent("User deleted successfully", correlationId);
        }
        catch (Exception ex)
        {
            return Error($"Error deleting user: {ex.Message}", correlationId);
        }
    }
}

// Request DTOs
public record GetUsersRequest(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string SortBy = "CreatedAt",
    string SortDirection = "desc");

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null);

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null);
```

#### 2. **Advanced Endpoints with CrudEndpoints Base Class**

```csharp
// ProductEndpoints.cs - Using CrudEndpoints for maximum reusability
using BuildingBlocks.API.Endpoints.Base;
using YourService.Domain.Products;
using YourService.Application.Products.DTOs;

public class ProductEndpoints : CrudEndpoints<Product, ProductResponse, ProductId>
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var productEndpoints = new ProductEndpoints();
        
        var group = endpoints.MapGroup("/api/v1/products")
            .WithTags("Products")
            .WithOpenApi();
        
        // Register CRUD endpoints with custom behavior
        productEndpoints.RegisterEndpoints(group, options =>
        {
            options.EnableGet = true;
            options.EnableGetAll = true;
            options.EnableCreate = true;
            options.EnableUpdate = true;
            options.EnableDelete = true;
            options.RequireAuthentication = true;
            options.GetAllRequiresRole = "User";
            options.CreateRequiresRole = "Admin";
            options.UpdateRequiresRole = "Admin";
            options.DeleteRequiresRole = "Admin";
        });
        
        // Add custom business endpoints
        group.MapPost("/{id:guid}/activate", ActivateProduct)
            .WithName("ActivateProduct")
            .WithSummary("Activate product")
            .RequireAuthorization("AdminOnly");
        
        group.MapPost("/{id:guid}/deactivate", DeactivateProduct)
            .WithName("DeactivateProduct")
            .WithSummary("Deactivate product")
            .RequireAuthorization("AdminOnly");
        
        group.MapGet("/categories/{categoryId:guid}", GetProductsByCategory)
            .WithName("GetProductsByCategory")
            .WithSummary("Get products by category")
            .RequireAuthorization("User");
    }
    
    // Override methods to customize behavior
    protected override async Task<ProductResponse?> HandleGetAsync(
        ProductId id, 
        IMediator mediator, 
        HttpContext context)
    {
        // Add custom logging or business logic
        var correlationId = GetCorrelationId(context);
        
        using var activity = ActivitySource.StartActivity("GetProduct");
        activity?.SetTag("product.id", id.Value.ToString());
        activity?.SetTag("correlation.id", correlationId);
        
        var query = new GetProductByIdQuery { Id = id };
        return await mediator.Send(query);
    }
    
    protected override async Task<ProductResponse> HandleCreateAsync(
        CreateProductRequest request, 
        IMediator mediator, 
        HttpContext context)
    {
        // Add business rule validation
        await ValidateBusinessRules(request);
        
        var command = new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            CreatedBy = GetUserId(context)
        };
        
        return await mediator.Send(command);
    }
    
    // Custom business endpoints
    private static async Task<IResult> ActivateProduct(
        Guid id,
        IMediator mediator,
        HttpContext context)
    {
        var correlationId = GetCorrelationId(context);
        
        try
        {
            var command = new ActivateProductCommand { Id = ProductId.From(id) };
            await mediator.Send(command);
            
            return Results.Ok(ApiResponse.Success(
                "Product activated successfully", 
                correlationId));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ApiResponse.Error(
                $"Error activating product: {ex.Message}", 
                correlationId));
        }
    }
    
    private async Task ValidateBusinessRules(CreateProductRequest request)
    {
        // Custom business validation logic
        if (request.Price <= 0)
        {
            throw new ValidationException("Price must be greater than zero");
        }
        
        // Additional validations...
    }
}
```

### 🔐 **Authentication & Authorization Examples**

#### 1. **JWT Authentication Setup**

```csharp
// Program.cs - JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Authentication:Jwt");
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            ClockSkew = TimeSpan.FromMinutes(5)
        };
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log authentication failures
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT authentication failed: {Error}", 
                    context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Add custom claims or validation
                var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    context.Fail("User ID not found in token");
                }
                return Task.CompletedTask;
            }
        };
    });
```

#### 2. **API Key Authentication Setup**

```csharp
// ApiKeyAuthenticationHandler.cs - Custom implementation
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string ApiKeyHeaderName = "X-API-Key";
    private readonly IApiKeyService _apiKeyService;
    
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyService apiKeyService) 
        : base(options, logger, encoder, clock)
    {
        _apiKeyService = apiKeyService;
    }
    
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }
        
        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }
        
        var apiKey = await _apiKeyService.ValidateApiKeyAsync(providedApiKey);
        if (apiKey == null)
        {
            return AuthenticateResult.Fail("Invalid API key");
        }
        
        if (!apiKey.IsActive || apiKey.ExpiryDate < DateTime.UtcNow)
        {
            return AuthenticateResult.Fail("API key is inactive or expired");
        }
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, apiKey.Name),
            new(ClaimTypes.NameIdentifier, apiKey.Id.ToString()),
            new("ApiKeyId", apiKey.Id.ToString())
        };
        
        // Add role claims
        claims.AddRange(apiKey.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return AuthenticateResult.Success(ticket);
    }
}

// Usage in endpoints
group.MapGet("/admin/stats", GetAdminStats)
    .RequireAuthorization(policy => policy.RequireRole("Admin"))
    .WithMetadata(new ApiKeyRequiredAttribute());
```

#### 3. **Advanced Authorization Policies**

```csharp
// AuthorizationPolicyExtensions.cs
public static class AuthorizationPolicyExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(
        this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin")
                      .RequireAuthenticatedUser())
            
            .AddPolicy("UserOrAdmin", policy =>
                policy.RequireRole("User", "Admin")
                      .RequireAuthenticatedUser())
            
            .AddPolicy("ResourceOwner", policy =>
                policy.RequireAuthenticatedUser()
                      .AddRequirements(new ResourceOwnerRequirement()))
            
            .AddPolicy("ApiKeyOrJwt", policy =>
                policy.AddAuthenticationSchemes(
                    JwtBearerDefaults.AuthenticationScheme,
                    ApiKeyDefaults.AuthenticationScheme)
                      .RequireAuthenticatedUser())
            
            .AddPolicy("HighValueOperation", policy =>
                policy.RequireRole("Admin")
                      .RequireClaim("permission", "high-value-operations")
                      .RequireAssertion(context =>
                          context.User.HasClaim("mfa_verified", "true")));
        
        // Register custom requirement handlers
        services.AddScoped<IAuthorizationHandler, ResourceOwnerHandler>();
        services.AddScoped<IAuthorizationHandler, RateLimitHandler>();
        
        return services;
    }
}

// Custom Authorization Requirement
public class ResourceOwnerRequirement : IAuthorizationRequirement { }

public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var resourceUserId = context.Resource as string; // Resource user ID
        
        if (userId != null && 
            (userId == resourceUserId || context.User.IsInRole("Admin")))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

### ✅ **Validation Examples**

#### 1. **FluentValidation Integration**

```csharp
// CreateUserRequestValidator.cs
using FluentValidation;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly IUserRepository _userRepository;
    
    public CreateUserRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s]+$")
            .WithMessage("First name can only contain letters and spaces");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmail)
            .WithMessage("Email already exists");
        
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[\d\s\-\(\)]+$")
            .WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
    
    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        return existingUser == null;
    }
}

// Program.cs - Register validators
builder.Services.AddFluentValidation(config =>
{
    config.RegisterValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
    config.DisableDataAnnotationsValidation = false;
    config.ImplicitlyValidateChildProperties = true;
});

// Automatic validation in endpoints
builder.Services.AddScoped<IEndpointFilter, ValidationFilter>();
```

#### 2. **Custom Validation Filter**

```csharp
// ValidationFilter.cs
public class ValidationFilter : IEndpointFilter
{
    private readonly IServiceProvider _serviceProvider;
    
    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        // Get all parameters that need validation
        var validationTasks = new List<Task<ValidationResult>>();
        
        for (int i = 0; i < context.Arguments.Count; i++)
        {
            var argument = context.Arguments[i];
            if (argument == null) continue;
            
            var argumentType = argument.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            
            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(argument);
                validationTasks.Add(validator.ValidateAsync(validationContext));
            }
        }
        
        if (validationTasks.Any())
        {
            var validationResults = await Task.WhenAll(validationTasks);
            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();
            
            if (failures.Any())
            {
                var correlationId = context.HttpContext.GetCorrelationId();
                var errors = failures.GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray());
                
                return Results.BadRequest(ApiResponse.ValidationError(
                    "Validation failed",
                    errors,
                    correlationId));
            }
        }
        
        return await next(context);
    }
}
```

### 📊 **Response Building Examples**

#### 1. **Using ApiResponseBuilder**

```csharp
// Advanced response building
public static class ResponseExamples
{
    public static IResult BuildSuccessResponse<T>(T data, string message = "Success")
    {
        var response = ApiResponseBuilder<T>
            .Success()
            .WithData(data)
            .WithMessage(message)
            .WithCorrelationId(Guid.NewGuid().ToString())
            .WithTimestamp(DateTime.UtcNow)
            .WithMetadata("version", "1.0")
            .WithMetadata("source", "UserService")
            .Build();
            
        return Results.Ok(response);
    }
    
    public static IResult BuildPagedResponse<T>(
        IEnumerable<T> items, 
        int page, 
        int pageSize, 
        int totalCount)
    {
        var pagedResponse = PagedResponse<T>.Create(
            items,
            page,
            pageSize,
            totalCount);
            
        var response = ApiResponseBuilder<PagedResponse<T>>
            .Success()
            .WithData(pagedResponse)
            .WithMessage($"Retrieved {items.Count()} items")
            .WithMetadata("totalPages", pagedResponse.TotalPages.ToString())
            .WithMetadata("hasNext", pagedResponse.HasNext.ToString())
            .WithMetadata("hasPrevious", pagedResponse.HasPrevious.ToString())
            .Build();
            
        return Results.Ok(response);
    }
    
    public static IResult BuildErrorResponse(
        string message, 
        string? details = null,
        int statusCode = 400)
    {
        var response = ErrorResponseBuilder
            .Error()
            .WithMessage(message)
            .WithDetails(details)
            .WithErrorCode($"ERR_{statusCode}")
            .WithCorrelationId(Guid.NewGuid().ToString())
            .WithTimestamp(DateTime.UtcNow)
            .Build();
            
        return Results.Problem(
            detail: response.Details,
            instance: response.CorrelationId,
            status: statusCode,
            title: response.Message,
            type: $"https://api.company.com/errors/{response.ErrorCode}");
    }
    
    public static IResult BuildValidationErrorResponse(
        Dictionary<string, string[]> errors)
    {
        var response = ErrorResponseBuilder
            .ValidationError()
            .WithMessage("Validation failed")
            .WithValidationErrors(errors)
            .WithErrorCode("VALIDATION_ERROR")
            .WithCorrelationId(Guid.NewGuid().ToString())
            .Build();
            
        return Results.BadRequest(response);
    }
}
```

### 🔄 **Middleware Usage Examples**

#### 1. **Request Logging Configuration**

```csharp
// appsettings.json
{
  "Logging": {
    "RequestResponse": {
      "Enabled": true,
      "LogLevel": "Information",
      "LogRequestBody": true,
      "LogResponseBody": false,
      "MaxBodySize": 32768,
      "ExcludedPaths": ["/health", "/metrics", "/swagger"],
      "SensitiveHeaders": ["Authorization", "X-API-Key", "Cookie"],
      "SensitiveFormFields": ["password", "confirmPassword", "token"],
      "IncludeRequestHeaders": true,
      "IncludeResponseHeaders": true,
      "LogSlowRequests": true,
      "SlowRequestThreshold": "00:00:05"
    }
  }
}

// Program.cs
app.UseRequestLogging(options =>
{
    options.LogLevel = LogLevel.Information;
    options.IncludeRequestBody = true;
    options.IncludeResponseBody = app.Environment.IsDevelopment();
    options.MaxBodySize = 32 * 1024; // 32KB
    
    // Custom request filtering
    options.ShouldLogRequest = (context) =>
    {
        // Don't log health check requests
        if (context.Request.Path.StartsWithSegments("/health"))
            return false;
            
        // Only log errors in production
        if (app.Environment.IsProduction())
            return context.Response.StatusCode >= 400;
            
        return true;
    };
    
    // Custom enrichment
    options.EnrichLog = (context, logContext) =>
    {
        logContext.Add(new PropertyEnricher("UserId", context.User?.Identity?.Name));
        logContext.Add(new PropertyEnricher("UserAgent", context.Request.Headers.UserAgent));
        logContext.Add(new PropertyEnricher("ClientIP", context.Connection.RemoteIpAddress?.ToString()));
    };
});
```

#### 2. **Rate Limiting Configuration**

```csharp
// Program.cs - Rate limiting setup
builder.Services.AddRateLimiter(options =>
{
    // Global rate limit
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: "global",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10000,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // Per-user rate limit
    options.AddPolicy("UserPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User?.Identity?.Name ?? "anonymous",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 1000,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // Per-IP rate limit
    options.AddPolicy("IpPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // API key rate limit
    options.AddPolicy("ApiKeyPolicy", httpContext =>
    {
        var apiKey = httpContext.Request.Headers["X-API-Key"].ToString();
        var permitLimit = GetApiKeyRateLimit(apiKey); // Custom logic
        
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: apiKey,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(1)
            });
    });
    
    // Custom rejection response
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        
        var response = ApiResponse.Error(
            "Rate limit exceeded. Too many requests.",
            context.HttpContext.GetCorrelationId());
            
        await context.HttpContext.Response.WriteAsJsonAsync(response, token);
    };
});

// Apply rate limiting to specific endpoints
group.MapGet("/", GetUsers)
    .RequireRateLimiting("UserPolicy");

group.MapPost("/", CreateUser)
    .RequireRateLimiting("ApiKeyPolicy")
    .RequireAuthorization("AdminOnly");
```

### 📚 **OpenAPI Documentation Examples**

#### 1. **Advanced OpenAPI Configuration**

```csharp
// Program.cs - OpenAPI setup
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<AuthenticationDocumentTransformer>();
    options.AddOperationTransformer<AuthenticationOperationTransformer>();
    options.AddSchemaTransformer<EnumSchemaTransformer>();
});

builder.Services.AddScalar(options =>
{
    options.Title = "User Management API";
    options.Theme = ScalarTheme.Purple;
    options.ShowSidebar = true;
    options.DarkMode = true;
    options.CustomCss = """
        .scalar-app {
            --scalar-color-1: #121212;
            --scalar-color-2: #1e1e1e;
            --scalar-color-3: #2d2d2d;
        }
        """;
});

// Custom document transformer
public class AuthenticationDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document, 
        OpenApiDocumentTransformerContext context, 
        CancellationToken cancellationToken)
    {
        // Add security schemes
        document.Components ??= new();
        document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["Bearer"] = new()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token"
            },
            ["ApiKey"] = new()
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "X-API-Key",
                Description = "Enter your API key"
            }
        };
        
        // Add global security requirement
        document.SecurityRequirements = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }] = new string[] { }
            }
        };
        
        return Task.CompletedTask;
    }
}
```

#### 2. **Endpoint Documentation with Examples**

```csharp
// Enhanced endpoint documentation
group.MapPost("/", CreateUser)
    .WithName("CreateUser")
    .WithSummary("Create a new user account")
    .WithDescription("""
        Creates a new user account with the provided information.
        
        **Business Rules:**
        - Email must be unique across the system
        - Phone number format must be valid (E.164 format recommended)
        - Password must meet complexity requirements
        
        **Rate Limits:**
        - 100 requests per hour for authenticated users
        - 10 requests per hour for anonymous users
        """)
    .WithOpenApi(operation =>
    {
        // Add request examples
        operation.RequestBody.Content["application/json"].Examples = new Dictionary<string, OpenApiExample>
        {
            ["basic"] = new()
            {
                Summary = "Basic user creation",
                Description = "Standard user account creation",
                Value = new CreateUserRequest(
                    "John",
                    "Doe", 
                    "john.doe@example.com",
                    "+1-555-123-4567")
            },
            ["minimal"] = new()
            {
                Summary = "Minimal required fields",
                Description = "User creation with only required fields",
                Value = new CreateUserRequest(
                    "Jane",
                    "Smith",
                    "jane.smith@example.com",
                    null)
            }
        };
        
        // Add response examples
        operation.Responses["201"].Content["application/json"].Examples = new Dictionary<string, OpenApiExample>
        {
            ["success"] = new()
            {
                Summary = "Successful creation",
                Value = new ApiResponse<UserResponse>
                {
                    Success = true,
                    Message = "User created successfully",
                    Data = new UserResponse(
                        Guid.NewGuid(),
                        "John",
                        "Doe",
                        "john.doe@example.com",
                        "+1-555-123-4567",
                        DateTime.UtcNow),
                    CorrelationId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                }
            }
        };
        
        operation.Responses["400"].Content["application/json"].Examples = new Dictionary<string, OpenApiExample>
        {
            ["validation_error"] = new()
            {
                Summary = "Validation error",
                Value = new ValidationErrorResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = new Dictionary<string, string[]>
                    {
                        ["Email"] = new[] { "Email already exists" },
                        ["PhoneNumber"] = new[] { "Invalid phone number format" }
                    },
                    CorrelationId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                }
            }
        };
        
        return operation;
    })
    .RequireAuthorization("AdminOnly")
    .RequireRateLimiting("CreateUserPolicy");
```

### 🩺 **Health Checks Implementation**

#### 1. **Comprehensive Health Check Setup**

```csharp
// Program.cs - Health checks configuration
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"))
    .AddDbContextCheck<YourDbContext>("database")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, "redis")
    .AddUrlGroup(new Uri("https://external-api.com/health"), "external-api")
    .AddCheck<CustomBusinessHealthCheck>("business-rules")
    .AddCheck<DiskSpaceHealthCheck>("disk-space")
    .AddCheck<MemoryHealthCheck>("memory-usage");

// Custom health check implementation
public class CustomBusinessHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CustomBusinessHealthCheck> _logger;
    
    public CustomBusinessHealthCheck(
        IServiceProvider serviceProvider,
        ILogger<CustomBusinessHealthCheck> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            
            // Check if we can perform basic database operations
            var userCount = await repository.CountAsync(cancellationToken);
            
            var data = new Dictionary<string, object>
            {
                ["userCount"] = userCount,
                ["lastChecked"] = DateTime.UtcNow,
                ["version"] = "1.0.0"
            };
            
            if (userCount >= 0)
            {
                return HealthCheckResult.Healthy(
                    "Business operations are functioning normally",
                    data);
            }
            
            return HealthCheckResult.Degraded(
                "Business operations are degraded",
                data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            
            return HealthCheckResult.Unhealthy(
                "Business operations are not available",
                ex,
                new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["lastChecked"] = DateTime.UtcNow
                });
        }
    }
}

// Health check endpoints mapping
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = WriteHealthCheckResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthCheckResponse
});

// Custom health check response writer
static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
{
    var response = new
    {
        Status = report.Status.ToString(),
        Checks = report.Entries.Select(entry => new
        {
            Name = entry.Key,
            Status = entry.Value.Status.ToString(),
            Description = entry.Value.Description,
            Duration = entry.Value.Duration.TotalMilliseconds,
            Data = entry.Value.Data,
            Exception = entry.Value.Exception?.Message
        }),
        TotalDuration = report.TotalDuration.TotalMilliseconds,
        Timestamp = DateTime.UtcNow
    };
    
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(response);
}
```

#### 2. **Monitoring Integration**

```csharp
// Program.cs - Monitoring setup
builder.Services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    activity.SetTag("http.request.correlation_id", 
                        request.HttpContext.GetCorrelationId());
                };
                options.EnrichWithHttpResponse = (activity, response) =>
                {
                    activity.SetTag("http.response.status_code", 
                        response.StatusCode.ToString());
                };
            })
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
                options.SetDbStatementForStoredProcedure = true;
            })
            .AddConsoleExporter()
            .AddOtlpExporter();
    })
    .WithMetrics(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddPrometheusExporter();
    });

// Custom metrics
builder.Services.AddSingleton<IMetrics>(provider =>
{
    var meterProvider = provider.GetRequiredService<MeterProvider>();
    return new Metrics(meterProvider);
});

// Metrics collection service
public class Metrics : IMetrics
{
    private readonly Meter _meter;
    private readonly Counter<int> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly Counter<int> _errorCounter;
    
    public Metrics(IMeterProvider meterProvider)
    {
        _meter = new Meter("YourService.API", "1.0.0");
        _requestCounter = _meter.CreateCounter<int>("api_requests_total");
        _requestDuration = _meter.CreateHistogram<double>("api_request_duration_seconds");
        _errorCounter = _meter.CreateCounter<int>("api_errors_total");
    }
    
    public void IncrementRequestCount(string endpoint, string method, int statusCode)
    {
        _requestCounter.Add(1, new KeyValuePair<string, object?>[]
        {
            new("endpoint", endpoint),
            new("method", method),
            new("status_code", statusCode.ToString())
        });
    }
    
    public void RecordRequestDuration(string endpoint, string method, double duration)
    {
        _requestDuration.Record(duration, new KeyValuePair<string, object?>[]
        {
            new("endpoint", endpoint),
            new("method", method)
        });
    }
    
    public void IncrementErrorCount(string endpoint, string method, string errorType)
    {
        _errorCounter.Add(1, new KeyValuePair<string, object?>[]
        {
            new("endpoint", endpoint),
            new("method", method),
            new("error_type", errorType)
        });
    }
}
```

### 🔧 **Best Practices & Guidelines**

#### 1. **Error Handling Best Practices**

```csharp
// Comprehensive error handling middleware
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    private readonly IMetrics _metrics;
    
    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment environment,
        IMetrics metrics)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _metrics = metrics;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.GetCorrelationId();
        var endpoint = context.Request.Path.Value ?? "unknown";
        var method = context.Request.Method;
        
        // Log the exception with correlation ID
        _logger.LogError(exception, 
            "Unhandled exception occurred. CorrelationId: {CorrelationId}, Endpoint: {Endpoint}",
            correlationId, endpoint);
        
        // Record metrics
        _metrics.IncrementErrorCount(endpoint, method, exception.GetType().Name);
        
        // Determine response based on exception type
        var (statusCode, message, details) = exception switch
        {
            ValidationException validationEx => (400, "Validation failed", 
                _environment.IsDevelopment() ? validationEx.Message : null),
            UnauthorizedAccessException => (401, "Unauthorized access", null),
            KeyNotFoundException => (404, "Resource not found", null),
            ArgumentException argEx => (400, "Invalid argument", 
                _environment.IsDevelopment() ? argEx.Message : null),
            InvalidOperationException opEx => (409, "Operation not allowed", 
                _environment.IsDevelopment() ? opEx.Message : null),
            TimeoutException => (408, "Request timeout", null),
            TaskCanceledException => (408, "Request timeout", null),
            _ => (500, "An error occurred while processing your request", 
                _environment.IsDevelopment() ? exception.Message : null)
        };
        
        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        if (_environment.IsDevelopment() && details != null)
        {
            response.Details = details;
        }
        
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        await context.Response.WriteAsJsonAsync(response);
    }
}
```

#### 2. **Performance Optimization Patterns**

```csharp
// Response caching implementation
public static class ResponseCachingExtensions
{
    public static IServiceCollection AddApiResponseCaching(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024; // 1MB
            options.UseCaseSensitivePaths = true;
        });
        
        services.AddOutputCache(options =>
        {
            // Default policy
            options.AddBasePolicy(builder => 
                builder.Expire(TimeSpan.FromMinutes(5)));
            
            // User-specific caching
            options.AddPolicy("UserCache", builder =>
                builder
                    .Expire(TimeSpan.FromMinutes(2))
                    .VaryByValue((context, _) => 
                        context.User.Identity?.Name ?? "anonymous"));
            
            // Long-term caching for reference data
            options.AddPolicy("ReferenceData", builder =>
                builder
                    .Expire(TimeSpan.FromHours(1))
                    .Tag("reference-data"));
        });
        
        return services;
    }
}

// Usage in endpoints
group.MapGet("/reference/countries", GetCountries)
    .CacheOutput("ReferenceData")
    .WithName("GetCountries")
    .WithSummary("Get list of countries");

group.MapGet("/user/profile", GetUserProfile)
    .CacheOutput("UserCache")
    .RequireAuthorization()
    .WithName("GetUserProfile");

// Cache invalidation
public class CacheInvalidationService : ICacheInvalidationService
{
    private readonly IOutputCacheStore _cacheStore;
    
    public CacheInvalidationService(IOutputCacheStore cacheStore)
    {
        _cacheStore = cacheStore;
    }
    
    public async Task InvalidateUserCache(string userId)
    {
        await _cacheStore.EvictByTagAsync($"user-{userId}", CancellationToken.None);
    }
    
    public async Task InvalidateReferenceData()
    {
        await _cacheStore.EvictByTagAsync("reference-data", CancellationToken.None);
    }
}
```

#### 3. **Security Best Practices**

```csharp
// Comprehensive security headers middleware
app.Use(async (context, next) =>
{
    // Security headers
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Permissions-Policy", 
        "camera=(), microphone=(), geolocation=()");
    
    if (context.Request.IsHttps)
    {
        context.Response.Headers.Add("Strict-Transport-Security", 
            "max-age=31536000; includeSubDomains");
    }
    
    // Content Security Policy
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https: data:; " +
        "connect-src 'self' https:; " +
        "frame-ancestors 'none';");
    
    await next();
});

// Input sanitization
public class InputSanitizationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Sanitize string inputs
        for (int i = 0; i < context.Arguments.Count; i++)
        {
            if (context.Arguments[i] is string stringValue)
            {
                context.Arguments[i] = SanitizeInput(stringValue);
            }
            else if (context.Arguments[i] != null)
            {
                SanitizeObject(context.Arguments[i]);
            }
        }
        
        return await next(context);
    }
    
    private static string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        // Remove potentially dangerous characters
        return input
            .Replace("<script", "&lt;script", StringComparison.OrdinalIgnoreCase)
            .Replace("</script>", "&lt;/script&gt;", StringComparison.OrdinalIgnoreCase)
            .Replace("javascript:", "", StringComparison.OrdinalIgnoreCase)
            .Replace("vbscript:", "", StringComparison.OrdinalIgnoreCase)
            .Replace("onload=", "", StringComparison.OrdinalIgnoreCase)
            .Replace("onerror=", "", StringComparison.OrdinalIgnoreCase);
    }
    
    private static void SanitizeObject(object obj)
    {
        var properties = obj.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(string) && p.CanWrite);
        
        foreach (var property in properties)
        {
            var value = property.GetValue(obj) as string;
            if (!string.IsNullOrEmpty(value))
            {
                property.SetValue(obj, SanitizeInput(value));
            }
        }
    }
}
```

### 🚨 **Troubleshooting Guide**

#### 1. **Common Issues and Solutions**

```csharp
// Diagnostic middleware for troubleshooting
public class DiagnosticMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DiagnosticMiddleware> _logger;
    
    public DiagnosticMiddleware(RequestDelegate next, ILogger<DiagnosticMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
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
            
            // Log slow requests
            if (stopwatch.ElapsedMilliseconds > 5000) // 5 seconds
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
            
            // Log large responses
            if (responseBody.Length > 1024 * 1024) // 1MB
            {
                _logger.LogWarning(
                    "Large response detected: {Method} {Path} returned {Size} bytes",
                    context.Request.Method,
                    context.Request.Path,
                    responseBody.Length);
            }
            
            // Copy response back to original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}

// Health check for troubleshooting
public class TroubleshootingHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    
    public TroubleshootingHealthCheck(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var diagnostics = new Dictionary<string, object>();
        
        try
        {
            // Check memory usage
            var memoryUsage = GC.GetTotalMemory(false);
            diagnostics["memoryUsage"] = $"{memoryUsage / 1024 / 1024} MB";
            
            // Check thread pool
            ThreadPool.GetAvailableThreads(out var workerThreads, out var ioThreads);
            diagnostics["availableWorkerThreads"] = workerThreads;
            diagnostics["availableIOThreads"] = ioThreads;
            
            // Check configuration
            diagnostics["environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            diagnostics["machineOSVersion"] = Environment.OSVersion.ToString();
            diagnostics["processorCount"] = Environment.ProcessorCount;
            
            // Check service registrations
            var serviceCount = _serviceProvider.GetServices<object>().Count();
            diagnostics["registeredServices"] = serviceCount;
            
            return HealthCheckResult.Healthy("Diagnostics collected successfully", diagnostics);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to collect diagnostics", ex, diagnostics);
        }
    }
}
```

#### 2. **Performance Monitoring**

```csharp
// Performance monitoring endpoint
app.MapGet("/admin/performance", async (
    IServiceProvider serviceProvider,
    HttpContext context) =>
{
    if (!context.User.IsInRole("Admin"))
    {
        return Results.Forbid();
    }
    
    var performanceData = new
    {
        Memory = new
        {
            TotalMemory = GC.GetTotalMemory(false),
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2)
        },
        ThreadPool = new
        {
            ThreadPool.CompletedWorkItemCount,
            ThreadPool.PendingWorkItemCount,
            ThreadPool.ThreadCount
        },
        Process = new
        {
            WorkingSet = Environment.WorkingSet,
            ProcessorTime = Environment.TickCount,
            StartTime = Process.GetCurrentProcess().StartTime
        },
        AspNetCore = new
        {
            RequestsPerSecond = GetRequestsPerSecond(),
            AverageResponseTime = GetAverageResponseTime(),
            ErrorRate = GetErrorRate()
        }
    };
    
    return Results.Ok(ApiResponse<object>.Success(
        performanceData, 
        "Performance data retrieved successfully",
        context.GetCorrelationId()));
})
.WithName("GetPerformanceMetrics")
.WithTags("Admin", "Monitoring")
.RequireAuthorization("AdminOnly");

// Configuration validation endpoint
app.MapGet("/admin/config-validation", (
    IConfiguration configuration,
    HttpContext context) =>
{
    if (!context.User.IsInRole("Admin"))
    {
        return Results.Forbid();
    }
    
    var validationResults = new List<object>();
    
    // Validate database connection
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    validationResults.Add(new
    {
        Setting = "Database Connection",
        IsValid = !string.IsNullOrEmpty(connectionString),
        Value = string.IsNullOrEmpty(connectionString) ? "Not configured" : "Configured"
    });
    
    // Validate JWT settings
    var jwtSecret = configuration["Authentication:Jwt:SecretKey"];
    validationResults.Add(new
    {
        Setting = "JWT Secret",
        IsValid = !string.IsNullOrEmpty(jwtSecret) && jwtSecret.Length >= 32,
        Value = string.IsNullOrEmpty(jwtSecret) ? "Not configured" : 
               jwtSecret.Length < 32 ? "Too short" : "Valid"
    });
    
    // Validate Redis connection
    var redisConnection = configuration.GetConnectionString("Redis");
    validationResults.Add(new
    {
        Setting = "Redis Connection",
        IsValid = !string.IsNullOrEmpty(redisConnection),
        Value = string.IsNullOrEmpty(redisConnection) ? "Not configured" : "Configured"
    });
    
    return Results.Ok(ApiResponse<object>.Success(
        new { ValidationResults = validationResults },
        "Configuration validation completed",
        context.GetCorrelationId()));
})
.WithName("ValidateConfiguration")
.WithTags("Admin", "Configuration")
.RequireAuthorization("AdminOnly");
```

## 🔗 Clean Architecture Integration Points

### **Inbound Dependencies** (What this layer uses)
- **Application Layer**: 
  - `IMediator` for command/query dispatching
  - Application service interfaces for business operations
  - DTOs for data transfer and API contracts
  - Validation interfaces for request validation
- **Domain Layer**: 
  - Domain entities for API model mapping
  - Domain exceptions for error handling
  - Value objects for type-safe parameters

### **Outbound Dependencies** (What uses this layer)
- **Client Applications**: Web applications, mobile apps, third-party integrations
- **API Gateways**: For request routing and transformation
- **Monitoring Systems**: For health checks and metrics collection
- **Documentation Tools**: For API specification consumption

### **Infrastructure Integration**
- **Database Layer**: Through application services and health checks
- **Caching Layer**: For response caching and rate limiting storage
- **Message Queues**: For asynchronous operation handling
- **External APIs**: Through HTTP client abstraction and health monitoring

### **Deployment & Operations**
- **Container Orchestration**: Kubernetes, Docker Swarm integration
- **Service Discovery**: Consul, Eureka integration through health checks
- **Load Balancers**: Health check endpoints for traffic routing
- **Monitoring Platforms**: Prometheus, Grafana, Application Insights integration