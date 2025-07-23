using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using PatientService.Domain.Entities;
using PatientService.Domain.Exceptions;
using PatientService.Domain.ValueObjects;

namespace PatientService.Application.Commands;

public class UpdatePatientContactCommandHandler : ICommandHandler<UpdatePatientContactCommand>
{
    private readonly IRepository<Patient, PatientId> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePatientContactCommandHandler(
        IRepository<Patient, PatientId> patientRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdatePatientContactCommand request, CancellationToken cancellationToken)
    {
        var patientId = PatientId.From(request.PatientId);
        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        
        if (patient == null)
            throw new PatientNotFoundException(patientId);

        var email = new Email(request.Email);
        var phoneNumber = string.IsNullOrEmpty(request.PhoneNumber) 
            ? null 
            : new PhoneNumber(request.PhoneNumber);

        patient.UpdateContactInformation(email, phoneNumber);

        _patientRepository.Update(patient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}