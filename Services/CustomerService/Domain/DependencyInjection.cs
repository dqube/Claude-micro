using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Domain services can be registered here if needed
        // Currently, the domain layer is self-contained with entities and value objects
        
        return services;
    }
} 