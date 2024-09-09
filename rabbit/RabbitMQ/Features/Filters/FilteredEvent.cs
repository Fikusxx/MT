namespace RabbitMQ.Features.Filters;

public sealed record FilteredEvent
{
    public required bool ShouldFilter { get; init; }
}