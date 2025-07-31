using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Domain.Repositories;
using ReportingService.Infrastructure.Persistence;
using ReportingService.Infrastructure.Repositories;

namespace ReportingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context
        services.AddDbContext<ReportingDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("ReportingServiceDb");
            }
        });
        
        // Register ReportingDbContext as IDbContext for BuildingBlocks services
        services.AddScoped<BuildingBlocks.Infrastructure.Data.Context.IDbContext, ReportingDbContext>();

        // Register SalesSnapshot Repository
        services.AddScoped<ISalesSnapshotRepository, SalesSnapshotRepository>();
        services.AddScoped<IRepository<SalesSnapshot, SalesSnapshotId>, SalesSnapshotRepository>();
        services.AddScoped<IReadOnlyRepository<SalesSnapshot, SalesSnapshotId>, SalesSnapshotRepository>();

        // Register InventorySnapshot Repository
        services.AddScoped<IInventorySnapshotRepository, InventorySnapshotRepository>();
        services.AddScoped<IRepository<InventorySnapshot, InventorySnapshotId>, InventorySnapshotRepository>();
        services.AddScoped<IReadOnlyRepository<InventorySnapshot, InventorySnapshotId>, InventorySnapshotRepository>();

        // Register PromotionEffectiveness Repository
        services.AddScoped<IPromotionEffectivenessRepository, PromotionEffectivenessRepository>();
        services.AddScoped<IRepository<PromotionEffectiveness, PromotionEffectivenessId>, PromotionEffectivenessRepository>();
        services.AddScoped<IReadOnlyRepository<PromotionEffectiveness, PromotionEffectivenessId>, PromotionEffectivenessRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register BuildingBlocks OutboxService instead of custom implementation
        services.AddScoped<BuildingBlocks.Application.Outbox.IOutboxService, BuildingBlocks.Infrastructure.Services.OutboxService>();

        return services;
    }
} 