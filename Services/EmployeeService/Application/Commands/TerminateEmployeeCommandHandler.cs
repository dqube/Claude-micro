using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using EmployeeService.Domain.Exceptions;
using EmployeeService.Domain.Repositories;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Application.Commands;

public sealed class TerminateEmployeeCommandHandler : ICommandHandler<TerminateEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TerminateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(TerminateEmployeeCommand command, CancellationToken cancellationToken = default)
    {
        var employeeId = EmployeeId.From(command.EmployeeId);
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken)
            ?? throw new EmployeeNotFoundException(employeeId);

        employee.Terminate(command.TerminationDate);

        // No need to call UpdateAsync - EF tracks changes automatically
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}