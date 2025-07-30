using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Inbox;
using BuildingBlocks.Application.Outbox;
using BuildingBlocksInboxService = BuildingBlocks.Infrastructure.Services.InboxService;
using BuildingBlocksOutboxService = BuildingBlocks.Infrastructure.Services.OutboxService;

namespace BuildingBlocks.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering Inbox/Outbox services
/// </summary>
public static class InboxOutboxExtensions
{
    /// <summary>
    /// Registers the default BuildingBlocks Inbox and Outbox services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInboxOutboxServices(this IServiceCollection services)
    {
        services.AddScoped<IInboxService, BuildingBlocksInboxService>();
        services.AddScoped<IOutboxService, BuildingBlocksOutboxService>();
        
        return services;
    }
    
    /// <summary>
    /// Registers the default BuildingBlocks Inbox service only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInboxService(this IServiceCollection services)
    {
        services.AddScoped<IInboxService, BuildingBlocksInboxService>();
        
        return services;
    }
    
    /// <summary>
    /// Registers the default BuildingBlocks Outbox service only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOutboxService(this IServiceCollection services)
    {
        services.AddScoped<IOutboxService, BuildingBlocksOutboxService>();
        
        return services;
    }
    
    /// <summary>
    /// Registers Inbox/Outbox services based on configuration settings
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInboxOutboxServices(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("InboxOutbox").Get<InboxOutboxOptions>() ?? new InboxOutboxOptions();
        
        if (options.Enabled)
        {
            if (options.IncludeInboxService)
            {
                services.AddScoped<IInboxService, BuildingBlocksInboxService>();
            }
            
            if (options.IncludeOutboxService)
            {
                services.AddScoped<IOutboxService, BuildingBlocksOutboxService>();
            }
        }
        
        return services;
    }
}

/// <summary>
/// Configuration options for Inbox/Outbox services
/// </summary>
public class InboxOutboxOptions
{
    public bool Enabled { get; set; } = true;
    public bool IncludeInboxService { get; set; } = true;
    public bool IncludeOutboxService { get; set; } = true;
}