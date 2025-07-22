using BuildingBlocks.Application.Extensions;
using PatientService.API.Endpoints;
using PatientService.Application;
using PatientService.Domain;
using PatientService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Patient Service API", Version = "v1" });
});

// Add layers
builder.Services.AddDomain();
PatientService.Application.DependencyInjection.AddApplication(builder.Services);
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
        var context = scope.ServiceProvider.GetRequiredService<PatientService.Infrastructure.Persistence.PatientDbContext>();
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