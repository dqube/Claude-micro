using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Exceptions;
using EmployeeService.Domain.Repositories;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Application.Commands;

public sealed class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand, Guid>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreateEmployeeCommand command, CancellationToken cancellationToken = default)
    {
        var employeeNumber = new EmployeeNumber(command.EmployeeNumber);
        
        if (await _employeeRepository.ExistsByEmployeeNumberAsync(employeeNumber, cancellationToken))
            throw new EmployeeAlreadyExistsException(employeeNumber);

        if (await _employeeRepository.ExistsByUserIdAsync(command.UserId, cancellationToken))
            throw new EmployeeAlreadyExistsException(command.UserId);

        var employee = new Employee(
            EmployeeId.New(),
            command.UserId,
            command.StoreId,
            employeeNumber,
            command.HireDate,
            new Position(command.Position),
            command.AuthLevel);

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return employee.Id.Value;
    }
}