using BuildingBlocks.Domain.BusinessRules;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.BusinessRules;

public class CountryPricingMustHaveValidCountryRule : BusinessRuleBase
{
    private readonly CountryCode _countryCode;

    public CountryPricingMustHaveValidCountryRule(CountryCode countryCode)
    {
        _countryCode = countryCode;
    }

    public override bool IsBroken()
    {
        return _countryCode == null || string.IsNullOrWhiteSpace(_countryCode.Value) || _countryCode.Value.Length != 2;
    }

    public override string Message => "Country pricing must have a valid 2-letter country code";
}