namespace BuildingBlocks.Application.CQRS.Commands;

public abstract class CommandBase : ICommand
{
    protected CommandBase()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public DateTime Timestamp { get; }
}

public abstract class CommandBase<TResult> : CommandBase, ICommand<TResult>
{
}