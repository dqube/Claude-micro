using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Common;

public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be null or empty", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be null or empty", nameof(country));

        Street = street.Trim();
        City = city.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
    }

    public string FullAddress => $"{Street}, {City}, {PostalCode}, {Country}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString() => FullAddress;
}