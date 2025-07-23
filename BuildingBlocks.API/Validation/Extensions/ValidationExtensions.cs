using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BuildingBlocks.API.Responses.Base;
using BuildingBlocks.API.Responses.Builders;

namespace BuildingBlocks.API.Validation.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(ValidationExtensions));
        return services;
    }

    public static IServiceCollection AddFluentValidation(
        this IServiceCollection services,
        params Type[] assemblyMarkerTypes)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblyMarkerTypes);
        
        foreach (var markerType in assemblyMarkerTypes)
        {
            services.AddValidatorsFromAssemblyContaining(markerType);
        }
        return services;
    }

    public static async Task<IResult> ValidateAsync<T>(
        this T model,
        IValidator<T> validator,
        string? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(validator);
        
        var validationResult = await validator.ValidateAsync(model);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray());

            var response = ApiResponseBuilder.ValidationError(errors, correlationId: correlationId);
            return Microsoft.AspNetCore.Http.Results.BadRequest(response);
        }

        return Microsoft.AspNetCore.Http.Results.Ok();
    }

    public static async Task<(bool IsValid, IResult? ErrorResult)> TryValidateAsync<T>(
        this T model,
        IValidator<T> validator,
        string? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(validator);
        
        var validationResult = await validator.ValidateAsync(model);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray());

            var response = ApiResponseBuilder.ValidationError(errors, correlationId: correlationId);
            return (false, Microsoft.AspNetCore.Http.Results.BadRequest(response));
        }

        return (true, null);
    }

    public static IResult ValidationProblem(
        this IDictionary<string, string[]> errors,
        string? correlationId = null)
    {
        var response = ApiResponseBuilder.ValidationError(errors, correlationId: correlationId);
        return Microsoft.AspNetCore.Http.Results.BadRequest(response);
    }
}