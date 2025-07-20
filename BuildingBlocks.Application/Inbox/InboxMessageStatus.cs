namespace BuildingBlocks.Application.Inbox;

public enum InboxMessageStatus
{
    Pending = 0,
    Processing = 1,
    Processed = 2,
    Failed = 3,
    Discarded = 4
}