using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using CustomerService.Application.DTOs;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;
using CustomerService.Domain.Repositories;

namespace CustomerService.Application.Commands;

public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate email uniqueness if provided
        Email? email = null;
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            email = new Email(request.Email);
            if (await _customerRepository.EmailExistsAsync(email, cancellationToken))
            {
                throw new InvalidOperationException($"Customer with email '{request.Email}' already exists");
            }
        }

        // Generate or validate membership number
        MembershipNumber membershipNumber;
        if (!string.IsNullOrWhiteSpace(request.MembershipNumber))
        {
            membershipNumber = MembershipNumber.From(request.MembershipNumber);
            if (await _customerRepository.MembershipNumberExistsAsync(membershipNumber, cancellationToken))
            {
                throw new InvalidOperationException($"Customer with membership number '{request.MembershipNumber}' already exists");
            }
        }
        else
        {
            // Generate unique membership number
            do
            {
                membershipNumber = MembershipNumber.Generate();
            } while (await _customerRepository.MembershipNumberExistsAsync(membershipNumber, cancellationToken));
        }

        // Use UserId directly if provided
        var userId = request.UserId;

        // Create customer entity
        var customer = new Customer(
            CustomerId.New(),
            request.FirstName,
            request.LastName,
            email,
            membershipNumber,
            request.JoinDate ?? DateTime.UtcNow,
            request.ExpiryDate ?? DateTime.UtcNow.AddYears(1),
            request.CountryCode,
            userId);

        // Add to repository
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(customer);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
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