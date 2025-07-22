using BuildingBlocks.Domain.Exceptions;
using PatientService.Domain.ValueObjects;

namespace PatientService.Domain.Exceptions;

public class PatientNotFoundException : DomainException
{
    public PatientNotFoundException()
        : base("Patient was not found.")
    {
    }

    public PatientNotFoundException(string message)
        : base(message)
    {
    }

    public PatientNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public PatientNotFoundException(PatientId patientId) 
        : base($"Patient with ID '{patientId}' was not found.")
    {
    }

    public PatientNotFoundException(MedicalRecordNumber mrn) 
        : base($"Patient with medical record number '{mrn}' was not found.")
    {
    }
}