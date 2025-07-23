using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Outbox;

namespace BuildingBlocks.Application.Services;

public partial class OutboxBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxBackgroundService> _logger;
    private readonly OutboxOptions _options;

    public OutboxBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OutboxBackgroundService> logger,
        OutboxOptions options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogOutboxServiceStarted(_logger);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();

                await processor.ProcessPendingMessagesAsync(stoppingToken);
                await processor.RetryFailedMessagesAsync(stoppingToken);
                await processor.CleanupExpiredMessagesAsync(stoppingToken);
            }
            catch (InvalidOperationException ex)
            {
                LogOutboxProcessingError(_logger, ex);
            }
            catch (TaskCanceledException ex) when (!stoppingToken.IsCancellationRequested)
            {
                LogOutboxProcessingError(_logger, ex);
            }
            catch (TimeoutException ex)
            {
                LogOutboxProcessingError(_logger, ex);
            }
            catch (ArgumentException ex)
            {
                LogOutboxProcessingError(_logger, ex);
            }

            await Task.Delay(_options.ProcessingInterval, stoppingToken);
        }

        LogOutboxServiceStopped(_logger);
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Outbox background service started")]
    private static partial void LogOutboxServiceStarted(ILogger logger);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Error occurred while processing outbox messages")]
    private static partial void LogOutboxProcessingError(ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "Outbox background service stopped")]
    private static partial void LogOutboxServiceStopped(ILogger logger);
}