using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Events;
using BuildingBlocks.Application.CQRS.Queries;
using EmployeeService.Application.Commands;
using EmployeeService.Application.DTOs;
using EmployeeService.Application.EventHandlers;
using EmployeeService.Application.Queries;
using EmployeeService.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateEmployeeCommand, Guid>, CreateEmployeeCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateEmployeePositionCommand>, UpdateEmployeePositionCommandHandler>();
        services.AddScoped<ICommandHandler<TerminateEmployeeCommand>, TerminateEmployeeCommandHandler>();

        services.AddScoped<IQueryHandler<GetEmployeeByIdQuery, EmployeeDto>, GetEmployeeByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetEmployeesByStoreQuery, IEnumerable<EmployeeDto>>, GetEmployeesByStoreQueryHandler>();

        services.AddScoped<IEventHandler<DomainEventWrapper<EmployeeCreatedEvent>>, EmployeeCreatedEventHandler>();
        services.AddScoped<IEventHandler<DomainEventWrapper<EmployeeTerminatedEvent>>, EmployeeTerminatedEventHandler>();

        return services;
    }
}