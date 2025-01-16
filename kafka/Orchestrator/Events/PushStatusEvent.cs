namespace Orchestrator.Events;

public sealed record PushStatusEvent
{
    // public required Guid Id { get; init; }
    public required long Id { get; init; }
    public required bool Success { get; init; }
}