using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.OpenApi.Extensions;
using SupplierService.API.Endpoints;
using SupplierService.Application;
using SupplierService.Domain;
using SupplierService.Infrastructure;

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

// Map API endpoints
app.MapSupplierEndpoints();
app.MapSupplierContactEndpoints();
app.MapSupplierAddressEndpoints();
app.MapPurchaseOrderEndpoints();

app.Run(); 