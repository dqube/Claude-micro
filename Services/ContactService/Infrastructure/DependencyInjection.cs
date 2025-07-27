using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContactService.Domain.Entities;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;
using ContactService.Infrastructure.Persistence;
using ContactService.Infrastructure.Repositories;

namespace ContactService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ContactDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase("ContactServiceDb");
            }
        });

        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IRepository<Contact, ContactId>, ContactRepository>();
        services.AddScoped<IReadOnlyRepository<Contact, ContactId>, ContactRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}