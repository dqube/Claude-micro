using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.OpenApi.Extensions;
using PromotionService.API.Endpoints;
using PromotionService.Application;
using PromotionService.Domain;
using PromotionService.Infrastructure;

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
    // using (var scope = app.Services.CreateScope())
    // {
    //     var context = scope.ServiceProvider.GetRequiredService<PromotionService.Infrastructure.Persistence.PromotionDbContext>();
    //     context.Database.EnsureCreated();
    // }
}

// Map application-specific endpoints
app.MapDiscountTypeEndpoints();
app.MapDiscountCampaignEndpoints();
app.MapDiscountRuleEndpoints();
app.MapPromotionEndpoints();
app.MapPromotionProductEndpoints();

// Add root endpoint redirect to documentation (only in development)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
        .WithName("Root")
        .WithSummary("Redirect to API documentation")
        .ExcludeFromDescription();
}

app.Run(); 