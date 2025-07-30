using BuildingBlocks.Domain.Entities;
using EmployeeService.Domain.ValueObjects;
using EmployeeService.Domain.Events;

namespace EmployeeService.Domain.Entities;

public class Employee : AggregateRoot<EmployeeId>
{
    private readonly List<EmployeeContactNumber> _contactNumbers = [];
    private readonly List<EmployeeAddress> _addresses = [];

    public Guid UserId { get; private set; }
    public int StoreId { get; private set; }
    public EmployeeNumber EmployeeNumber { get; private set; }
    public DateTime HireDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }
    public Position Position { get; private set; }
    public int AuthLevel { get; private set; }
    public bool IsActive => TerminationDate == null;

    public IReadOnlyCollection<EmployeeContactNumber> ContactNumbers => _contactNumbers.AsReadOnly();
    public IReadOnlyCollection<EmployeeAddress> Addresses => _addresses.AsReadOnly();

    private Employee() : base(EmployeeId.New())
    {
        EmployeeNumber = new EmployeeNumber("temp");
        Position = new Position("temp");
    }

    public Employee(
        EmployeeId id,
        Guid userId,
        int storeId,
        EmployeeNumber employeeNumber,
        DateTime hireDate,
        Position position,
        int authLevel = 1) : base(id)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (storeId <= 0)
            throw new ArgumentException("StoreId must be greater than 0", nameof(storeId));
        if (authLevel < 1 || authLevel > 10)
            throw new ArgumentException("AuthLevel must be between 1 and 10", nameof(authLevel));

        UserId = userId;
        StoreId = storeId;
        EmployeeNumber = employeeNumber ?? throw new ArgumentNullException(nameof(employeeNumber));
        HireDate = hireDate;
        Position = position ?? throw new ArgumentNullException(nameof(position));
        AuthLevel = authLevel;

        AddDomainEvent(new EmployeeCreatedEvent(Id, UserId, StoreId, EmployeeNumber, Position, HireDate));
    }

    public void UpdatePosition(Position newPosition)
    {
        if (newPosition == null)
            throw new ArgumentNullException(nameof(newPosition));
        
        if (Position.Value == newPosition.Value) return;

        var oldPosition = Position;
        Position = newPosition;
        
        AddDomainEvent(new EmployeePositionUpdatedEvent(Id, oldPosition, newPosition));
    }

    public void UpdateAuthLevel(int newAuthLevel)
    {
        if (newAuthLevel < 1 || newAuthLevel > 10)
            throw new ArgumentException("AuthLevel must be between 1 and 10", nameof(newAuthLevel));
        
        if (AuthLevel == newAuthLevel) return;

        var oldLevel = AuthLevel;
        AuthLevel = newAuthLevel;
        
        AddDomainEvent(new EmployeeAuthLevelUpdatedEvent(Id, oldLevel, newAuthLevel));
    }

    public void Terminate(DateTime terminationDate)
    {
        if (terminationDate == default)
            throw new ArgumentException("TerminationDate cannot be default", nameof(terminationDate));
        
        if (TerminationDate.HasValue)
            throw new InvalidOperationException("Employee is already terminated");

        if (terminationDate < HireDate)
            throw new ArgumentException("Termination date cannot be before hire date", nameof(terminationDate));

        TerminationDate = terminationDate;
        
        AddDomainEvent(new EmployeeTerminatedEvent(Id, terminationDate));
    }

    public void Reactivate()
    {
        if (!TerminationDate.HasValue)
            throw new InvalidOperationException("Employee is not terminated");

        TerminationDate = null;
        
        AddDomainEvent(new EmployeeReactivatedEvent(Id));
    }

    public void AddContactNumber(EmployeeContactNumber contactNumber)
    {
        if (contactNumber == null)
            throw new ArgumentNullException(nameof(contactNumber));
        
        if (contactNumber.IsPrimary)
        {
            foreach (var existing in _contactNumbers.Where(c => c.ContactNumberTypeId == contactNumber.ContactNumberTypeId))
            {
                existing.SetAsPrimary(false);
            }
        }

        _contactNumbers.Add(contactNumber);
        AddDomainEvent(new EmployeeContactNumberAddedEvent(Id, contactNumber.Id, contactNumber.PhoneNumber));
    }

    public void RemoveContactNumber(ContactNumberId contactNumberId)
    {
        var contactNumber = _contactNumbers.FirstOrDefault(c => c.Id == contactNumberId);
        if (contactNumber == null)
            throw new ArgumentException("Contact number not found", nameof(contactNumberId));

        _contactNumbers.Remove(contactNumber);
        AddDomainEvent(new EmployeeContactNumberRemovedEvent(Id, contactNumberId));
    }

    public void AddAddress(EmployeeAddress address)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));
        
        if (address.IsPrimary)
        {
            foreach (var existing in _addresses.Where(a => a.AddressTypeId == address.AddressTypeId))
            {
                existing.SetAsPrimary(false);
            }
        }

        _addresses.Add(address);
        AddDomainEvent(new EmployeeAddressAddedEvent(Id, address.Id, address.AddressTypeId));
    }

    public void RemoveAddress(AddressId addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null)
            throw new ArgumentException("Address not found", nameof(addressId));

        _addresses.Remove(address);
        AddDomainEvent(new EmployeeAddressRemovedEvent(Id, addressId));
    }
}