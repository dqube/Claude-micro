using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Outbox;

namespace BuildingBlocks.Application.Services;

public class OutboxBackgroundService : BackgroundService
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
        _logger.LogInformation("Outbox background service started");

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
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }
            catch (TaskCanceledException ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }

            await Task.Delay(_options.ProcessingInterval, stoppingToken);
        }

        _logger.LogInformation("Outbox background service stopped");
    }
}