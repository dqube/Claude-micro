namespace BuildingBlocks.Infrastructure.BackgroundServices;

public interface IBackgroundTaskService
{
    Task<string> EnqueueAsync<T>(T task, CancellationToken cancellationToken = default) where T : class;
    Task<string> ScheduleAsync<T>(T task, TimeSpan delay, CancellationToken cancellationToken = default) where T : class;
    Task<string> ScheduleAsync<T>(T task, DateTime scheduledTime, CancellationToken cancellationToken = default) where T : class;
    Task<bool> CancelAsync(string taskId, CancellationToken cancellationToken = default);
    Task<TaskStatus> GetStatusAsync(string taskId, CancellationToken cancellationToken = default);
}

public enum TaskStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}