using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Extensions;
using FluentValidation;
using System.Reflection;

namespace InventoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddMediatorWithAssemblies(assembly);
        
        services.AddApplicationLayer();
        
        services.AddValidatorsFromAssembly(assembly);
        
        return services;
    }
}