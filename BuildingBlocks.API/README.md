# BuildingBlocks.API

A comprehensive API layer building blocks library for ASP.NET Core 9 applications using Minimal APIs, following Clean Architecture principles.

## 🚀 Features

### 🏗️ Minimal API Support
- **Base Classes**: `EndpointBase` and `CrudEndpoints` for rapid API development
- **Response Models**: Standardized API responses with consistent structure
- **Route Conventions**: Consistent endpoint routing and naming
- **Validation**: Integrated request validation with detailed error responses

### 🔒 Authentication & Security
- **JWT Authentication**: Bearer token authentication with configurable options
- **API Key Authentication**: Simple API key-based authentication
- **CORS**: Cross-origin resource sharing configuration
- **Security Headers**: HTTPS, HSTS, and security header middleware
- **Rate Limiting**: Configurable rate limiting per endpoint

### 🛡️ Middleware Pipeline
- **Error Handling**: Global exception handling with structured error responses
- **Request Logging**: Comprehensive request/response logging with correlation IDs
- **Correlation ID**: Request correlation tracking for distributed systems
- **Security Middleware**: Security headers, CORS, and rate limiting

### 📚 Documentation & Monitoring
- **OpenAPI**: Native .NET 9 OpenAPI integration with Swagger
- **Scalar**: Modern API documentation interface (replaces Swagger UI)
- **Health Checks**: Built-in health monitoring for APIs, databases, and external services
- **Versioning**: URL-based API versioning with OpenAPI documentation

### 🔧 Configuration & Validation
- **Options Pattern**: Strongly-typed configuration with validation
- **FluentValidation**: Comprehensive request validation framework
- **Configuration Extensions**: Easy setup and binding of configuration sections

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