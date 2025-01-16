namespace Orchestrator.Events;

public sealed record CommunicationEvent
{
    // public required Guid Id { get; init; }
    public required long Id { get; init; }
    public required string Text { get; init; }
}