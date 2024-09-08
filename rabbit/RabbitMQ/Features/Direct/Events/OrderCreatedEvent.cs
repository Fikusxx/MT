using MassTransit;

namespace RabbitMQ.Features.Direct.Events;

public interface IEvent;

public sealed record OrderCreatedEvent : IEvent
{
    public Guid Id { get; init; } = NewId.NextGuid();
    public required string Number { get; init; }
    public required string Priority { get; init; }
}