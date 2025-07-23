using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Repositories;
using BuildingBlocks.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthService.Domain.ValueObjects;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRepository<Domain.Entities.User, UserId>, UserRepository>();
        services.AddScoped<IReadOnlyRepository<Domain.Entities.User, UserId>, UserRepository>();

        return services;
    }
}