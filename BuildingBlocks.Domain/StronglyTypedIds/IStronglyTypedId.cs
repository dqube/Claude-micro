namespace BuildingBlocks.Domain.StronglyTypedIds;

/// <summary>
/// Base marker interface for strongly typed IDs (used for type checking)
/// </summary>
public interface IStronglyTypedId
{
}

/// <summary>
/// Generic interface for strongly typed IDs with type-safe value access
/// </summary>
public interface IStronglyTypedId<out T> : IStronglyTypedId
{
    T Value { get; }
}
