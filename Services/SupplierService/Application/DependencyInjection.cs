using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BuildingBlocks.Application.Extensions;
using BuildingBlocks.Application.CQRS.Mediator;

namespace SupplierService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Register mediator with all handlers from this assembly
        services.AddMediatorWithAssemblies(assembly);
        
        // Register BuildingBlocks application layer services
        services.AddApplicationLayer();
        
        return services;
    }
} 