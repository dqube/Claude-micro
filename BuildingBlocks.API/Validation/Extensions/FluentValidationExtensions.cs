using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.API.Validation.Results;

namespace BuildingBlocks.API.Validation.Extensions;

public static class FluentValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(FluentValidationExtensions));
        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services, params System.Reflection.Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);
        
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
        return services;
    }

    public static BuildingBlocks.API.Validation.Results.ValidationResult ToValidationResult(this FluentValidation.Results.ValidationResult fluentResult)
    {
        ArgumentNullException.ThrowIfNull(fluentResult);
        
        if (fluentResult.IsValid)
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Success();
        }

        var errors = fluentResult.Errors.Select(e => new ValidationError(
            e.PropertyName,
            e.ErrorMessage,
            e.AttemptedValue,
            e.ErrorCode)).ToArray();

        return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(errors);
    }

    public static IDictionary<string, string[]> ToErrorDictionary(this FluentValidation.Results.ValidationResult fluentResult)
    {
        ArgumentNullException.ThrowIfNull(fluentResult);
        
        return fluentResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
    }

    public static async Task<T?> ValidateAndThrowAsync<T>(this IValidator<T> validator, T instance)
    {
        ArgumentNullException.ThrowIfNull(validator);
        
        var result = await validator.ValidateAsync(instance);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
        return instance;
    }

    public static IRuleBuilderOptions<T, TProperty> WithCustomErrorCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder,
        string errorCode)
    {
        return ruleBuilder.WithErrorCode(errorCode);
    }
}