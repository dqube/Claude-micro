using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;
using PaymentService.Domain.Repositories;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Repositories;

namespace PaymentService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context
        services.AddDbContext<PaymentDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("PaymentServiceDb");
            }
        });
        
        // Register PaymentDbContext as IDbContext for BuildingBlocks services
        services.AddScoped<BuildingBlocks.Infrastructure.Data.Context.IDbContext, PaymentDbContext>();

        // Register PaymentProcessor Repository
        services.AddScoped<IPaymentProcessorRepository, PaymentProcessorRepository>();
        services.AddScoped<IRepository<PaymentProcessor, PaymentProcessorId>, PaymentProcessorRepository>();
        services.AddScoped<IReadOnlyRepository<PaymentProcessor, PaymentProcessorId>, PaymentProcessorRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register BuildingBlocks OutboxService instead of custom implementation
        services.AddScoped<BuildingBlocks.Application.Outbox.IOutboxService, BuildingBlocks.Infrastructure.Services.OutboxService>();

        return services;
    }
} 