using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using EmployeeService.Domain.Exceptions;
using EmployeeService.Domain.Repositories;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Application.Commands;

public sealed class UpdateEmployeePositionCommandHandler : ICommandHandler<UpdateEmployeePositionCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeePositionCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdateEmployeePositionCommand command, CancellationToken cancellationToken = default)
    {
        var employeeId = EmployeeId.From(command.EmployeeId);
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken)
            ?? throw new EmployeeNotFoundException(employeeId);

        employee.UpdatePosition(new Position(command.Position));

        // No need to call UpdateAsync - EF tracks changes automatically
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}