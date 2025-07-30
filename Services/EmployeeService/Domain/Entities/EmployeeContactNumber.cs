using BuildingBlocks.Domain.Entities;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Entities;

public class EmployeeContactNumber : Entity<ContactNumberId>
{
    public EmployeeId EmployeeId { get; private set; }
    public int ContactNumberTypeId { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool Verified { get; private set; }

    private EmployeeContactNumber() { }

    public EmployeeContactNumber(
        ContactNumberId id,
        EmployeeId employeeId,
        int contactNumberTypeId,
        string phoneNumber,
        bool isPrimary = false) : base(id)
    {
        if (employeeId == null || employeeId.Value == Guid.Empty)
            throw new ArgumentException("EmployeeId cannot be empty", nameof(employeeId));
        if (contactNumberTypeId <= 0)
            throw new ArgumentException("ContactNumberTypeId must be greater than 0", nameof(contactNumberTypeId));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("PhoneNumber cannot be null or empty", nameof(phoneNumber));
        if (phoneNumber.Length > 20)
            throw new ArgumentException("PhoneNumber cannot exceed 20 characters", nameof(phoneNumber));

        EmployeeId = employeeId;
        ContactNumberTypeId = contactNumberTypeId;
        PhoneNumber = phoneNumber;
        IsPrimary = isPrimary;
        Verified = false;
    }

    public void SetAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    public void SetVerified(bool verified)
    {
        Verified = verified;
    }

    public void UpdatePhoneNumber(string newPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(newPhoneNumber))
            throw new ArgumentException("PhoneNumber cannot be null or empty", nameof(newPhoneNumber));
        if (newPhoneNumber.Length > 20)
            throw new ArgumentException("PhoneNumber cannot exceed 20 characters", nameof(newPhoneNumber));

        PhoneNumber = newPhoneNumber;
        Verified = false;
    }
}