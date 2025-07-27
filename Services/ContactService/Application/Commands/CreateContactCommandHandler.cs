using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using ContactService.Domain.Entities;
using ContactService.Domain.Exceptions;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;

namespace ContactService.Application.Commands;

public class CreateContactCommandHandler : ICommandHandler<CreateContactCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContactCommandHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(CreateContactCommand command, CancellationToken cancellationToken)
    {
        var emailExists = await _contactRepository.ExistsWithEmailAsync(command.Email, cancellationToken);
        if (emailExists)
        {
            throw new DuplicateContactException(command.Email, true);
        }

        var email = new Email(command.Email);
        var contactType = ContactType.From(command.ContactType);
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

        var contact = new Contact(
            ContactId.New(),
            command.FirstName,
            command.LastName,
            email,
            contactType,
            phoneNumber,
            address,
            command.Company,
            command.JobTitle,
            command.Notes);

        await _contactRepository.AddAsync(contact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}