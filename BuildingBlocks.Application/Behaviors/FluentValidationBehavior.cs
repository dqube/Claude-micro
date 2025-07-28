using FluentValidation;
using FluentValidation.Results;

namespace BuildingBlocks.Application.Behaviors;

public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        if (_validators.Any())
        {
            // Run all validators
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(request, cancellationToken)));

            // Collect all validation failures
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count > 0)
            {
                throw new FluentValidationException(failures);
            }
        }

        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);
        return await next();
    }
}

/// <summary>
/// Custom exception that wraps FluentValidation errors for the middleware
/// </summary>
public class FluentValidationException : Exception
{
    public FluentValidationException()
        : base("Validation failed.")
    {
        Errors = new List<ValidationFailure>();
    }

    public FluentValidationException(string message)
        : base(message)
    {
        Errors = new List<ValidationFailure>();
    }

    public FluentValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = new List<ValidationFailure>();
    }

    public FluentValidationException(IEnumerable<ValidationFailure> failures)
        : base($"Validation failed: {string.Join(", ", failures.Select(f => f.ErrorMessage))}")
    {
        Errors = failures.ToList();
    }

    public IReadOnlyList<ValidationFailure> Errors { get; }
}