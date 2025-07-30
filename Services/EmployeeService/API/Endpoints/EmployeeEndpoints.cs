using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Application.CQRS.Queries;
using EmployeeService.Application.Commands;
using EmployeeService.Application.Queries;

namespace EmployeeService.API.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/employees")
            .WithTags("Employees");

        group.MapPost("", CreateEmployeeAsync)
            .WithName("CreateEmployee")
            .WithSummary("Create a new employee");

        group.MapGet("{id:guid}", GetEmployeeByIdAsync)
            .WithName("GetEmployeeById")
            .WithSummary("Get employee by ID");

        group.MapGet("store/{storeId:int}", GetEmployeesByStoreAsync)
            .WithName("GetEmployeesByStore")
            .WithSummary("Get employees by store ID");

        group.MapPut("{id:guid}/position", UpdateEmployeePositionAsync)
            .WithName("UpdateEmployeePosition")
            .WithSummary("Update employee position");

        group.MapPut("{id:guid}/terminate", TerminateEmployeeAsync)
            .WithName("TerminateEmployee")
            .WithSummary("Terminate employee");
    }

    private static async Task<IResult> CreateEmployeeAsync(
        CreateEmployeeRequest request,
        ICommandHandler<CreateEmployeeCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateEmployeeCommand(
            request.UserId,
            request.StoreId,
            request.EmployeeNumber,
            request.HireDate,
            request.Position,
            request.AuthLevel);

        var employeeId = await handler.HandleAsync(command, cancellationToken);

        return Results.Created($"/api/employees/{employeeId}", new { EmployeeId = employeeId });
    }

    private static async Task<IResult> GetEmployeeByIdAsync(
        Guid id,
        IQueryHandler<GetEmployeeByIdQuery, EmployeeService.Application.DTOs.EmployeeDto> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeeByIdQuery(id);
        var employee = await handler.HandleAsync(query, cancellationToken);
        return Results.Ok(employee);
    }

    private static async Task<IResult> GetEmployeesByStoreAsync(
        int storeId,
        IQueryHandler<GetEmployeesByStoreQuery, IEnumerable<EmployeeService.Application.DTOs.EmployeeDto>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeesByStoreQuery(storeId);
        var employees = await handler.HandleAsync(query, cancellationToken);
        return Results.Ok(employees);
    }

    private static async Task<IResult> UpdateEmployeePositionAsync(
        Guid id,
        UpdateEmployeePositionRequest request,
        ICommandHandler<UpdateEmployeePositionCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEmployeePositionCommand(id, request.Position);
        await handler.HandleAsync(command, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> TerminateEmployeeAsync(
        Guid id,
        TerminateEmployeeRequest request,
        ICommandHandler<TerminateEmployeeCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new TerminateEmployeeCommand(id, request.TerminationDate);
        await handler.HandleAsync(command, cancellationToken);
        return Results.NoContent();
    }
}

public record CreateEmployeeRequest(
    Guid UserId,
    int StoreId,
    string EmployeeNumber,
    DateTime HireDate,
    string Position,
    int AuthLevel = 1);

public record UpdateEmployeePositionRequest(string Position);

public record TerminateEmployeeRequest(DateTime TerminationDate);