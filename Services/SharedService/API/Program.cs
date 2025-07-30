using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.OpenApi.Extensions;
using SharedService.API.Endpoints;
using SharedService.Application;
using SharedService.Domain;
using SharedService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add BuildingBlocks.API services (includes endpoints, OpenAPI, health checks, CORS, JSON converters, etc.)
builder.Services.AddBuildingBlocksApi(builder.Configuration, builder.Environment);

// Add domain layers
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

// Use BuildingBlocks.API middleware pipeline (includes error handling, security, CORS, etc.)
app.UseBuildingBlocksApi(builder.Configuration);

// Ensure database is created in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    // Database will be created if it doesn't exist
}

// Map endpoints
app.MapCurrencyEndpoints();
app.MapCountryEndpoints();

app.Run(); 