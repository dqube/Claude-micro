using Microsoft.Extensions.Logging;
using BuildingBlocks.Infrastructure.Logging;

namespace PatientService.API.Examples;

/// <summary>
/// Demonstrates how PHI (Protected Health Information) redaction works in PatientService
/// This is critical for HIPAA compliance in healthcare applications
/// </summary>
public class PatientRedactionExample
{
    private readonly ILogger<PatientRedactionExample> _logger;
    private readonly IDataRedactionService _redactionService;

    public PatientRedactionExample(ILogger<PatientRedactionExample> logger, IDataRedactionService redactionService)
    {
        _logger = logger;
        _redactionService = redactionService;
    }

    /// <summary>
    /// Example: Patient registration with automatic PHI redaction
    /// </summary>
    public void LogPatientRegistration(PatientRegistrationData patient)
    {
        // ❌ HIPAA Violation Without Redaction:
        // Patient registered: John Doe (MRN: 12345678, DOB: 1990-05-15, SSN: 123-45-6789, Email: john.doe@email.com)
        
        // ✅ HIPAA Compliant With Redaction:
        _logger.LogInformation("Patient registered: {@Patient}", patient);
        // Output: Patient registered: {"FirstName":"[PHI_REDACTED]","LastName":"[PHI_REDACTED]","MRN":"[PHI_REDACTED]",...}
    }

    /// <summary>
    /// Example: Medical diagnosis logging with redaction
    /// </summary>
    public void LogMedicalDiagnosis(string patientId, string diagnosis, string medication)
    {
        // All sensitive medical data is automatically redacted
        _logger.LogInformation("Patient {PatientId} diagnosed with {Diagnosis}, prescribed {Medication}", 
            patientId, diagnosis, medication);
        // Output: Patient [PHI_REDACTED] diagnosed with [PHI_REDACTED], prescribed [PHI_REDACTED]
    }

    /// <summary>
    /// Example: Patient contact update with redaction
    /// </summary>
    public void LogContactUpdate(PatientContactUpdate contact)
    {
        using (_logger.BeginScope("Updating patient contact information"))
        {
            _logger.LogInformation("Contact update received: {@ContactUpdate}", contact);
            // All PII fields (email, phone, address) are automatically redacted
        }
    }

    /// <summary>
    /// Example: Medical procedure logging
    /// </summary>
    public void LogMedicalProcedure(string patientId, string procedure, string physician)
    {
        _logger.LogInformation("Medical procedure scheduled: Patient {PatientId}, Procedure {Procedure}, Physician {Physician}", 
            patientId, procedure, physician);
        // Output: Medical procedure scheduled: Patient [PHI_REDACTED], Procedure [PHI_REDACTED], Physician [PHI_REDACTED]
    }

    /// <summary>
    /// Example: Lab results with automatic redaction
    /// </summary>
    public void LogLabResults(string patientId, LabResultsData labResults)
    {
        _logger.LogInformation("Lab results received for patient {PatientId}: {@LabResults}", 
            patientId, labResults);
        // All sensitive medical data and patient identifiers are redacted
    }

    /// <summary>
    /// Example: Emergency contact logging
    /// </summary>
    public void LogEmergencyContact(string patientId, EmergencyContactData emergencyContact)
    {
        _logger.LogInformation("Emergency contact updated for patient {PatientId}: {@EmergencyContact}", 
            patientId, emergencyContact);
        // Contact information is redacted for privacy
    }

    /// <summary>
    /// Example: Insurance information logging
    /// </summary>
    public void LogInsuranceUpdate(string patientId, InsuranceData insurance)
    {
        _logger.LogInformation("Insurance information updated for patient {PatientId}: {@Insurance}", 
            patientId, insurance);
        // Insurance IDs, member numbers, and personal data are redacted
    }

    /// <summary>
    /// Example: Manual redaction for custom scenarios
    /// </summary>
    public void LogCustomSensitiveData(string sensitiveHealthData)
    {
        // For custom scenarios, you can manually redact before logging
        var redactedData = _redactionService.RedactJson(sensitiveHealthData);
        _logger.LogInformation("Custom health data processed: {Data}", redactedData);
    }

    /// <summary>
    /// Example: Audit logging with redaction (important for compliance)
    /// </summary>
    public void LogAuditEvent(string userId, string action, string patientId, object details)
    {
        _logger.LogInformation("AUDIT: User {UserId} performed {Action} on patient {PatientId}. Details: {@Details}", 
            userId, action, patientId, details);
        // User IDs, patient IDs, and sensitive details are automatically redacted
    }
}

// Example data models that would be used in PatientService
public class PatientRegistrationData
{
    public string FirstName { get; set; } = string.Empty;        // Will be redacted
    public string LastName { get; set; } = string.Empty;         // Will be redacted
    public string MRN { get; set; } = string.Empty;              // Will be redacted
    public DateTime DateOfBirth { get; set; }                    // Will be redacted
    public string SSN { get; set; } = string.Empty;              // Will be redacted
    public string Email { get; set; } = string.Empty;            // Will be redacted
    public string Phone { get; set; } = string.Empty;            // Will be redacted
    public string Address { get; set; } = string.Empty;          // Will be redacted
    public string BloodType { get; set; } = string.Empty;        // Will be redacted
    public string Gender { get; set; } = string.Empty;           // Will be redacted
    public string EmergencyContact { get; set; } = string.Empty; // Will be redacted
}

public class PatientContactUpdate
{
    public string PatientId { get; set; } = string.Empty;        // Will be redacted
    public string Email { get; set; } = string.Empty;            // Will be redacted
    public string Phone { get; set; } = string.Empty;            // Will be redacted
    public string Address { get; set; } = string.Empty;          // Will be redacted
    public string EmergencyContact { get; set; } = string.Empty; // Will be redacted
}

public class LabResultsData
{
    public string PatientId { get; set; } = string.Empty;        // Will be redacted
    public string TestType { get; set; } = string.Empty;         // Will be redacted
    public string Results { get; set; } = string.Empty;          // Will be redacted
    public DateTime TestDate { get; set; }
    public string Physician { get; set; } = string.Empty;        // Will be redacted
    public string LabTechnician { get; set; } = string.Empty;    // Will be redacted
}

public class EmergencyContactData
{
    public string Name { get; set; } = string.Empty;             // Will be redacted
    public string Phone { get; set; } = string.Empty;            // Will be redacted
    public string Email { get; set; } = string.Empty;            // Will be redacted
    public string Relationship { get; set; } = string.Empty;     // Will be redacted
    public string Address { get; set; } = string.Empty;          // Will be redacted
}

public class InsuranceData
{
    public string InsuranceId { get; set; } = string.Empty;      // Will be redacted
    public string MemberNumber { get; set; } = string.Empty;     // Will be redacted
    public string GroupNumber { get; set; } = string.Empty;      // Will be redacted
    public string PolicyHolder { get; set; } = string.Empty;     // Will be redacted
    public string ProviderName { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}