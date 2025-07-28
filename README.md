# Your Awesome Project - Clean Architecture Solution

A comprehensive .NET solution implementing Clean Architecture principles with Domain-Driven Design (DDD), Command Query Responsibility Segregation (CQRS), and Event-Driven Architecture patterns.

## 🏗️ Solution Architecture

This solution follows Clean Architecture principles with clear separation of concerns across multiple layers:

```
📁 Your Awesome Project/
├── 🏗️ BuildingBlocks/                    # Shared infrastructure and patterns
│   ├── 📦 BuildingBlocks.Domain/          # Domain layer foundation
│   ├── ⚙️ BuildingBlocks.Application/     # Application layer patterns
│   ├── 🔧 BuildingBlocks.Infrastructure/  # Infrastructure implementations
│   └── 🌐 BuildingBlocks.API/             # API layer utilities
├── 🏢 Services/                           # Microservices
│   ├── 👤 AuthService/                    # Authentication & Authorization
│   ├── 🏥 PatientService/                 # Patient management
│   └── 📊 [Additional Services]/          # Other domain services
├── 🧪 Tests/                              # Test projects
├── 📚 Documentation/                      # Architecture & API docs
└── 🚀 Deployment/                         # Docker & Kubernetes configs
```

## 🌟 Key Features

### 🏛️ Clean Architecture Implementation
- **Domain Layer**: Pure business logic with no external dependencies
- **Application Layer**: Use cases and business workflows
- **Infrastructure Layer**: Data access, external services, and frameworks
- **Presentation Layer**: APIs, controllers, and user interfaces

### 🎯 Domain-Driven Design (DDD)
- **Aggregate Roots**: Maintain business invariants and consistency
- **Value Objects**: Immutable domain concepts with structural equality
- **Domain Events**: Event-driven communication within domain boundaries
- **Business Rules**: Encapsulated domain logic with validation
- **Strongly Typed IDs**: Type-safe entity identifiers

### 📬 CQRS & Event Sourcing
- **Commands**: Write operations that modify system state
- **Queries**: Read operations with pagination and filtering
- **Events**: Domain and integration events for loose coupling
- **Event Handlers**: Asynchronous event processing

### 🔄 Event-Driven Architecture
- **Domain Events**: Internal domain notifications
- **Integration Events**: Cross-service communication
- **Inbox/Outbox Pattern**: Reliable message delivery
- **Saga Pattern**: Long-running process orchestration

### 🛡️ Security & Authentication
- **JWT Authentication**: Secure token-based authentication
- **API Key Authentication**: Service-to-service authentication
- **Role-Based Access Control**: Permission-based authorization
- **Multi-Tenant Support**: Organization-aware data isolation

### 📊 Observability & Monitoring
- **Structured Logging**: Serilog with enrichers and correlation IDs
- **OpenTelemetry**: Distributed tracing and metrics
- **Health Checks**: Comprehensive system health monitoring
- **Performance Monitoring**: Request/response time tracking

## 🚀 Quick Start

### Prerequisites
- **.NET 9.0 SDK** or later
- **Docker** (for containerized services)
- **SQL Server** or **PostgreSQL** (for data persistence)
- **Redis** (for caching and distributed locking)

### 1. Clone and Setup
```bash
git clone <repository-url>
cd your-awesome-project

# Restore dependencies
dotnet restore

# Setup database (example for SQL Server)
dotnet ef database update --project Services/AuthService/Infrastructure
dotnet ef database update --project Services/PatientService/Infrastructure
```

### 2. Configuration
Update `appsettings.json` files in each service:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourApp;Trusted_Connection=true;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "YourApp",
    "Audience": "YourApp-API",
    "ExpiryMinutes": 60
  },
  "OpenTelemetry": {
    "ServiceName": "YourApp",
    "ServiceVersion": "1.0.0"
  }
}
```

### 3. Run Services
```bash
# Run AuthService
cd Services/AuthService/API
dotnet run

# Run PatientService
cd Services/PatientService/API
dotnet run

# Or use Docker Compose
docker-compose up -d
```

### 4. Access APIs
- **AuthService**: https://localhost:5001
- **PatientService**: https://localhost:5002
- **Swagger UI**: Available at `/swagger` endpoint for each service

## 📦 BuildingBlocks Overview

### 🏗️ BuildingBlocks.Domain
The foundation layer providing essential domain patterns:

- **Entities & Aggregate Roots**: Base classes with domain event support
- **Value Objects**: Immutable domain concepts
- **Strongly Typed IDs**: Type-safe identifiers with JSON serialization
- **Business Rules**: Encapsulated validation logic
- **Specifications**: Query object pattern for complex filtering
- **Domain Events**: Event-driven domain notifications
- **Repository Contracts**: Data access abstractions

[📖 Full Documentation](./BuildingBlocks.Domain/README.md)

### ⚙️ BuildingBlocks.Application
Application layer patterns and orchestration:

- **CQRS**: Commands, queries, and handlers
- **Pipeline Behaviors**: Cross-cutting concerns (logging, validation, caching)
- **Mediator Pattern**: Request routing and pipeline orchestration
- **Inbox/Outbox**: Reliable message processing
- **Saga Pattern**: Long-running process management
- **Validation Framework**: Comprehensive request validation

[📖 Full Documentation](./BuildingBlocks.Application/README.md)

### 🔧 BuildingBlocks.Infrastructure
Infrastructure implementations and external integrations:

- **Entity Framework Core**: Advanced ORM configuration with interceptors
- **Caching**: Multi-tier caching (Memory, Distributed, Redis)
- **Authentication**: JWT and API Key authentication
- **Messaging**: Event bus and message bus implementations
- **Background Services**: Hosted services for continuous processing
- **Monitoring**: Health checks, metrics, and observability

[📖 Full Documentation](./BuildingBlocks.Infrastructure/README.md)

### 🌐 BuildingBlocks.API
API layer utilities and middleware:

- **Minimal APIs**: Endpoint routing and configuration
- **Middleware**: Error handling, logging, security headers
- **Response Building**: Standardized API responses
- **Validation**: Request validation integration
- **OpenAPI**: Swagger documentation and customization
- **Rate Limiting**: API throttling and protection

[📖 Full Documentation](./BuildingBlocks.API/README.md)

## 🏢 Services Architecture

### Service Template Structure
Each microservice follows the same clean architecture pattern:

```
ServiceName/
├── 📁 API/                    # Presentation Layer
│   ├── Endpoints/             # Minimal API endpoints
│   ├── Middleware/            # Custom middleware
│   ├── Validators/            # Request validators
│   └── Program.cs             # Service configuration
├── 📁 Application/            # Application Layer
│   ├── Commands/              # Write operations
│   ├── Queries/               # Read operations
│   ├── EventHandlers/         # Event processing
│   ├── DTOs/                  # Data transfer objects
│   └── Services/              # Application services
├── 📁 Domain/                 # Domain Layer
│   ├── Aggregates/            # Domain aggregates
│   ├── ValueObjects/          # Value objects
│   ├── Events/                # Domain events
│   ├── Specifications/        # Business rules
│   └── Repositories/          # Repository contracts
├── 📁 Infrastructure/         # Infrastructure Layer
│   ├── Persistence/           # Database context & repositories
│   ├── Services/              # External service integrations
│   ├── Messaging/             # Message handling
│   └── Configuration/         # Infrastructure setup
└── 📁 Tests/                  # Test projects
    ├── Unit/                  # Unit tests
    ├── Integration/           # Integration tests
    └── Acceptance/            # End-to-end tests
```

### 👤 AuthService
Handles authentication, authorization, and user management:

**Key Features:**
- User registration and authentication
- Role and permission management
- JWT token generation and validation
- Password policies and security
- Multi-factor authentication support

**Endpoints:**
- `POST /auth/login` - User authentication
- `POST /auth/register` - User registration
- `POST /auth/refresh` - Token refresh
- `GET /users/{id}` - User profile
- `PUT /users/{id}/roles` - Role management

### 🏥 PatientService
Manages patient information and medical records:

**Key Features:**
- Patient registration and profiles
- Medical history tracking
- Appointment scheduling
- Document management
- Privacy and compliance (HIPAA)

**Endpoints:**
- `POST /patients` - Create patient
- `GET /patients/{id}` - Get patient details
- `PUT /patients/{id}` - Update patient
- `GET /patients/{id}/history` - Medical history
- `POST /patients/{id}/appointments` - Schedule appointment

## 🧪 Testing Strategy

### Test Pyramid Implementation
```
        🔺 E2E Tests (Few)
       🔺🔺 Integration Tests (Some)
    🔺🔺🔺🔺 Unit Tests (Many)
```

### Unit Tests
- **Domain Logic**: Entity behavior, business rules, value objects
- **Application Logic**: Command/query handlers, validators
- **Fast Execution**: No external dependencies
- **High Coverage**: Target 80%+ code coverage

### Integration Tests
- **Database Integration**: Repository implementations
- **API Endpoints**: Full request/response testing
- **Message Handling**: Event and message processing
- **External Services**: Third-party integrations

### End-to-End Tests
- **Business Scenarios**: Complete user workflows
- **Cross-Service Communication**: Integration testing
- **Performance Testing**: Load and stress testing
- **Security Testing**: Authentication and authorization

### Test Utilities
```csharp
// Example test setup
public class PatientServiceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PatientServiceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreatePatient_ShouldReturnCreatedResult()
    {
        // Arrange
        var command = new CreatePatientCommand
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "john.doe@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/patients", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var patientId = await response.Content.ReadFromJsonAsync<PatientId>();
        patientId.Should().NotBeNull();
    }
}
```

## 📊 Monitoring & Observability

### Structured Logging
```csharp
// Example logging with correlation
[HttpPost]
public async Task<ActionResult<PatientId>> CreatePatient(CreatePatientCommand command)
{
    using var activity = ActivitySource.StartActivity("CreatePatient");
    activity?.SetTag("patient.email", command.Email);
    
    _logger.LogInformation("Creating patient for {Email}", command.Email);
    
    var patientId = await _mediator.Send(command);
    
    _logger.LogInformation("Patient {PatientId} created successfully", patientId);
    return CreatedAtAction(nameof(GetPatient), new { id = patientId }, patientId);
}
```

### OpenTelemetry Configuration
```csharp
// In Program.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
               .AddEntityFrameworkCoreInstrumentation()
               .AddRedisInstrumentation()
               .AddJaegerExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
               .AddPrometheusExporter();
    });
```

### Health Checks
```csharp
// Comprehensive health checking
builder.Services.AddHealthChecks()
    .AddDbContext<ApplicationDbContext>()
    .AddRedis(connectionString)
    .AddUrlGroup(new Uri("https://external-api.com/health"), "External API")
    .AddCheck<CustomHealthCheck>("Custom Health Check");
```

## 🚀 Deployment

### Docker Support
Each service includes Docker support with multi-stage builds:

```dockerfile
# Example Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Services/PatientService/API/PatientService.API.csproj", "Services/PatientService/API/"]
COPY ["BuildingBlocks/", "BuildingBlocks/"]
RUN dotnet restore "Services/PatientService/API/PatientService.API.csproj"

COPY . .
WORKDIR "/src/Services/PatientService/API"
RUN dotnet build "PatientService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PatientService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientService.API.dll"]
```

### Docker Compose
```yaml
version: '3.8'
services:
  auth-service:
    build:
      context: .
      dockerfile: Services/AuthService/API/Dockerfile
    ports:
      - "5001:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=AuthService;User=sa;Password=YourPassword123!
    depends_on:
      - sqlserver
      - redis

  patient-service:
    build:
      context: .
      dockerfile: Services/PatientService/API/Dockerfile
    ports:
      - "5002:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=PatientService;User=sa;Password=YourPassword123!
    depends_on:
      - sqlserver
      - redis

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    ports:
      - "1433:1433"

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
```

### Kubernetes Deployment
```yaml
# Example Kubernetes deployment
apiVersion: apps/v1
kind: Deployment
metadata:
  name: patient-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: patient-service
  template:
    metadata:
      labels:
        app: patient-service
    spec:
      containers:
      - name: patient-service
        image: patient-service:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: db-connection
              key: connection-string
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
```

## 📈 Performance & Scalability

### Caching Strategy
- **Memory Cache**: Fast, local caching for frequently accessed data
- **Distributed Cache**: Redis for shared caching across instances
- **Cache Invalidation**: Tag-based and pattern-based invalidation
- **Cache-Aside Pattern**: Manual cache management with fallback to data source

### Database Optimization
- **Entity Framework Core**: Optimized queries with appropriate tracking
- **Connection Pooling**: Efficient database connection management
- **Read/Write Separation**: CQRS with dedicated read models
- **Database Migrations**: Version-controlled schema changes

### Message Processing
- **Inbox/Outbox Pattern**: Reliable message delivery with exactly-once processing
- **Background Services**: Continuous message processing with error handling
- **Event Sourcing**: Complete audit trail of domain changes
- **Saga Pattern**: Long-running process coordination

## 🔒 Security

### Authentication & Authorization
- **JWT Tokens**: Secure, stateless authentication
- **Refresh Tokens**: Long-lived authentication renewal
- **Role-Based Access**: Permission-based authorization
- **API Keys**: Service-to-service authentication

### Data Protection
- **Encryption at Rest**: Database encryption
- **Encryption in Transit**: HTTPS/TLS
- **Sensitive Data**: PII and PHI protection
- **Audit Logging**: Complete audit trail

### Security Headers
```csharp
// Security middleware configuration
app.UseSecurityHeaders(options =>
{
    options.AddContentSecurityPolicy("default-src 'self'");
    options.AddStrictTransportSecurity(365);
    options.AddXFrameOptions(XFrameOptions.Deny);
    options.AddXContentTypeOptions();
    options.AddReferrerPolicy(ReferrerPolicy.StrictOriginWhenCrossOrigin);
});
```

## 🤝 Contributing

### Development Guidelines
1. **Follow Clean Architecture**: Maintain clear separation of concerns
2. **Write Tests**: Unit, integration, and end-to-end tests
3. **Document APIs**: OpenAPI/Swagger documentation
4. **Code Reviews**: All changes require peer review
5. **Security First**: Security considerations in all features

### Code Standards
- **C# Coding Standards**: Follow Microsoft conventions
- **Architecture Decisions**: Document significant architectural choices
- **Performance**: Consider performance implications
- **Maintainability**: Write clean, readable code

### Pull Request Process
1. Create feature branch from `main`
2. Implement changes with tests
3. Update documentation
4. Create pull request with description
5. Code review and approval
6. Merge to `main`

## 📚 Additional Resources

### Documentation
- [Architecture Decision Records](./Documentation/ADR/)
- [API Documentation](./Documentation/API/)
- [Deployment Guide](./Documentation/Deployment.md)
- [Troubleshooting Guide](./Documentation/Troubleshooting.md)

### Learning Resources
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋‍♂️ Support

For questions, issues, or contributions:
- **Issues**: GitHub Issues for bug reports and feature requests
- **Discussions**: GitHub Discussions for questions and ideas
- **Documentation**: Comprehensive documentation in each BuildingBlocks project

---

**Built with ❤️ using Clean Architecture, DDD, and modern .NET practices**