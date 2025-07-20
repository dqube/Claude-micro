namespace BuildingBlocks.Application.Caching;

public interface ICacheKey
{
    string Key { get; }
    TimeSpan? Expiration { get; }
}