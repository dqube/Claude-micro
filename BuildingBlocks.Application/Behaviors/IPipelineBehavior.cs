namespace BuildingBlocks.Application.Behaviors;

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> handlerDelegate, CancellationToken cancellationToken = default);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();