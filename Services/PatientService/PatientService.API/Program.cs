using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.OpenApi.Extensions;
using PatientService.API.Converters;
using PatientService.API.Endpoints;
using PatientService.Application;
using PatientService.Domain;
using PatientService.Infrastructure;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApiDocumentation(builder.Configuration);
// Add BuildingBlocks.API services (includes endpoints, OpenAPI, health checks, CORS, etc.)
builder.Services.AddBuildingBlocksApi(builder.Configuration);

// Configure custom JSON options (merges with BuildingBlocks.API defaults)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
});

// Add domain layers
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();
var app = builder.Build();
// OpenAPI documentation

// Use BuildingBlocks.API middleware pipeline (includes error handling, security, CORS, etc.)
app.UseBuildingBlocksApi(builder.Configuration);

// Ensure database is created in development
if (app.Environment.IsDevelopment())
{
    // using (var scope = app.Services.CreateScope())
    // {
    //     var context = scope.ServiceProvider.GetRequiredService<PatientService.Infrastructure.Persistence.PatientDbContext>();
    //     context.Database.EnsureCreated();
    // }
}

// Map application-specific endpoints
app.MapPatientEndpoints();

// Add root endpoint redirect to documentation (only in development)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
        .WithName("Root")
        .WithSummary("Redirect to API documentation")
        .ExcludeFromDescription();
}

app.Run();