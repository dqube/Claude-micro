using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InventoryService.Domain.Entities;
using InventoryService.Domain.ValueObjects;
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Repositories;

namespace InventoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase("InventoryServiceDb");
            }
        });
        
        services.AddScoped<BuildingBlocks.Infrastructure.Data.Context.IDbContext, InventoryDbContext>();

        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        services.AddScoped<IRepository<InventoryItem, InventoryItemId>, InventoryItemRepository>();
        services.AddScoped<IReadOnlyRepository<InventoryItem, InventoryItemId>, InventoryItemRepository>();

        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IRepository<StockMovement, StockMovementId>, StockMovementRepository>();
        services.AddScoped<IReadOnlyRepository<StockMovement, StockMovementId>, StockMovementRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register BuildingBlocks OutboxService instead of custom implementation
        services.AddScoped<BuildingBlocks.Application.Outbox.IOutboxService, BuildingBlocks.Infrastructure.Services.OutboxService>();

        return services;
    }
}