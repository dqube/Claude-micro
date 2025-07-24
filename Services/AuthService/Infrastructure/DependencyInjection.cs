using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Repositories;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Database Context
        services.AddDbContext<AuthDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // For development, use InMemory database for simplicity
                options.UseInMemoryDatabase("AuthServiceDb");
            }
        });

        // Register User Repository
        services.AddScoped<IRepository<User, UserId>, UserRepository>();
        services.AddScoped<IReadOnlyRepository<User, UserId>, UserRepository>();

        // Register Role Repository
        services.AddScoped<IRepository<Role, RoleId>, RoleRepository>();
        services.AddScoped<IReadOnlyRepository<Role, RoleId>, RoleRepository>();

        // Register RegistrationToken Repository
        services.AddScoped<IRepository<RegistrationToken, TokenId>, RegistrationTokenRepository>();
        services.AddScoped<IReadOnlyRepository<RegistrationToken, TokenId>, RegistrationTokenRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}