using BuildingBlocks.Application.Extensions;
using BuildingBlocks.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using PatientService.API.Endpoints;
using PatientService.Application.Commands;
using PatientService.Application.EventHandlers;
using PatientService.Application.Queries;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;
using PatientService.Infrastructure.Persistence;
using PatientService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Patient Service API", Version = "v1" });
});

// Add basic services we need - skip complex infra services for now
//builder.Services.AddApplicationLayer();

// Add Database Context
builder.Services.AddDbContext<PatientDbContext>(options =>
{
    // For development, use InMemory database for simplicity
    options.UseInMemoryDatabase("PatientServiceDb");
    
    // For production, use SQL Server
    // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Repositories
builder.Services.AddScoped<IRepository<Patient, PatientId>, PatientRepository>();
builder.Services.AddScoped<IReadOnlyRepository<Patient, PatientId>, PatientRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Command Handlers
builder.Services.AddScoped<CreatePatientCommandHandler>();
builder.Services.AddScoped<UpdatePatientContactCommandHandler>();

// Add Query Handlers
builder.Services.AddScoped<GetPatientByIdQueryHandler>();
builder.Services.AddScoped<GetPatientsQueryHandler>();

// Add Event Handlers
builder.Services.AddScoped<PatientCreatedEventHandler>();

// Add Health Checks
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<PatientDbContext>();

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
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient Service API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
    
    app.UseCors("AllowAll");
    
    // Ensure database is created in development
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<PatientDbContext>();
        context.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();

// Map minimal API endpoints
app.MapPatientEndpoints();

// Add Health Check endpoints
app.MapHealthChecks("/health");

// Add root endpoint
app.MapGet("/", () => Results.Redirect("/swagger"))
    .WithName("Root")
    .WithSummary("Redirect to API documentation")
    .ExcludeFromDescription();

app.Run();