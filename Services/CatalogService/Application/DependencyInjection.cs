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
        
        // Register domain event handlers
        services.AddScoped<IEventHandler<ProductCreatedEvent>, ProductCreatedEventHandler>();
        services.AddScoped<IEventHandler<ProductDiscontinuedEvent>, ProductDiscontinuedEventHandler>();
        services.AddScoped<IEventHandler<ProductReactivatedEvent>, ProductReactivatedEventHandler>();
        services.AddScoped<IEventHandler<ProductPricingUpdatedEvent>, ProductPricingUpdatedEventHandler>();
        services.AddScoped<IEventHandler<ProductTaxabilityUpdatedEvent>, ProductTaxabilityUpdatedEventHandler>();
        services.AddScoped<IEventHandler<ProductCategoryMovedEvent>, ProductCategoryMovedEventHandler>();
        services.AddScoped<IEventHandler<ProductInventoryTrackingEnabledEvent>, ProductInventoryTrackingEnabledEventHandler>();
        services.AddScoped<IEventHandler<ProductInventoryTrackingDisabledEvent>, ProductInventoryTrackingDisabledEventHandler>();
        services.AddScoped<IEventHandler<ProductBarcodeAddedEvent>, ProductBarcodeAddedEventHandler>();
        services.AddScoped<IEventHandler<ProductBarcodeRemovedEvent>, ProductBarcodeRemovedEventHandler>();
        services.AddScoped<IEventHandler<ProductCountryPricingUpdatedEvent>, ProductCountryPricingUpdatedEventHandler>();
        services.AddScoped<IEventHandler<ProductBasicInfoUpdatedEvent>, ProductBasicInfoUpdatedEventHandler>();
        services.AddScoped<IEventHandler<ProductCategoryCreatedEvent>, ProductCategoryCreatedEventHandler>();
        services.AddScoped<IEventHandler<ProductCategoryDeletedEvent>, ProductCategoryDeletedEventHandler>();
        services.AddScoped<IEventHandler<ProductCategoryNameUpdatedEvent>, ProductCategoryNameUpdatedEventHandler>();
        services.AddScoped<IEventHandler<ProductCategoryParentUpdatedEvent>, ProductCategoryParentUpdatedEventHandler>();
        services.AddScoped<IEventHandler<TaxConfigurationCreatedEvent>, TaxConfigurationCreatedEventHandler>();
        services.AddScoped<IEventHandler<TaxConfigurationUpdatedEvent>, TaxConfigurationUpdatedEventHandler>();
        services.AddScoped<IEventHandler<TaxConfigurationCategoryUpdatedEvent>, TaxConfigurationCategoryUpdatedEventHandler>();
        
        return services;
    }
}