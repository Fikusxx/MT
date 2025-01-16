namespace Orchestrator.Events;

public sealed record FinalEvent
{
    public required Guid Id { get; init; }
    public required bool Success { get; init; }
}