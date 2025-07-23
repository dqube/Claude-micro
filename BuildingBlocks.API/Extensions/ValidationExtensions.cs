using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.API.Validation.Results;
using BuildingBlocks.API.Utilities.Factories;
using BuildingBlocks.API.Utilities.Helpers;

namespace BuildingBlocks.API.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddApiValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(ValidationExtensions));
        return services;
    }

    public static IServiceCollection AddApiValidation(this IServiceCollection services, params System.Reflection.Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
        return services;
    }

    public static async Task<IResult> ValidateAsync<T>(
        this IValidator<T> validator,
        T model,
        string? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(validator);
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToErrorDictionary();
            return ResponseFactory.ValidationError(errors, correlationId);
        }
        return Results.Ok();
    }

    public static async Task<T?> ValidateAndThrowAsync<T>(
        this IValidator<T> validator,
        T model)
    {
        ArgumentNullException.ThrowIfNull(validator);
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        return model;
    }

    public static BuildingBlocks.API.Validation.Results.ValidationResult ToApiValidationResult(
        this FluentValidation.Results.ValidationResult fluentResult)
    {
        ArgumentNullException.ThrowIfNull(fluentResult);
        if (fluentResult.IsValid)
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Success();
        }
        var errors = fluentResult.Errors.Select(e => new BuildingBlocks.API.Validation.Results.ValidationError(
            e.PropertyName,
            e.ErrorMessage,
            e.AttemptedValue,
            e.ErrorCode)).ToArray();
        return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(errors);
    }

    public static IDictionary<string, string[]> ToErrorDictionary(
        this FluentValidation.Results.ValidationResult fluentResult)
    {
        ArgumentNullException.ThrowIfNull(fluentResult);
        return fluentResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
    }

    public static bool IsValidModel<T>(this T model, IValidator<T> validator)
    {
        ArgumentNullException.ThrowIfNull(validator);
        var result = validator.Validate(model);
        return result.IsValid;
    }

    public static bool IsValidModel<T>(this T model, IValidator<T> validator, out IDictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(validator);
        var result = validator.Validate(model);
        errors = result.ToErrorDictionary();
        return result.IsValid;
    }

    public static IResult ToValidationProblem(this FluentValidation.Results.ValidationResult validationResult, string? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(validationResult);
        if (validationResult.IsValid)
        {
            return Results.Ok();
        }
        var errors = validationResult.ToErrorDictionary();
        return ResponseFactory.ValidationError(errors, correlationId);
    }

    public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance)
    {
        ArgumentNullException.ThrowIfNull(validator);
        var result = validator.Validate(instance);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }

    public static bool TryValidate<T>(this IValidator<T> validator, T instance, out FluentValidation.Results.ValidationResult result)
    {
        ArgumentNullException.ThrowIfNull(validator);
        result = validator.Validate(instance);
        return result.IsValid;
    }

    public static async Task<bool> TryValidateAsync<T>(this IValidator<T> validator, T instance)
    {
        ArgumentNullException.ThrowIfNull(validator);
        var result = await validator.ValidateAsync(instance);
        return result.IsValid;
    }
}