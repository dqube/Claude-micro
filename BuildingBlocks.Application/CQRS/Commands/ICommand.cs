namespace BuildingBlocks.Application.CQRS.Commands;

public interface ICommand
{
    Guid Id { get; }
    DateTime Timestamp { get; }
}

public interface ICommand<out TResult> : ICommand
{
}