using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.OpenApi.Extensions;
using ContactService.API.Endpoints;
using ContactService.Application;
using ContactService.Domain;
using ContactService.Infrastructure;

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
}

app.MapContactEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
        .WithName("Root")
        .WithSummary("Redirect to API documentation")
        .ExcludeFromDescription();
}

app.Run();