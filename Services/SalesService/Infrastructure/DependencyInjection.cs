using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Extensions;
using SalesService.Domain.Repositories;
using SalesService.Infrastructure.Persistence;
using SalesService.Infrastructure.Repositories;

namespace SalesService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Add Database Context
        services.AddDbContext<SalesDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("SalesServiceDb");
            }
        });
        
        // Register SalesDbContext as IDbContext for BuildingBlocks services
        services.AddScoped<BuildingBlocks.Infrastructure.Data.Context.IDbContext, SalesDbContext>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register Sale Repository
        services.AddScoped<ISaleRepository, SaleRepository>();
        
        // Register Return Repository
        services.AddScoped<IReturnRepository, ReturnRepository>();

        return services;
    }
}