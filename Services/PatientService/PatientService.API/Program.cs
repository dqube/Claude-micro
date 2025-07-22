using PatientService.API.Endpoints;
using PatientService.API.Converters;
using PatientService.Application;
using PatientService.Domain;
using PatientService.Infrastructure;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure JSON options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.AllowTrailingCommas = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
});

// Add layers
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Health Checks
builder.Services.AddHealthChecks();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    app.UseCors("AllowAll");
    
    // Ensure database is created in development
    // using (var scope = app.Services.CreateScope())
    // {
    //     var context = scope.ServiceProvider.GetRequiredService<PatientService.Infrastructure.Persistence.PatientDbContext>();
    //     context.Database.EnsureCreated();
    // }
}

app.UseHttpsRedirection();

// Map minimal API endpoints
app.MapPatientEndpoints();

// Add Health Check endpoints
app.MapHealthChecks("/health");

// Add root endpoint
app.MapGet("/", () => Results.Redirect("/scalar/v1"))
    .WithName("Root")
    .WithSummary("Redirect to API documentation")
    .ExcludeFromDescription();

app.Run();