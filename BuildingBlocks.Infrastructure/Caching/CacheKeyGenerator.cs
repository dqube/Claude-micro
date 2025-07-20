using System.Text;
using BuildingBlocks.Application.Caching;

namespace BuildingBlocks.Infrastructure.Caching;

public class CacheKeyGenerator
{
    private const string Separator = ":";

    public static string Generate(params string[] parts)
    {
        return string.Join(Separator, parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    public static string Generate<T>(params object[] parts)
    {
        var typeName = typeof(T).Name;
        var stringParts = new[] { typeName }.Concat(parts.Select(p => p?.ToString() ?? string.Empty));
        return Generate(stringParts.ToArray());
    }

    public static string GenerateFromCacheKey(ICacheKey cacheKey)
    {
        return cacheKey.Key;
    }

    public static string GenerateForEntity<TEntity>(object id)
    {
        return Generate<TEntity>("entity", id.ToString()!);
    }

    public static string GenerateForList<TEntity>(params object[] parameters)
    {
        var parts = new[] { typeof(TEntity).Name, "list" }.Concat(parameters.Select(p => p?.ToString() ?? string.Empty));
        return Generate(parts.ToArray());
    }

    public static string GenerateForQuery<TQuery>(TQuery query) where TQuery : class
    {
        var queryName = typeof(TQuery).Name;
        var properties = typeof(TQuery).GetProperties();
        
        var sb = new StringBuilder();
        sb.Append(queryName);
        
        foreach (var property in properties)
        {
            var value = property.GetValue(query);
            if (value != null)
            {
                sb.Append(Separator);
                sb.Append(property.Name);
                sb.Append(Separator);
                sb.Append(value.ToString());
            }
        }
        
        return sb.ToString();
    }

    public static string GenerateForUser(string userId, params string[] additionalParts)
    {
        var parts = new[] { "user", userId }.Concat(additionalParts);
        return Generate(parts.ToArray());
    }

    public static string GenerateWithPrefix(string prefix, params string[] parts)
    {
        var allParts = new[] { prefix }.Concat(parts);
        return Generate(allParts.ToArray());
    }
}