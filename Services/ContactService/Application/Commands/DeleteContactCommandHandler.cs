using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using ContactService.Domain.Exceptions;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;

namespace ContactService.Application.Commands;

public class DeleteContactCommandHandler : ICommandHandler<DeleteContactCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContactCommandHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(DeleteContactCommand command, CancellationToken cancellationToken)
    {
        var contactId = ContactId.From(command.ContactId);
        var contact = await _contactRepository.GetByIdAsync(contactId, cancellationToken);
        
        if (contact is null)
        {
            throw new ContactNotFoundException(contactId);
        }

        _contactRepository.Delete(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}