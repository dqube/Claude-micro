using Microsoft.Extensions.DependencyInjection;

namespace PatientService.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}