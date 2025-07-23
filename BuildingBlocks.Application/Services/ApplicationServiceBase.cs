using BuildingBlocks.Domain.Repository;

namespace BuildingBlocks.Application.Services;

public abstract class ApplicationServiceBase : IApplicationService
{
    protected readonly IUnitOfWork UnitOfWork;

    protected ApplicationServiceBase(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        UnitOfWork = unitOfWork;
    }

    protected async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        try
        {
            await UnitOfWork.BeginTransactionAsync();
            var result = await operation();
            await UnitOfWork.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    protected async Task ExecuteAsync(Func<Task> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        try
        {
            await UnitOfWork.BeginTransactionAsync();
            await operation();
            await UnitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}