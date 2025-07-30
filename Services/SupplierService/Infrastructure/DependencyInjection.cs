using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Infrastructure.Data.Context;
using SupplierService.Domain.Repositories;
using SupplierService.Infrastructure.Persistence;
using SupplierService.Infrastructure.Repositories;

namespace SupplierService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context
        services.AddDbContext<SupplierDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("SupplierServiceDb");
            }
        });
        
        // Register SupplierDbContext as IDbContext for BuildingBlocks services
        services.AddScoped<IDbContext, SupplierDbContext>();

        // Register Supplier Repository
        services.AddScoped<ISupplierRepository, SupplierRepository>();

        // Register Supplier Contact Repository
        services.AddScoped<ISupplierContactRepository, SupplierContactRepository>();

        // Register Supplier Address Repository
        services.AddScoped<ISupplierAddressRepository, SupplierAddressRepository>();

        // Register Purchase Order Repository
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

        // Register Purchase Order Detail Repository
        services.AddScoped<IPurchaseOrderDetailRepository, PurchaseOrderDetailRepository>();

        return services;
    }
} 