using BuildingBlocks.Domain.DomainEvents;
using BuildingBlocks.Domain.Common;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Events;

public sealed class CustomerCreatedEvent : DomainEventBase
{
    public CustomerId CustomerId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Email? Email { get; }
    public MembershipNumber MembershipNumber { get; }
    public DateTime JoinDate { get; }

    public CustomerCreatedEvent(
        CustomerId customerId,
        string firstName,
        string lastName,
        Email? email,
        MembershipNumber membershipNumber,
        DateTime joinDate)
    {
        CustomerId = customerId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        MembershipNumber = membershipNumber;
        JoinDate = joinDate;
    }
} 