using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BuildingBlocks.API.Validation.Extensions;
using System.Reflection;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class ValidationExtensions
{
    public static IServiceCollection AddApiValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<FluentValidationExtensions>();
        return services;
    }

    public static IServiceCollection AddApiValidation(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
        return services;
    }

    public static IServiceCollection AddApiValidation(this IServiceCollection services, params Type[] markerTypes)
    {
        foreach (var markerType in markerTypes)
        {
            services.AddValidatorsFromAssemblyContaining(markerType);
        }
        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining<FluentValidationExtensions>();
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services, params Type[] markerTypes)
    {
        foreach (var markerType in markerTypes)
        {
            services.AddValidatorsFromAssemblyContaining(markerType);
        }
        return services;
    }

    public static IServiceCollection AddValidationBehaviors(this IServiceCollection services)
    {
        // This would typically add MediatR validation behaviors
        // For now, it's a placeholder for future implementation
        return services;
    }

    public static IServiceCollection AddDataAnnotationValidation(this IServiceCollection services)
    {
        services.AddSingleton<IValidationService, DataAnnotationValidationService>();
        return services;
    }

    public static IServiceCollection AddCompositeValidation(this IServiceCollection services)
    {
        services.AddFluentValidation();
        services.AddDataAnnotationValidation();
        services.AddSingleton<IValidationService, CompositeValidationService>();
        return services;
    }
}

public interface IValidationService
{
    Task<BuildingBlocks.API.Validation.Results.ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default);
}

public class DataAnnotationValidationService : IValidationService
{
    public Task<BuildingBlocks.API.Validation.Results.ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default)
    {
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj!);
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        
        bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj!, validationContext, validationResults, true);
        
        if (isValid)
        {
            return Task.FromResult(BuildingBlocks.API.Validation.Results.ValidationResult.Success());
        }

        var errors = validationResults
            .SelectMany(vr => vr.MemberNames.Select(memberName => 
                new BuildingBlocks.API.Validation.Results.ValidationError(memberName, vr.ErrorMessage ?? "Validation error")))
            .ToArray();

        return Task.FromResult(BuildingBlocks.API.Validation.Results.ValidationResult.Failure(errors));
    }
}

public class CompositeValidationService : IValidationService
{
    private readonly IEnumerable<IValidationService> _validationServices;

    public CompositeValidationService(IEnumerable<IValidationService> validationServices)
    {
        _validationServices = validationServices;
    }

    public async Task<BuildingBlocks.API.Validation.Results.ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default)
    {
        var result = BuildingBlocks.API.Validation.Results.ValidationResult.Success();
        
        foreach (var service in _validationServices)
        {
            var serviceResult = await service.ValidateAsync(obj, cancellationToken);
            result = result.Combine(serviceResult);
        }
        
        return result;
    }
}