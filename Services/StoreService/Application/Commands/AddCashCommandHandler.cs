using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using StoreService.Domain.Entities;
using StoreService.Domain.Exceptions;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class AddCashCommandHandler : ICommandHandler<AddCashCommand>
{
    private readonly IRepository<Register, RegisterId> _registerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCashCommandHandler(
        IRepository<Register, RegisterId> registerRepository,
        IUnitOfWork unitOfWork)
    {
        _registerRepository = registerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(AddCashCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var register = await _registerRepository.GetByIdAsync(request.RegisterId, cancellationToken);
        if (register is null)
            throw new RegisterNotFoundException(request.RegisterId);

        register.AddCash(request.Amount, request.Note);

        _registerRepository.Update(register);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
} 