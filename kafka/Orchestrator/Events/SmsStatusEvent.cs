namespace Orchestrator.Events;

public sealed record SmsStatusEvent
{
    public required Guid Id { get; init; }
    public required bool Success { get; init; }
}