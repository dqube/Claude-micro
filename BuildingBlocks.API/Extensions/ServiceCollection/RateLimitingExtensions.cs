using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using BuildingBlocks.API.Configuration.Options;
using System.Threading.RateLimiting;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitOptions = configuration.GetSection(RateLimitingOptions.ConfigurationSection).Get<RateLimitingOptions>();
        
        if (rateLimitOptions == null || !rateLimitOptions.Enabled)
        {
            return services;
        }

        return services.AddApiRateLimiting(rateLimitOptions);
    }

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, RateLimitingOptions options)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = 429;
            
            // Add default global policy
            rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var clientId = GetClientIdentifier(httpContext);
                
                return options.Policy switch
                {
                    RateLimitingPolicy.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(clientId, _ => 
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = options.PermitLimit,
                            Window = options.Window,
                            QueueLimit = options.QueueLimit
                        }),
                    RateLimitingPolicy.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(clientId, _ => 
                        new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = options.PermitLimit,
                            Window = options.Window,
                            SegmentsPerWindow = 8,
                            QueueLimit = options.QueueLimit
                        }),
                    RateLimitingPolicy.TokenBucket => RateLimitPartition.GetTokenBucketLimiter(clientId, _ => 
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = options.PermitLimit,
                            TokensPerPeriod = options.PermitLimit / 2,
                            ReplenishmentPeriod = options.Window,
                            QueueLimit = options.QueueLimit
                        }),
                    RateLimitingPolicy.Concurrency => RateLimitPartition.GetConcurrencyLimiter(clientId, _ => 
                        new ConcurrencyLimiterOptions
                        {
                            PermitLimit = options.PermitLimit,
                            QueueLimit = options.QueueLimit
                        }),
                    _ => RateLimitPartition.GetFixedWindowLimiter(clientId, _ => 
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = options.PermitLimit,
                            Window = options.Window,
                            QueueLimit = options.QueueLimit
                        })
                };
            });

            // Add endpoint-specific policies
            foreach (var endpointPolicy in options.EndpointPolicies)
            {
                var policyName = $"Endpoint_{endpointPolicy.Key}";
                
                rateLimiterOptions.AddPolicy(policyName, httpContext =>
                {
                    var clientId = GetClientIdentifier(httpContext);
                    var endpointOptions = endpointPolicy.Value;
                    
                    return endpointOptions.Policy switch
                    {
                        RateLimitingPolicy.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(clientId, _ => 
                            new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = endpointOptions.PermitLimit,
                                Window = endpointOptions.Window
                            }),
                        RateLimitingPolicy.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(clientId, _ => 
                            new SlidingWindowRateLimiterOptions
                            {
                                PermitLimit = endpointOptions.PermitLimit,
                                Window = endpointOptions.Window,
                                SegmentsPerWindow = 8
                            }),
                        _ => RateLimitPartition.GetFixedWindowLimiter(clientId, _ => 
                            new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = endpointOptions.PermitLimit,
                                Window = endpointOptions.Window
                            })
                    };
                });
            }

            rateLimiterOptions.OnRejected = (context, _) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                context.HttpContext.Response.Headers.Add("Retry-After", options.Window.TotalSeconds.ToString());
                
                return new ValueTask();
            };
        });

        return services;
    }

    public static IServiceCollection AddFixedWindowRateLimiting(this IServiceCollection services, int permitLimit, TimeSpan window)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("FixedWindow", limiterOptions =>
            {
                limiterOptions.PermitLimit = permitLimit;
                limiterOptions.Window = window;
                limiterOptions.QueueLimit = 0;
            });
        });

        return services;
    }

    public static IServiceCollection AddSlidingWindowRateLimiting(this IServiceCollection services, int permitLimit, TimeSpan window)
    {
        services.AddRateLimiter(options =>
        {
            options.AddSlidingWindowLimiter("SlidingWindow", limiterOptions =>
            {
                limiterOptions.PermitLimit = permitLimit;
                limiterOptions.Window = window;
                limiterOptions.SegmentsPerWindow = 8;
                limiterOptions.QueueLimit = 0;
            });
        });

        return services;
    }

    private static string GetClientIdentifier(Microsoft.AspNetCore.Http.HttpContext context)
    {
        // Try to get authenticated user ID first
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        // Fall back to IP address
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        
        // Check for forwarded IP headers
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }
        else if (context.Request.Headers.ContainsKey("X-Real-IP"))
        {
            clientIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }

        return $"ip:{clientIp ?? "unknown"}";
    }
}