using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Sagas;

public static class SagaExtensions
{
    public static IServiceCollection AddSagas(this IServiceCollection services)
    {
        // Register saga orchestrator
        services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();
        
        // Register saga repositories (implementations would be in Infrastructure layer)
        // services.AddScoped(typeof(ISagaRepository<,>), typeof(SagaRepository<,>));
        
        return services;
    }

    public static IServiceCollection AddSaga<TSaga, TData>(this IServiceCollection services)
        where TSaga : class, ISaga<TData>
        where TData : class
    {
        services.AddTransient<TSaga>();
        services.AddScoped<ISagaRepository<TSaga, TData>>();
        return services;
    }
}

// Basic orchestrator implementation
public class SagaOrchestrator : ISagaOrchestrator
{
    private readonly IServiceProvider _serviceProvider;

    public SagaOrchestrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TSaga> StartSagaAsync<TSaga, TData>(TData data, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga<TData>
        where TData : class
    {
        var saga = _serviceProvider.GetRequiredService<TSaga>();
        var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga, TData>>();
        
        await repository.SaveAsync(saga, cancellationToken);
        await saga.StartAsync(cancellationToken);
        await repository.SaveAsync(saga, cancellationToken);
        
        return saga;
    }

    public async Task<TSaga?> GetSagaAsync<TSaga, TData>(Guid sagaId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga<TData>
        where TData : class
    {
        var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga, TData>>();
        return await repository.GetAsync(sagaId, cancellationToken);
    }

    public async Task CompensateSagaAsync<TSaga, TData>(Guid sagaId, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga<TData>
        where TData : class
    {
        var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga, TData>>();
        var saga = await repository.GetAsync(sagaId, cancellationToken);
        
        if (saga != null)
        {
            await saga.CompensateAsync(cancellationToken);
            await repository.SaveAsync(saga, cancellationToken);
        }
    }

    public async Task<IEnumerable<TSaga>> GetSagasByStatusAsync<TSaga, TData>(SagaStatus status, CancellationToken cancellationToken = default)
        where TSaga : class, ISaga<TData>
        where TData : class
    {
        var repository = _serviceProvider.GetRequiredService<ISagaRepository<TSaga, TData>>();
        return await repository.GetByStatusAsync(status, cancellationToken);
    }
}