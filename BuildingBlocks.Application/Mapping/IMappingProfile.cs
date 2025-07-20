using System.Linq.Expressions;

namespace BuildingBlocks.Application.Mapping;

public interface IMappingProfile
{
    void Configure(IMapperConfiguration configuration);
    int Order { get; }
}

public interface IMapperConfiguration
{
    void CreateMap<TSource, TDestination>();
    void CreateMap<TSource, TDestination>(Action<IMappingExpression<TSource, TDestination>> configurationExpression);
    void CreateMap(Type sourceType, Type destinationType);
}

public interface IMappingExpression<TSource, TDestination>
{
    IMappingExpression<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions);

    IMappingExpression<TSource, TDestination> IgnoreMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember);

    IMappingExpression<TSource, TDestination> ReverseMap();
}

public interface IMemberConfigurationExpression<TSource, TDestination, TMember>
{
    void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember);
    void MapFrom(Func<TSource, TMember> mapFunction);
    void MapFrom(Func<TSource, TDestination, TMember> mapFunction);
    void Ignore();
    void UseValue(TMember value);
    void Condition(Func<TSource, bool> condition);
}