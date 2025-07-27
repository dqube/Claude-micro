using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using ContactService.Domain.Exceptions;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;

namespace ContactService.Application.Commands;

public class UpdateContactCommandHandler : ICommandHandler<UpdateContactCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactCommandHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdateContactCommand command, CancellationToken cancellationToken)
    {
        var contactId = ContactId.From(command.ContactId);
        var contact = await _contactRepository.GetByIdAsync(contactId, cancellationToken);
        
        if (contact is null)
        {
            throw new ContactNotFoundException(contactId);
        }

        var email = new Email(command.Email);
        var phoneNumber = !string.IsNullOrWhiteSpace(command.PhoneNumber) ? new PhoneNumber(command.PhoneNumber) : null;
        
        Address? address = null;
        if (command.Address != null)
        {
            address = new Address(
                command.Address.Street,
                command.Address.City,
                command.Address.PostalCode,
                command.Address.Country);
        }

        contact.UpdateContactInformation(email, phoneNumber);
        contact.UpdateAddress(address);
        contact.UpdatePersonalInformation(command.FirstName, command.LastName, command.Company, command.JobTitle);
        contact.UpdateNotes(command.Notes);

        _contactRepository.Update(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}