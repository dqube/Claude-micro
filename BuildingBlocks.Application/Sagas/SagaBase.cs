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

        LogStartingSaga(_logger, Name, Id, null);

        try
        {
            await ConfigureSteps();
            await ExecuteStepsAsync(cancellationToken);
            await CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            LogSagaFailed(_logger, Name, ex.Message, ex);
            await CompensateAsync(cancellationToken);
            throw;
        }
    }

    public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        Status = SagaStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;

        LogSagaCompleted(_logger, Name, null);
        await OnCompletedAsync(cancellationToken);
    }

    public virtual async Task CompensateAsync(CancellationToken cancellationToken = default)
    {
        Status = SagaStatus.Compensating;
        LastUpdatedAt = DateTime.UtcNow;

        LogSagaCompensating(_logger, Name, null);

        // Compensate in reverse order
        var completedSteps = _steps.Where(s => s.Status == SagaStepStatus.Completed).Reverse();

        foreach (var step in completedSteps)
        {
            try
            {
                await step.CompensateAsync(cancellationToken);
                LogStepCompensated(_logger, step.Name, Name, null);
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is TaskCanceledException || ex is TimeoutException || ex is ArgumentException)
            {
                LogStepCompensationFailed(_logger, step.Name, Name, ex);
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

        LogSagaFailed(_logger, Name, reason, null);
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
            LogStepExecuting(_logger, step.Name, Name, null);
            await step.ExecuteAsync(cancellationToken);
            LogStepExecuted(_logger, step.Name, Name, null);
        }
    }
    // LoggerMessage delegates for performance and code analysis compliance
    private static readonly Action<ILogger, string, Guid, Exception?> LogStartingSaga =
        LoggerMessage.Define<string, Guid>(LogLevel.Information, new EventId(1000, "SagaStarting"), "Starting saga {SagaName} with ID {SagaId}");

    private static readonly Action<ILogger, string, string, Exception?> LogSagaFailed =
        LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(1001, "SagaFailed"), "Saga {SagaName} failed: {Error}");

    private static readonly Action<ILogger, string, Exception?> LogSagaCompleted =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1002, "SagaCompleted"), "Saga {SagaName} completed successfully");

    private static readonly Action<ILogger, string, Exception?> LogSagaCompensating =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1003, "SagaCompensating"), "Starting compensation for saga {SagaName}");

    private static readonly Action<ILogger, string, string, Exception?> LogStepCompensationFailed =
        LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(1004, "StepCompensationFailed"), "Failed to compensate step {StepName} in saga {SagaName}");

    private static readonly Action<ILogger, string, string, Exception?> LogStepCompensated =
        LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(1005, "StepCompensated"), "Compensated step {StepName} in saga {SagaName}");

    private static readonly Action<ILogger, string, string, Exception?> LogStepExecuting =
        LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(1006, "StepExecuting"), "Executing step {StepName} in saga {SagaName}");

    private static readonly Action<ILogger, string, string, Exception?> LogStepExecuted =
        LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(1007, "StepExecuted"), "Completed step {StepName} in saga {SagaName}");
}