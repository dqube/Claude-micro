using Microsoft.Extensions.DependencyInjection;

namespace PaymentService.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Domain layer typically doesn't need external dependencies
        // Business rules and domain services can be registered here if needed
        
        return services;
    }
} 