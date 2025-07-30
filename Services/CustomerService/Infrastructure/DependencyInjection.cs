using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;
using CustomerService.Domain.Repositories;
using CustomerService.Infrastructure.Persistence;
using CustomerService.Infrastructure.Repositories;

namespace CustomerService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context
        services.AddDbContext<CustomerDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("CustomerServiceDb");
            }
        });
        
        // Register CustomerDbContext as IDbContext for BuildingBlocks services
        services.AddScoped<BuildingBlocks.Infrastructure.Data.Context.IDbContext, CustomerDbContext>();

        // Register Customer Repository
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRepository<Customer, CustomerId>, CustomerRepository>();
        services.AddScoped<IReadOnlyRepository<Customer, CustomerId>, CustomerRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Inbox/Outbox services are automatically registered by AddBuildingBlocksApi

        return services;
    }
} 