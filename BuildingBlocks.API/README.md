# BuildingBlocks.API

A lightweight API layer building blocks library for ASP.NET Core applications using Minimal APIs.

## Features

### 🏗️ Minimal API Support
- **Base Classes**: EndpointBase and CrudEndpoints for rapid API development
- **Response Models**: Standardized API responses with consistent structure
- **Validation**: Integrated request validation with detailed error responses

### 🔒 Authentication & Security
- **JWT Authentication**: Bearer token authentication with configurable options
- **API Key Authentication**: Simple API key-based authentication
- **CORS**: Cross-origin resource sharing configuration

### 🛡️ Middleware Pipeline
- **Error Handling**: Global exception handling with structured error responses
- **Request Logging**: Comprehensive request/response logging
- **Correlation ID**: Request correlation tracking for distributed systems

### 📚 Documentation
- **OpenAPI**: Native .NET 9 OpenAPI integration
- **Scalar**: Modern API documentation interface
- **Configuration**: Flexible API documentation setup

## Installation

Add the project reference to your API layer:

```xml
<ProjectReference Include="..\BuildingBlocks.API\BuildingBlocks.API.csproj" />
```

## Quick Start

Setup in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add API services
builder.Services.AddBuildingBlocksApi(builder.Configuration);

var app = builder.Build();

// Configure API pipeline
app.UseBuildingBlocksApi(builder.Configuration);

app.Run();
```

## Configuration

Configure `appsettings.json`:

```json
{
  "Api": {
    "Title": "My API",
    "Version": "v1",
    "Description": "API Documentation"
  },
  "Authentication": {
    "Jwt": {
      "SecretKey": "your-secret-key",
      "Issuer": "MyApp",
      "Audience": "MyApp-API"
    }
  },
  "Cors": {
    "AllowedOrigins": ["https://localhost:3000"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["*"]
  }
}
```

## Core Components

### Endpoint Base Classes

```csharp
public class ProductEndpoints : CrudEndpoints<Product, ProductDto, Guid>
{
    public ProductEndpoints(IMediator mediator) : base(mediator) { }
    
    protected override string Tag => "Products";
    protected override string Route => "/api/products";
}
```

### API Responses

```csharp
public class ProductController : EndpointBase
{
    public async Task<IResult> GetProduct(Guid id)
    {
        var product = await GetProductAsync(id);
        
        if (product == null)
            return ApiResponse.NotFound("Product not found");
            
        return ApiResponse.Success(product);
    }
}
```

### Validation

```csharp
public class CreateProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Validation is automatically applied using FluentValidation
public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

## Key Services

### Authentication
- **JWT Authentication**: Bearer token validation
- **API Key Authentication**: Simple key-based auth
- **Authentication Extensions**: Easy setup methods

### Middleware
- **GlobalExceptionMiddleware**: Centralized error handling
- **RequestLoggingMiddleware**: Request/response logging
- **CorrelationIdMiddleware**: Request correlation tracking

### OpenAPI
- **OpenApiConfiguration**: API documentation setup
- **ScalarConfiguration**: Modern documentation UI
- **API Documentation**: Automated endpoint documentation

### Response Handling
- **ApiResponse**: Standardized response wrapper
- **ApiResponseBuilder**: Fluent response building
- **Error Responses**: Consistent error formatting

## Dependencies

This library depends on:
- **BuildingBlocks.Domain**: Domain layer abstractions
- **BuildingBlocks.Application**: Application layer interfaces
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication
- **FluentValidation.AspNetCore**: Request validation
- **Scalar.AspNetCore**: API documentation

## Architecture Integration

```
📁 YourApplication/
├── 📁 Domain/ (BuildingBlocks.Domain)
├── 📁 Application/ (BuildingBlocks.Application)
├── 📁 Infrastructure/ (BuildingBlocks.Infrastructure)
├── 📁 API/ (BuildingBlocks.API) ← This library
└── 📁 Tests/
```