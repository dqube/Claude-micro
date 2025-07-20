# BuildingBlocks.API Architecture Specification

## Overview
The BuildingBlocks.API library provides essential API layer components for ASP.NET Core applications using Minimal APIs and following Clean Architecture principles. This layer serves as a simple presentation tier, handling HTTP requests, responses, and core cross-cutting concerns.

## Directory Structure

```
BuildingBlocks.API/
├── Endpoints/
│   ├── Base/
│   │   ├── EndpointBase.cs
│   │   ├── CrudEndpoints.cs
│   │   └── QueryEndpoints.cs
│   ├── Extensions/
│   │   ├── EndpointRouteBuilderExtensions.cs
│   │   └── MinimalApiExtensions.cs
│   └── Conventions/
│       ├── ApiEndpointConvention.cs
│       └── VersioningEndpointConvention.cs
├── Middleware/
│   ├── ErrorHandling/
│   │   ├── GlobalExceptionMiddleware.cs
│   │   ├── ErrorResponse.cs
│   │   └── ProblemDetailsFactory.cs
│   ├── Logging/
│   │   ├── RequestLoggingMiddleware.cs
│   │   └── CorrelationIdMiddleware.cs
│   └── Security/
│       ├── SecurityHeadersMiddleware.cs
│       └── RateLimitingMiddleware.cs
├── Responses/
│   ├── Base/
│   │   ├── ApiResponse.cs
│   │   ├── PagedResponse.cs
│   │   └── ErrorResponse.cs
│   └── Builders/
│       ├── ApiResponseBuilder.cs
│       └── ErrorResponseBuilder.cs
├── Authentication/
│   ├── JWT/
│   │   ├── JwtAuthenticationExtensions.cs
│   │   └── JwtBearerOptionsSetup.cs
│   └── ApiKey/
│       ├── ApiKeyAuthenticationExtensions.cs
│       └── ApiKeyAuthenticationHandler.cs
├── Validation/
│   ├── Validators/
│   │   ├── RequestValidator.cs
│   │   └── PaginationValidator.cs
│   ├── Extensions/
│   │   ├── ValidationExtensions.cs
│   │   └── FluentValidationExtensions.cs
│   └── Results/
│       ├── ValidationResult.cs
│       └── ValidationError.cs
├── OpenApi/
│   ├── Configuration/
│   │   ├── OpenApiConfiguration.cs
│   │   ├── ScalarConfiguration.cs
│   │   └── ApiDocumentationOptions.cs
│   ├── Filters/
│   │   ├── AuthorizationOperationFilter.cs
│   │   └── DefaultResponseOperationFilter.cs
│   └── Extensions/
│       ├── OpenApiExtensions.cs
│       └── ScalarExtensions.cs
├── Versioning/
│   ├── Extensions/
│   │   ├── ApiVersioningExtensions.cs
│   │   └── VersionedEndpointExtensions.cs
│   └── Conventions/
│       ├── VersioningConvention.cs
│       └── EndpointVersioningConvention.cs
├── Health/
│   ├── Extensions/
│   │   ├── HealthCheckExtensions.cs
│   │   └── HealthEndpointExtensions.cs
│   └── Reporters/
│       ├── JsonHealthReporter.cs
│       └── SimpleHealthReporter.cs
├── Configuration/
│   ├── Options/
│   │   ├── ApiOptions.cs
│   │   ├── CorsOptions.cs
│   │   ├── AuthenticationOptions.cs
│   │   └── RateLimitingOptions.cs
│   └── Extensions/
│       ├── ConfigurationExtensions.cs
│       └── OptionsExtensions.cs
├── Extensions/
│   ├── ServiceCollection/
│   │   ├── ApiServiceCollectionExtensions.cs
│   │   ├── AuthenticationExtensions.cs
│   │   ├── CorsExtensions.cs
│   │   ├── OpenApiExtensions.cs
│   │   ├── VersioningExtensions.cs
│   │   ├── RateLimitingExtensions.cs
│   │   ├── HealthCheckExtensions.cs
│   │   └── ValidationExtensions.cs
│   ├── WebApplication/
│   │   ├── ApiWebApplicationExtensions.cs
│   │   ├── MiddlewareExtensions.cs
│   │   ├── SecurityExtensions.cs
│   │   ├── ErrorHandlingExtensions.cs
│   │   ├── OpenApiExtensions.cs
│   │   └── HealthCheckExtensions.cs
│   └── HttpContext/
│       ├── HttpContextExtensions.cs
│       ├── ClaimsPrincipalExtensions.cs
│       ├── RequestExtensions.cs
│       └── ResponseExtensions.cs
└── Utilities/
    ├── Helpers/
    │   ├── ResponseHelper.cs
    │   ├── ValidationHelper.cs
    │   └── CorrelationHelper.cs
    ├── Constants/
    │   ├── ApiConstants.cs
    │   ├── HttpConstants.cs
    │   └── HeaderConstants.cs
    └── Factories/
        ├── ResponseFactory.cs
        └── ErrorFactory.cs
```

## Key Components

### 1. **Minimal API Endpoints**
- **EndpointBase**: Base functionality for minimal API endpoints
- **CrudEndpoints**: Standard CRUD operations for minimal APIs
- **QueryEndpoints**: Query-only operations for minimal APIs
- **Endpoint Extensions**: Helper methods for route building

### 2. **Middleware Pipeline**
- **Error Handling**: Global exception handling with problem details
- **Logging**: Request/response logging with correlation IDs
- **Security**: Essential security headers and rate limiting

### 3. **Authentication**
- **JWT Authentication**: Token-based authentication for minimal APIs
- **API Key Authentication**: Simple API key-based authentication

### 4. **Response Handling**
- **Standardized Responses**: Consistent API response format
- **Error Responses**: Structured error handling
- **Pagination**: Simple paging responses

### 5. **Validation**
- **FluentValidation Integration**: Request validation
- **Validation Results**: Standardized validation error responses

### 6. **OpenAPI Documentation**
- **Scalar Integration**: Modern API documentation UI
- **OpenAPI Configuration**: Automatic API documentation generation
- **Documentation Filters**: Enhanced documentation features

### 7. **API Versioning**
- **URL Versioning**: Simple URL-based API versioning
- **Version Extensions**: Helper methods for versioned endpoints

### 8. **Health Checks**
- **Simple Health Endpoints**: Basic health monitoring
- **Health Reporters**: JSON and simple text health reporting

## Design Patterns

1. **Clean Architecture**: Separation of concerns with dependency inversion
2. **Mediator Pattern**: Request/response handling via MediatR
3. **Chain of Responsibility**: Middleware pipeline
4. **Factory Pattern**: Response and error object creation
5. **Builder Pattern**: Fluent API configuration

## Configuration

Simple configuration through appsettings.json:

```json
{
  "Api": {
    "Title": "My API",
    "Version": "v1",
    "Description": "API Description"
  },
  "Authentication": {
    "Jwt": {
      "Issuer": "MyApp",
      "Audience": "MyApp-API"
    }
  },
  "Cors": {
    "AllowedOrigins": ["https://localhost:3000"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["*"]
  },
  "RateLimiting": {
    "PermitLimit": 100,
    "Window": "00:01:00"
  }
}
```

## Dependencies

This layer depends on:
- **BuildingBlocks.Domain**: Domain entities and business rules
- **BuildingBlocks.Application**: Application services and contracts
- **ASP.NET Core**: Web framework and hosting
- **MediatR**: Mediator pattern implementation
- **FluentValidation**: Validation framework
- **Scalar**: Modern OpenAPI documentation UI

## Key Features

- **Minimal APIs**: Lightweight endpoint definitions
- **Scalar Documentation**: Beautiful, interactive API documentation
- **Clean Response Models**: Consistent API responses
- **Simple Authentication**: JWT and API key support
- **Basic Validation**: FluentValidation integration
- **Essential Middleware**: Error handling, logging, security
- **API Versioning**: URL-based versioning support
- **Health Checks**: Basic health monitoring

## Integration Points

The API layer integrates with:
- **Application Layer**: Via dependency injection and MediatR
- **Infrastructure Layer**: Through service registration
- **Domain Layer**: For entity mapping and business rules
- **External Tools**: Scalar for documentation, monitoring tools