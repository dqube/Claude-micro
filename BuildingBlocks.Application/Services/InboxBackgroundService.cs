using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Inbox;

namespace BuildingBlocks.Application.Services;

public class InboxBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxBackgroundService> _logger;
    private readonly InboxOptions _options;

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
        _logger.LogInformation("Inbox background service started");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing inbox messages");
            }

            await Task.Delay(_options.ProcessingInterval, stoppingToken);
        }

        _logger.LogInformation("Inbox background service stopped");
    }
}