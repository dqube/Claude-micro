using BuildingBlocks.Application.Validation;

namespace BuildingBlocks.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
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
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(request, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count > 0)
            {
                throw new ValidationException(failures);
            }
        }

        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);
        return await next();
    }
}

public class ValidationContext<T>
{
    public ValidationContext(T instanceToValidate)
    {
        InstanceToValidate = instanceToValidate;
    }

    public T InstanceToValidate { get; }
}

public class ValidationException : Exception
{
    public ValidationException()
        : base("Validation failed.")
    {
        Errors = new List<ValidationError>();
    }

    public ValidationException(string message)
        : base(message)
    {
        Errors = new List<ValidationError>();
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = new List<ValidationError>();
    }

    public ValidationException(IEnumerable<ValidationError> failures)
        : base($"Validation failed: {string.Join(", ", failures.Select(f => f.ErrorMessage))}")
    {
        Errors = failures.ToList();
    }

    public IReadOnlyList<ValidationError> Errors { get; }
}