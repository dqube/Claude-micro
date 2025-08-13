using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using BuildingBlocks.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Repositories;
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
        
        // Register AuthDbContext as IDbContext for BuildingBlocks services
        services.AddScoped<BuildingBlocks.Infrastructure.Data.Context.IDbContext, AuthDbContext>();

        // Register User Repository
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRepository<User, UserId>, UserRepository>();
        services.AddScoped<IReadOnlyRepository<User, UserId>, UserRepository>();

        // Register Role Repository
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRepository<Role, RoleId>, RoleRepository>();
        services.AddScoped<IReadOnlyRepository<Role, RoleId>, RoleRepository>();

        // Register RegistrationToken Repository
        services.AddScoped<IRegistrationTokenRepository, RegistrationTokenRepository>();
        services.AddScoped<IRepository<RegistrationToken, TokenId>, RegistrationTokenRepository>();
        services.AddScoped<IReadOnlyRepository<RegistrationToken, TokenId>, RegistrationTokenRepository>();

        // Register UserRole Repository
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRepository<UserRole, UserId>, UserRoleRepository>();
        services.AddScoped<IReadOnlyRepository<UserRole, UserId>, UserRoleRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}