using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        // Get all pipeline behaviors for this command type
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TCommand, object>>().ToList();
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        if (behaviors.Count > 0)
        {
            // Create a delegate that represents the final handler call
            RequestHandlerDelegate<object> handlerDelegate = async () =>
            {
                await handler.HandleAsync(command, cancellationToken);
                return null!; // Commands don't return values
            };

            // Execute behaviors in reverse order (LIFO - Last In, First Out)
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                var currentDelegate = handlerDelegate;
                var behavior = behaviors[i];
                handlerDelegate = () => behavior.HandleAsync(command, currentDelegate, cancellationToken);
            }

            await handlerDelegate();
        }
        else
        {
            await handler.HandleAsync(command, cancellationToken);
        }
    }

    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TResult>
    {
        // Get all pipeline behaviors for this command type
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TCommand, TResult>>().ToList();
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();

        if (behaviors.Count > 0)
        {
            // Create a delegate that represents the final handler call
            RequestHandlerDelegate<TResult> handlerDelegate = () => handler.HandleAsync(command, cancellationToken);

            // Execute behaviors in reverse order (LIFO - Last In, First Out)
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                var currentDelegate = handlerDelegate;
                var behavior = behaviors[i];
                handlerDelegate = () => behavior.HandleAsync(command, currentDelegate, cancellationToken);
            }

            return await handlerDelegate();
        }
        else
        {
            return await handler.HandleAsync(command, cancellationToken);
        }
    }
}