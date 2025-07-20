namespace BuildingBlocks.Infrastructure.Data.UnitOfWork;

public interface IDbTransaction : IDisposable, IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    void Commit();
    void Rollback();
}