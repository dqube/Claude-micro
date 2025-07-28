using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using StoreService.Application.DTOs;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class CreateRegisterCommandHandler : ICommandHandler<CreateRegisterCommand, RegisterDto>
{
    private readonly IRepository<Register, RegisterId> _registerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRegisterCommandHandler(
        IRepository<Register, RegisterId> registerRepository,
        IUnitOfWork unitOfWork)
    {
        _registerRepository = registerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterDto> HandleAsync(CreateRegisterCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var registerId = RegisterId.From(0); // Will be set by database identity
        var register = new Register(registerId, request.StoreId, request.Name);

        await _registerRepository.AddAsync(register, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(register);
    }

    private static RegisterDto MapToDto(Register register)
    {
        return new RegisterDto
        {
            Id = register.Id.Value,
            StoreId = register.StoreId.Value,
            Name = register.Name,
            CurrentBalance = register.CurrentBalance,
            Status = register.Status.Name,
            IsOpen = register.IsOpen,
            LastOpen = register.LastOpen,
            LastClose = register.LastClose,
            CreatedAt = register.CreatedAt,
            UpdatedAt = register.UpdatedAt
        };
    }
} 