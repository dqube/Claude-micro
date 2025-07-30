using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreService.Domain.Entities;
using StoreService.Domain.Repositories;
using StoreService.Domain.ValueObjects;
using StoreService.Infrastructure.Persistence;
using StoreService.Infrastructure.Repositories;

namespace StoreService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Add Database Context
        services.AddDbContext<StoreDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("StoreServiceDb");
            }
        });

        // Register DbContext as IDbContext for inbox/outbox services
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<StoreDbContext>());

        // Register Store repositories
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IRepository<Store, StoreId>, StoreRepository>();
        services.AddScoped<IReadOnlyRepository<Store, StoreId>, StoreRepository>();

        // Register Register repositories
        services.AddScoped<IRegisterRepository, RegisterRepository>();
        services.AddScoped<IRepository<Register, RegisterId>, RegisterRepository>();
        services.AddScoped<IReadOnlyRepository<Register, RegisterId>, RegisterRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Inbox/Outbox services are automatically registered by AddBuildingBlocksApi

        return services;
    }
} 