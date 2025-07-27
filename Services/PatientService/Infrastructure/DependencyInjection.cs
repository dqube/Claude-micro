using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using BuildingBlocks.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;
using PatientService.Infrastructure.Persistence;
using PatientService.Infrastructure.Repositories;

namespace PatientService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context
        services.AddDbContext<PatientDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("PatientServiceDb");
            }
        });

        // Register DbContext as IDbContext for inbox/outbox services
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<PatientDbContext>());

        // Register Repositories
        services.AddScoped<IRepository<Patient, PatientId>, PatientRepository>();
        services.AddScoped<IReadOnlyRepository<Patient, PatientId>, PatientRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Inbox/Outbox services
        services.AddScoped<IInboxService, InboxService>();
        services.AddScoped<IOutboxService, OutboxService>();

        return services;
    }
}