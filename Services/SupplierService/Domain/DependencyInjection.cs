using Microsoft.Extensions.DependencyInjection;

namespace SupplierService.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Domain services can be registered here if needed
        // For now, we only have domain entities and value objects
        
        return services;
    }
} 