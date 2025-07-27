using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Extensions;
using System.Reflection;

namespace ContactService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatorWithAssemblies(assembly);

        return services;
    }
}