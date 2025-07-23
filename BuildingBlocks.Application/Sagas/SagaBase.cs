using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Sagas;

public abstract class SagaBase<TData> : ISaga<TData> where TData : class
{
    private readonly List<SagaStep> _steps = [];
    private readonly ILogger _logger;

    protected SagaBase(TData data, ILogger logger)
    {
        Id = Guid.NewGuid();
        Data = data;
        _logger = logger;
        Status = SagaStatus.NotStarted;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public abstract string Name { get; }
    public SagaStatus Status { get; private set; }
    public TData Data { get; }
    public DateTime CreatedAt { get; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? LastUpdatedAt { get; private set; }
    public IReadOnlyList<SagaStep> Steps => _steps.AsReadOnly();

    protected void AddStep(string name, Func<CancellationToken, Task> action, Func<CancellationToken, Task>? compensation = null)
    {
        var step = new SagaStep(name, action, compensation);
        _steps.Add(step);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (Status != SagaStatus.NotStarted)
            throw new InvalidOperationException($"Saga {Name} is already started");

        Status = SagaStatus.Running;
        LastUpdatedAt = DateTime.UtcNow;
        
        _logger.LogInformation("Starting saga {SagaName} with ID {SagaId}", Name, Id);

        try
        {
            await ConfigureSteps();
            await ExecuteStepsAsync(cancellationToken);
            await CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga {SagaName} failed: {Error}", Name, ex.Message);
            await CompensateAsync(cancellationToken);
            throw;
        }
    }

    public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        Status = SagaStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        
        _logger.LogInformation("Saga {SagaName} completed successfully", Name);
        await OnCompletedAsync(cancellationToken);
    }

    public virtual async Task CompensateAsync(CancellationToken cancellationToken = default)
    {
        Status = SagaStatus.Compensating;
        LastUpdatedAt = DateTime.UtcNow;
        
        _logger.LogWarning("Starting compensation for saga {SagaName}", Name);

        // Compensate in reverse order
        var completedSteps = _steps.Where(s => s.Status == SagaStepStatus.Completed).Reverse();
        
        foreach (var step in completedSteps)
        {
            try
            {
                await step.CompensateAsync(cancellationToken);
                _logger.LogInformation("Compensated step {StepName} in saga {SagaName}", step.Name, Name);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to compensate step {StepName} in saga {SagaName}", step.Name, Name);
                // Continue with other compensations
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Failed to compensate step {StepName} in saga {SagaName}", step.Name, Name);
                // Continue with other compensations
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Failed to compensate step {StepName} in saga {SagaName}", step.Name, Name);
                // Continue with other compensations
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Failed to compensate step {StepName} in saga {SagaName}", step.Name, Name);
                // Continue with other compensations
            }
        }

        Status = SagaStatus.Compensated;
        LastUpdatedAt = DateTime.UtcNow;
        
        await OnCompensatedAsync(cancellationToken);
    }

    public virtual async Task FailAsync(string reason, CancellationToken cancellationToken = default)
    {
        Status = SagaStatus.Failed;
        LastUpdatedAt = DateTime.UtcNow;
        
        _logger.LogError("Saga {SagaName} failed: {Reason}", Name, reason);
        await OnFailedAsync(reason, cancellationToken);
    }

    protected abstract Task ConfigureSteps();
    protected virtual Task OnCompletedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task OnCompensatedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task OnFailedAsync(string reason, CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task ExecuteStepsAsync(CancellationToken cancellationToken)
    {
        foreach (var step in _steps)
        {
            _logger.LogInformation("Executing step {StepName} in saga {SagaName}", step.Name, Name);
            await step.ExecuteAsync(cancellationToken);
            _logger.LogInformation("Completed step {StepName} in saga {SagaName}", step.Name, Name);
        }
    }
}