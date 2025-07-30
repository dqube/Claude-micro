using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Infrastructure.Logging;
using BuildingBlocks.Infrastructure.Observability;
using System.Text.Json;

namespace PatientService.API.Examples;

/// <summary>
/// Integration test demonstrating PatientService redaction functionality
/// This can be run as a console application to verify redaction is working
/// </summary>
public class PatientRedactionIntegrationTest
{
    public static void RunTest()
    {
        Console.WriteLine("🏥 PatientService PHI Redaction Integration Test");
        Console.WriteLine("================================================");
        
        // Setup services similar to PatientService
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenTelemetry:ServiceName"] = "PatientService.API",
                ["OpenTelemetry:Redaction:Enabled"] = "true",
                ["OpenTelemetry:Redaction:RedactionText"] = "[PHI_REDACTED]",
                ["OpenTelemetry:Logging:Enabled"] = "true",
                ["OpenTelemetry:Exporters:Console:Enabled"] = "true"
            })
            .Build();

        var environment = new TestHostEnvironment();
        
        // Add OpenTelemetry with redaction
        services.AddOpenTelemetryObservability(configuration, environment);
        
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<PatientRedactionIntegrationTest>>();
        var redactionService = serviceProvider.GetRequiredService<IDataRedactionService>();
        
        // Test 1: Direct redaction service
        Console.WriteLine("\n🔍 Test 1: Direct Redaction Service");
        TestDirectRedaction(redactionService);
        
        // Test 2: Logger integration
        Console.WriteLine("\n📝 Test 2: Logger Integration");
        TestLoggerIntegration(logger);
        
        // Test 3: Patient data scenarios
        Console.WriteLine("\n👤 Test 3: Patient Data Scenarios");
        TestPatientDataScenarios(logger, redactionService);
        
        Console.WriteLine("\n✅ All tests completed! Check the output above to verify PHI data is properly redacted.");
    }
    
    private static void TestDirectRedaction(IDataRedactionService redactionService)
    {
        var sensitivePatientData = "Patient John Doe (MRN: 12345678) with SSN 123-45-6789 and email john.doe@hospital.com";
        var redacted = redactionService.RedactMessage(sensitivePatientData);
        
        Console.WriteLine($"Original: {sensitivePatientData}");
        Console.WriteLine($"Redacted: {redacted}");
        
        // Test JSON redaction
        var patientJson = JsonSerializer.Serialize(new
        {
            PatientId = "P123456",
            FirstName = "John",
            LastName = "Doe",
            SSN = "123-45-6789",
            Email = "john.doe@hospital.com",
            Phone = "555-123-4567",
            Diagnosis = "Type 2 Diabetes",
            MRN = "MRN12345678"
        });
        
        var redactedJson = redactionService.RedactJson(patientJson);
        Console.WriteLine($"\nOriginal JSON: {patientJson}");
        Console.WriteLine($"Redacted JSON: {redactedJson}");
    }
    
    private static void TestLoggerIntegration(ILogger logger)
    {
        // These should all be automatically redacted in the log output
        logger.LogInformation("Patient {PatientId} with MRN {MRN} registered", "P123456", "MRN12345678");
        logger.LogInformation("Contact update: Email {Email}, Phone {Phone}", "patient@example.com", "555-123-4567");
        logger.LogInformation("Diagnosis recorded: {Diagnosis} for patient {PatientId}", "Acute Myocardial Infarction", "P789012");
        logger.LogInformation("Prescription: {Medication} prescribed to {PatientId}", "Metformin 500mg", "P345678");
    }
    
    private static void TestPatientDataScenarios(ILogger logger, IDataRedactionService redactionService)
    {
        // Scenario 1: Patient Registration
        var registrationData = new PatientRegistrationData
        {
            FirstName = "Jane",
            LastName = "Smith",
            MRN = "MRN87654321",
            DateOfBirth = new DateTime(1985, 3, 15),
            SSN = "987-65-4321",
            Email = "jane.smith@email.com",
            Phone = "555-987-6543",
            Address = "123 Main St, Anytown, ST 12345",
            BloodType = "O+",
            Gender = "Female",
            EmergencyContact = "Robert Smith (555-111-2222)"
        };
        
        logger.LogInformation("New patient registration: {@Patient}", registrationData);
        
        // Scenario 2: Lab Results
        var labResults = new LabResultsData
        {
            PatientId = "P987654",
            TestType = "Complete Blood Count",
            Results = "WBC: 7.5, RBC: 4.2, Hemoglobin: 13.8",
            TestDate = DateTime.Now,
            Physician = "Dr. Emily Johnson",
            LabTechnician = "Tech Mike Wilson"
        };
        
        logger.LogInformation("Lab results received: {@LabResults}", labResults);
        
        // Scenario 3: Emergency Contact Update
        var emergencyContact = new EmergencyContactData
        {
            Name = "Sarah Johnson",
            Phone = "555-444-3333",
            Email = "sarah.johnson@email.com",
            Relationship = "Sister",
            Address = "456 Oak Ave, Another City, ST 54321"
        };
        
        logger.LogInformation("Emergency contact updated: {@EmergencyContact}", emergencyContact);
        
        // Scenario 4: Insurance Information
        var insurance = new InsuranceData
        {
            InsuranceId = "INS123456789",
            MemberNumber = "MEM987654321",
            GroupNumber = "GRP555444333",
            PolicyHolder = "Jane Smith",
            ProviderName = "HealthCare Plus",
            EffectiveDate = DateTime.Now.AddMonths(-6),
            ExpirationDate = DateTime.Now.AddMonths(6)
        };
        
        logger.LogInformation("Insurance information updated: {@Insurance}", insurance);
        
        // Scenario 5: Audit Log (Critical for compliance)
        logger.LogInformation("AUDIT: User {UserId} accessed patient {PatientId} record at {Timestamp}", 
            "user123", "P987654", DateTime.Now);
            
        // Scenario 6: API Integration Log
        var apiResponseData = """
        {
            "patientId": "P111222",
            "personalInfo": {
                "firstName": "Michael",
                "lastName": "Brown",
                "ssn": "111-22-3333",
                "email": "michael.brown@email.com"
            },
            "medicalInfo": {
                "bloodType": "A+",
                "allergies": ["Penicillin", "Shellfish"],
                "currentMedications": ["Lisinopril 10mg", "Metformin 1000mg"]
            }
        }
        """;
        
        var redactedApiResponse = redactionService.RedactJson(apiResponseData);
        logger.LogInformation("External API response: {ApiResponse}", redactedApiResponse);
    }
}

// Test host environment for the integration test
internal class TestHostEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = "Development";
    public string ApplicationName { get; set; } = "PatientService.API";
    public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
    public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = 
        new Microsoft.Extensions.FileProviders.NullFileProvider();
}

// You can run this test by calling PatientRedactionIntegrationTest.RunTest() 
// from your Program.cs in development mode or create a separate test console app