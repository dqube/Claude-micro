namespace BuildingBlocks.Application.Sagas;

public class SagaStep
{
    public SagaStep(string name, Func<CancellationToken, Task> action, Func<CancellationToken, Task>? compensation = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Action = action;
        CompensationAction = compensation;
        Status = SagaStepStatus.NotStarted;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public string Name { get; }
    public SagaStepStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CompensatedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Func<CancellationToken, Task> Action { get; }
    public Func<CancellationToken, Task>? CompensationAction { get; }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Status = SagaStepStatus.Running;
        StartedAt = DateTime.UtcNow;
        
        try
        {
            await Action(cancellationToken);
            Status = SagaStepStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            Status = SagaStepStatus.Failed;
            ErrorMessage = ex.Message;
            throw;
        }
    }

    public async Task CompensateAsync(CancellationToken cancellationToken = default)
    {
        if (CompensationAction == null)
        {
            Status = SagaStepStatus.Compensated;
            CompensatedAt = DateTime.UtcNow;
            return;
        }

        Status = SagaStepStatus.Compensating;
        
        try
        {
            await CompensationAction(cancellationToken);
            Status = SagaStepStatus.Compensated;
            CompensatedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            Status = SagaStepStatus.CompensationFailed;
            ErrorMessage = ex.Message;
            throw;
        }
    }
}

public enum SagaStepStatus
{
    NotStarted,
    Running,
    Completed,
    Failed,
    Compensating,
    Compensated,
    CompensationFailed
}