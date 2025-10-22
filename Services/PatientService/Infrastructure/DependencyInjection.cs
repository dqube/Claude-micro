using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Context;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientService.Application.IntegrationEvents;
using PatientService.Application.IntegrationEvents.Handlers;
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

        // Configure Kafka Message Bus with Fluent API and Subscriptions
        services.AddMessageBus(
            options =>
            {
                // Kafka Connection Configuration
                options.BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
                options.GroupId = "patient-service-group";
                options.ClientId = "patient-service";
                options.AutoOffsetReset = "earliest";
                options.EnableAutoCommit = false;
                
                // Performance and Reliability
                options.CompressionType = "snappy";
                options.EnableIdempotence = true;
                options.SessionTimeoutMs = 30000;
                options.MaxPollIntervalMs = 300000;
                options.MessageTimeoutMs = 30000;
                options.RequestTimeoutMs = 30000;
                options.RequiredAcks = -1; // Wait for all replicas
            },
            subscriptions =>
            {
                // Subscribe to integration events from other microservices
                
                // Subscribe to AppointmentCreatedIntegrationEvent from AppointmentService
                // When an appointment is created, PatientService will be notified to update patient records
                subscriptions.Subscribe<AppointmentCreatedIntegrationEvent,
                                       AppointmentCreatedIntegrationEventHandler>("appointment.created");
                
               
            });

        // Inbox/Outbox services are automatically registered by AddBuildingBlocksApi

        return services;
    }
}