using System.Collections.Concurrent;
using System.Reflection;
using System.Linq.Expressions;

namespace BuildingBlocks.Application.Mapping;

public abstract class MapperBase : IMapper
{
    private readonly ConcurrentDictionary<(Type source, Type destination), Func<object, object>> _mappingCache;
    private readonly Dictionary<Type, IMappingProfile> _profiles;

    protected MapperBase()
    {
        _mappingCache = new ConcurrentDictionary<(Type, Type), Func<object, object>>();
        _profiles = new Dictionary<Type, IMappingProfile>();
        LoadProfiles();
    }

    public virtual TDestination Map<TDestination>(object source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return Map<object, TDestination>(source);
    }

    public virtual TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var sourceType = typeof(TSource);
        var destinationType = typeof(TDestination);

        var mappingFunc = _mappingCache.GetOrAdd((sourceType, destinationType), key =>
            CreateMappingFunction(key.source, key.destination));

        return (TDestination)mappingFunc(source);
    }

    public virtual TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        MapInternal(source, destination);
        return destination;
    }

    public virtual void MapTo<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        MapInternal(source, destination);
    }

    public virtual IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Select(Map<TSource, TDestination>);
    }

    public virtual IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
    {
        throw new NotImplementedException("ProjectTo requires a specific implementation for the underlying ORM/query provider");
    }

    public virtual IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> source)
    {
        throw new NotImplementedException("ProjectTo requires a specific implementation for the underlying ORM/query provider");
    }

    protected abstract Func<object, object> CreateMappingFunction(Type sourceType, Type destinationType);
    protected abstract void MapInternal<TSource, TDestination>(TSource source, TDestination destination);

    private void LoadProfiles()
    {
        var profileTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IMappingProfile).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var profileType in profileTypes)
        {
            if (Activator.CreateInstance(profileType) is IMappingProfile profile)
            {
                _profiles[profileType] = profile;
            }
        }
    }
}