using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.OpenApi.Extensions;
using SalesService.API.Endpoints;
using SalesService.Application;
using SalesService.Domain;
using SalesService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuildingBlocksApi(builder.Configuration, builder.Environment);

builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseBuildingBlocksApi(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    // using (var scope = app.Services.CreateScope())
    // {
    //     var context = scope.ServiceProvider.GetRequiredService<SalesService.Infrastructure.Persistence.SalesDbContext>();
    //     context.Database.EnsureCreated();
    // }
}

app.MapSaleEndpoints();
app.MapReturnEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
        .WithName("Root")
        .WithSummary("Redirect to API documentation")
        .ExcludeFromDescription();
}

app.Run();