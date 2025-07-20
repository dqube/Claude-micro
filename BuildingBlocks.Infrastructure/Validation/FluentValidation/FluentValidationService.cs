using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Validation;

namespace BuildingBlocks.Infrastructure.Validation.FluentValidation;

public class FluentValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public FluentValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<BuildingBlocks.Application.Validation.ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetService(typeof(global::FluentValidation.IValidator<T>)) as global::FluentValidation.IValidator<T>;
        
        if (validator == null)
        {
            return BuildingBlocks.Application.Validation.ValidationResult.Success();
        }

        var fluentResult = await validator.ValidateAsync(instance, cancellationToken);
        
        if (fluentResult.IsValid)
        {
            return BuildingBlocks.Application.Validation.ValidationResult.Success();
        }

        var errors = fluentResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        return BuildingBlocks.Application.Validation.ValidationResult.Failure(errors.ToArray());
    }

    public BuildingBlocks.Application.Validation.ValidationResult Validate<T>(T instance)
    {
        return ValidateAsync(instance).GetAwaiter().GetResult();
    }
}