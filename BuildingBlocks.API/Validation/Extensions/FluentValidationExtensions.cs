using BuildingBlocks.API.Validation.Results;
using BuildingBlocks.API.Responses.Base;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BuildingBlocks.API.Validation.Extensions;

public static class FluentValidationExtensions
{
    public static ValidationResult ToValidationResult(this FluentValidation.Results.ValidationResult fluentResult)
    {
        if (fluentResult.IsValid)
            return ValidationResult.Success();

        var errors = fluentResult.Errors
            .Select(failure => new ValidationError(
                failure.PropertyName, 
                failure.ErrorMessage)
            {
                AttemptedValue = failure.AttemptedValue?.ToString(),
                ErrorCode = failure.ErrorCode
            })
            .ToArray();

        return ValidationResult.Failure(errors);
    }

    public static ValidationErrorResponse ToValidationErrorResponse(this FluentValidation.Results.ValidationResult fluentResult, string? correlationId = null)
    {
        var validationErrors = fluentResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        return new ValidationErrorResponse
        {
            Success = false,
            Message = "Validation failed",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            Errors = validationErrors
        };
    }

    public static async Task<FluentValidation.Results.ValidationResult> ValidateAsync<T>(
        this IValidator<T> validator,
        HttpRequest request,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (!request.HasJsonContentType())
            {
                var result = new FluentValidation.Results.ValidationResult();
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("ContentType", "Request must have JSON content type"));
                return result;
            }

            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(json))
            {
                var result = new FluentValidation.Results.ValidationResult();
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("Body", "Request body cannot be empty"));
                return result;
            }

            var obj = JsonSerializer.Deserialize<T>(json);
            if (obj == null)
            {
                var result = new FluentValidation.Results.ValidationResult();
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("Body", "Invalid JSON format"));
                return result;
            }

            return await validator.ValidateAsync(obj, cancellationToken);
        }
        catch (JsonException ex)
        {
            var result = new FluentValidation.Results.ValidationResult();
            result.Errors.Add(new FluentValidation.Results.ValidationFailure("Body", $"Invalid JSON: {ex.Message}"));
            return result;
        }
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining<FluentValidationExtensions>();
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services, params Type[] markerTypes)
    {
        foreach (var markerType in markerTypes)
        {
            services.AddValidatorsFromAssemblyContaining(markerType);
        }
        return services;
    }
}