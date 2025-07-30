using BuildingBlocks.Domain.DomainEvents;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Events;

public class CountryNameUpdatedEvent : DomainEventBase
{
    public CountryCode CountryCode { get; }
    public CountryName OldName { get; }
    public CountryName NewName { get; }

    public CountryNameUpdatedEvent(CountryCode countryCode, CountryName oldName, CountryName newName)
    {
        CountryCode = countryCode;
        OldName = oldName;
        NewName = newName;
    }
} 