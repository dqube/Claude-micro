using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Infrastructure.Data.Context;
using SharedService.Domain.Repositories;
using SharedService.Infrastructure.Repositories;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context
        services.AddDbContext<SharedDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("SharedServiceDb");
            }
        });
        
        // Register SharedDbContext as IDbContext for BuildingBlocks services
        services.AddScoped<IDbContext, SharedDbContext>();

        // Register Currency Repository
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();

        // Register Country Repository
        services.AddScoped<ICountryRepository, CountryRepository>();

        return services;
    }
} 