using BuildingBlocks.API.Extensions;
using StoreService.API.Endpoints;
using StoreService.Application;
using StoreService.Domain;
using StoreService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBuildingBlocksApi(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseBuildingBlocksApi(builder.Configuration);

// Map application-specific endpoints
app.MapStoreEndpoints();
app.MapRegisterEndpoints();

// Add root endpoint redirect to documentation (only in development)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
        .WithName("Root")
        .WithSummary("Redirect to API documentation")
        .ExcludeFromDescription();
}

app.Run();
