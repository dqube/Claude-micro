using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Infrastructure.Extensions;
using EmployeeService.Domain.Repositories;
using EmployeeService.Infrastructure.Persistence;
using EmployeeService.Infrastructure.Repositories;
using EmployeeService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EmployeeDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Override with EmployeeService-specific OutboxService if needed
        // (Inbox/Outbox services are automatically registered by AddBuildingBlocksApi)
        services.AddScoped<BuildingBlocks.Application.Outbox.IOutboxService, OutboxService>();

        return services;
    }
}