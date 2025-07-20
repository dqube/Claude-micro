namespace BuildingBlocks.Application.Mapping;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    void MapTo<TSource, TDestination>(TSource source, TDestination destination);
    IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source);
    IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source);
    IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> source);
}