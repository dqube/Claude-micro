using Microsoft.Extensions.DependencyInjection;

namespace StoreService.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Domain services would be registered here if needed
        // Currently, the domain layer is pure and doesn't require any services
        
        return services;
    }
} 