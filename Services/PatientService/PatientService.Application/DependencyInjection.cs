using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Extensions;
using System.Reflection;

namespace PatientService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register handlers using BuildingBlocks extension methods with assembly parameter
        services.AddCommandHandlers(assembly);
        services.AddQueryHandlers(assembly);
        services.AddEventHandlers(assembly);

        return services;
    }
}