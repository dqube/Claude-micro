using Microsoft.Extensions.DependencyInjection;

namespace SalesService.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}