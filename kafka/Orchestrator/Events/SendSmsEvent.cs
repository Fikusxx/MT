namespace Orchestrator.Events;

public sealed record SendSmsEvent
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
}