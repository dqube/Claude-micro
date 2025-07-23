using BuildingBlocks.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);
        var requestName = typeof(TRequest).Name;

        LogTransactionStart(_logger, requestName, null);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            LogTransactionCommit(_logger, requestName, null);

            return response;
        }
        catch (Exception ex)
        {
            LogTransactionError(_logger, requestName, ex);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static readonly Action<ILogger, string, Exception?> LogTransactionStart =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, "TransactionStart"), "Starting transaction for {RequestName}");

    private static readonly Action<ILogger, string, Exception?> LogTransactionCommit =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(2, "TransactionCommit"), "Transaction committed successfully for {RequestName}");

    private static readonly Action<ILogger, string, Exception?> LogTransactionError =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(3, "TransactionError"), "Transaction failed for {RequestName}, rolling back");
}