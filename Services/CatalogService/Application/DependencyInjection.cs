using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BuildingBlocks.Application.CQRS.Events;
using CatalogService.Application.EventHandlers;
using CatalogService.Domain.Events;

namespace CatalogService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Register domain event handlers with DomainEventWrapper pattern
        services.AddScoped<IEventHandler<DomainEventWrapper<ProductCreatedEvent>>, ProductCreatedEventHandler>();
        services.AddScoped<IEventHandler<DomainEventWrapper<ProductDiscontinuedEvent>>, ProductDiscontinuedEventHandler>();
        services.AddScoped<IEventHandler<DomainEventWrapper<ProductPricingUpdatedEvent>>, ProductPricingUpdatedEventHandler>();
        services.AddScoped<IEventHandler<DomainEventWrapper<ProductCategoryCreatedEvent>>, ProductCategoryCreatedEventHandler>();
        services.AddScoped<IEventHandler<DomainEventWrapper<ProductCategoryDeletedEvent>>, ProductCategoryDeletedEventHandler>();
        services.AddScoped<IEventHandler<DomainEventWrapper<TaxConfigurationCreatedEvent>>, TaxConfigurationCreatedEventHandler>();
        
        return services;
    }
}