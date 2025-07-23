using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Inbox;

namespace BuildingBlocks.Application.Services;

public partial class InboxBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxBackgroundService> _logger;
    private readonly InboxOptions _options;

    [LoggerMessage(LogLevel.Information, "Inbox background service started")]
    private static partial void LogServiceStarted(ILogger logger);

    [LoggerMessage(LogLevel.Error, "Error occurred while processing inbox messages")]
    private static partial void LogProcessingError(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Information, "Inbox background service stopped")]
    private static partial void LogServiceStopped(ILogger logger);

    public InboxBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<InboxBackgroundService> logger,
        InboxOptions options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogServiceStarted(_logger);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IInboxProcessor>();

                await processor.ProcessPendingMessagesAsync(stoppingToken);
                await processor.RetryFailedMessagesAsync(stoppingToken);
                await processor.CleanupExpiredMessagesAsync(stoppingToken);
            }
            catch (InvalidOperationException ex)
            {
                LogProcessingError(_logger, ex);
            }
            catch (TaskCanceledException ex) when (!stoppingToken.IsCancellationRequested)
            {
                LogProcessingError(_logger, ex);
            }
            catch (TimeoutException ex)
            {
                LogProcessingError(_logger, ex);
            }
            catch (ArgumentException ex)
            {
                LogProcessingError(_logger, ex);
            }

            await Task.Delay(_options.ProcessingInterval, stoppingToken);
        }

        LogServiceStopped(_logger);
    }
}