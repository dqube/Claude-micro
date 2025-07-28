using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using StoreService.Domain.Entities;
using StoreService.Domain.Exceptions;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class OpenRegisterCommandHandler : ICommandHandler<OpenRegisterCommand>
{
    private readonly IRepository<Register, RegisterId> _registerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OpenRegisterCommandHandler(
        IRepository<Register, RegisterId> registerRepository,
        IUnitOfWork unitOfWork)
    {
        _registerRepository = registerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(OpenRegisterCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var register = await _registerRepository.GetByIdAsync(request.RegisterId, cancellationToken);
        if (register is null)
            throw new RegisterNotFoundException(request.RegisterId);

        register.Open(request.StartingCash);

        _registerRepository.Update(register);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
} 