using BuildingBlocks.Application.CQRS.Queries;
using CustomerService.Application.DTOs;
using CustomerService.Domain.ValueObjects;
using CustomerService.Domain.Repositories;

namespace CustomerService.Application.Queries;

public class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto?> HandleAsync(GetCustomerByIdQuery request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customerId = CustomerId.From(request.CustomerId);
        var customer = await _customerRepository.GetCustomerWithContactsAndAddressesAsync(customerId, cancellationToken);

        if (customer == null)
            return null;

        return new CustomerDto
        {
            Id = customer.Id.Value,
            UserId = customer.UserId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email?.Value,
            MembershipNumber = customer.MembershipNumber.Value,
            JoinDate = customer.JoinDate,
            ExpiryDate = customer.ExpiryDate,
            CountryCode = customer.CountryCode,
            LoyaltyPoints = customer.LoyaltyPoints,
            PreferredContactMethod = customer.PreferredContactMethod,
            PreferredAddressType = customer.PreferredAddressType,
            IsMembershipActive = customer.IsMembershipActive(),
            FullName = customer.GetFullName(),
            CreatedAt = customer.CreatedAt,
            LastUpdatedAt = customer.UpdatedAt,
            ContactNumbers = customer.ContactNumbers.Select(cn => new CustomerContactNumberDto
            {
                Id = cn.Id.Value,
                CustomerId = cn.CustomerId.Value,
                ContactNumberTypeId = cn.ContactNumberTypeId,
                PhoneNumber = cn.PhoneNumber.Value,
                IsPrimary = cn.IsPrimary,
                Verified = cn.Verified,
                CreatedAt = cn.CreatedAt,
                LastUpdatedAt = cn.UpdatedAt
            }).ToList(),
            Addresses = customer.Addresses.Select(addr => new CustomerAddressDto
            {
                Id = addr.Id.Value,
                CustomerId = addr.CustomerId.Value,
                AddressTypeId = addr.AddressTypeId,
                Line1 = addr.Line1,
                Line2 = addr.Line2,
                City = addr.City,
                State = addr.State,
                PostalCode = addr.PostalCode,
                CountryCode = addr.CountryCode,
                IsPrimary = addr.IsPrimary,
                FormattedAddress = addr.GetFormattedAddress(),
                CreatedAt = addr.CreatedAt,
                LastUpdatedAt = addr.UpdatedAt
            }).ToList()
        };
    }
} 