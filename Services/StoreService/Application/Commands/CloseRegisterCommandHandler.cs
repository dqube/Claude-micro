using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using StoreService.Domain.Entities;
using StoreService.Domain.Exceptions;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class CloseRegisterCommandHandler : ICommandHandler<CloseRegisterCommand>
{
    private readonly IRepository<Register, RegisterId> _registerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseRegisterCommandHandler(
        IRepository<Register, RegisterId> registerRepository,
        IUnitOfWork unitOfWork)
    {
        _registerRepository = registerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(CloseRegisterCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var register = await _registerRepository.GetByIdAsync(request.RegisterId, cancellationToken);
        if (register is null)
            throw new RegisterNotFoundException(request.RegisterId);

        register.Close(request.EndingCash);

        _registerRepository.Update(register);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
} 