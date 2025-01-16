namespace Orchestrator.Events;

public sealed record SendPushEvent
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
}