namespace BuildingBlocks.Application.Outbox;

public enum OutboxMessageStatus
{
    Pending = 0,
    Scheduled = 1,
    Publishing = 2,
    Published = 3,
    Failed = 4,
    Discarded = 5
}