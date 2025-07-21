using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PatientService.Domain.Entities;
using PatientService.Domain.ValueObjects;
using PatientService.Infrastructure.Persistence;

namespace PatientService.Tests.Integration;

public class StronglyTypedIdIntegrationTests : IDisposable
{
    private readonly DbContextOptions<PatientDbContext> _options;
    private readonly PatientDbContext _context;

    public StronglyTypedIdIntegrationTests()
    {
        _options = new DbContextOptionsBuilder<PatientDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientDbContext(_options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Should_Save_And_Retrieve_Patient_With_StronglyTypedId()
    {
        // Arrange
        var patientId = PatientId.New();
        var patient = new Patient(
            patientId,
            "John",
            "Doe",
            new Email("john.doe@example.com"),
            new PhoneNumber("555-1234"),
            DateTime.Parse("1990-01-01")
        );

        // Act
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Retrieve
        var retrievedPatient = await _context.Patients.FindAsync(patientId);

        // Assert
        Assert.NotNull(retrievedPatient);
        Assert.Equal(patientId, retrievedPatient.Id);
        Assert.Equal("John", retrievedPatient.FirstName);
        Assert.Equal("Doe", retrievedPatient.LastName);
    }

    [Fact]
    public async Task Should_Query_Patient_By_StronglyTypedId()
    {
        // Arrange
        var patientId = PatientId.New();
        var patient = new Patient(
            patientId,
            "Jane",
            "Smith",
            new Email("jane.smith@example.com"),
            new PhoneNumber("555-5678"),
            DateTime.Parse("1985-05-15")
        );

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var retrievedPatient = await _context.Patients
            .Where(p => p.Id == patientId)
            .FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(retrievedPatient);
        Assert.Equal(patientId, retrievedPatient.Id);
    }

    [Fact]
    public async Task Should_Handle_Multiple_Patients_With_Different_StronglyTypedIds()
    {
        // Arrange
        var patient1Id = PatientId.New();
        var patient2Id = PatientId.New();

        var patient1 = new Patient(
            patient1Id,
            "Alice",
            "Johnson",
            new Email("alice@example.com"),
            new PhoneNumber("555-1111"),
            DateTime.Parse("1992-03-10")
        );

        var patient2 = new Patient(
            patient2Id,
            "Bob",
            "Wilson",
            new Email("bob@example.com"),
            new PhoneNumber("555-2222"),
            DateTime.Parse("1988-07-22")
        );

        // Act
        _context.Patients.AddRange(patient1, patient2);
        await _context.SaveChangesAsync();

        // Retrieve all patients
        var allPatients = await _context.Patients.ToListAsync();

        // Assert
        Assert.Equal(2, allPatients.Count);
        Assert.Contains(allPatients, p => p.Id == patient1Id);
        Assert.Contains(allPatients, p => p.Id == patient2Id);
    }

    [Fact]
    public async Task Should_Properly_Convert_PatientId_To_Guid_In_Database()
    {
        // Arrange
        var patientId = PatientId.New();
        var patient = new Patient(
            patientId,
            "Test",
            "User",
            new Email("test@example.com"),
            new PhoneNumber("555-9999"),
            DateTime.Parse("1995-12-01")
        );

        // Act
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Verify that the underlying Guid value is stored correctly
        var rawValue = await _context.Database.SqlQueryRaw<Guid>(
            "SELECT Id FROM Patients WHERE FirstName = 'Test'"
        ).FirstOrDefaultAsync();

        // Assert
        Assert.Equal(patientId.Value, rawValue);
    }

    [Fact]
    public void Should_Generate_Unique_PatientIds()
    {
        // Arrange & Act
        var id1 = PatientId.New();
        var id2 = PatientId.New();
        var id3 = PatientId.New();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id2, id3);
        Assert.NotEqual(id1, id3);
    }

    [Fact]
    public void Should_Create_PatientId_From_Existing_Guid()
    {
        // Arrange
        var guidValue = Guid.NewGuid();

        // Act
        var patientId = PatientId.From(guidValue);

        // Assert
        Assert.Equal(guidValue, patientId.Value);
    }

    [Fact]
    public void Should_Support_PatientId_Equality()
    {
        // Arrange
        var guidValue = Guid.NewGuid();
        var id1 = PatientId.From(guidValue);
        var id2 = PatientId.From(guidValue);

        // Act & Assert
        Assert.Equal(id1, id2);
        Assert.True(id1 == id2);
        Assert.False(id1 != id2);
        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}